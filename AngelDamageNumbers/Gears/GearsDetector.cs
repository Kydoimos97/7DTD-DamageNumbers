using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AngelDamageNumbers.Utilities;

namespace AngelDamageNumbers.Gears
{
    public static class GearsDetector
    {
        private static readonly HashSet<string> _gearsNames = new HashSet<string> { "GearsAPI", "Gears" };
        private static List<Assembly> _gearsAssemblies = new List<Assembly>();
        private static bool _detectionCached;
        private static bool _cachedResult;

        public static bool DetectGearsAvailability()
        {
            if (_detectionCached) return _cachedResult;

            try
            {
                _gearsAssemblies = FindGearsAssemblies();

                if (_gearsAssemblies.Count > 0)
                {
                    foreach (var a in _gearsAssemblies)
                        AdnLogger.Debug($"Found Gears assembly: {a.FullName}");

                    var ok = ValidateGearsTypes();
                    _cachedResult = ok;
                    AdnLogger.Debug(ok
                        ? "Gears detected and validated! Using Gears for in-game configuration."
                        : "Gears assemblies found but required types missing. Using XML configuration.");
                }
                else
                {
                    _cachedResult = false;
                    AdnLogger.Debug("Gears not detected. Using XML configuration.");
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                _cachedResult = false;
                AdnLogger.Error($"Error loading Gears types: {ex.Message}");
                LogLoaderExceptions(ex);
            }
            catch (FileNotFoundException ex)
            {
                _cachedResult = false;
                AdnLogger.Debug($"Gears assembly not found: {ex.Message}. Using XML configuration.");
            }
            catch (BadImageFormatException ex)
            {
                _cachedResult = false;
                AdnLogger.Error($"Invalid Gears assembly format: {ex.Message}. Using XML configuration.");
            }
            catch (Exception ex)
            {
                _cachedResult = false;
                AdnLogger.Error($"Error detecting Gears: {ex.Message}. Using XML configuration.");
            }

            _detectionCached = true;
            return _cachedResult;
        }

        static IEnumerable<Type> SafeGetTypes(Assembly a)
        {
            try { return a.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null); }
        }

        static List<Assembly> FindGearsAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => _gearsNames.Contains(a.GetName().Name))
                .ToList();
        }

        static Type FindBySimpleName(IEnumerable<Assembly> asms, string simpleName)
        {
            return asms.SelectMany(SafeGetTypes).FirstOrDefault(t => t != null && t.Name == simpleName);
        }

        static bool ValidateGearsTypes()
        {
            var asms = FindGearsAssemblies();
            if (asms.Count == 0) return false;

            var iGearsModApi      = FindBySimpleName(asms, "IGearsModApi");
            var iGearsMod         = FindBySimpleName(asms, "IGearsMod");
            var iModGlobalSettings= FindBySimpleName(asms, "IModGlobalSettings");
            if (iGearsModApi == null || iGearsMod == null || iModGlobalSettings == null) return false;

            var hasSwitch = FindBySimpleName(asms, "ISwitchGlobalSetting") != null;
            var hasSlider = FindBySimpleName(asms, "ISliderGlobalSetting") != null;
            var hasColor  = FindBySimpleName(asms, "IColorSelectorGlobalSetting") != null;

            return hasSwitch && hasSlider && hasColor;
        }

        public static bool IsFeatureAvailable(string fullOrSimpleName)
        {
            if (_gearsAssemblies.Count == 0) return false;
            try
            {
                bool found = _gearsAssemblies.Any(a => a.GetType(fullOrSimpleName, false) != null);
                if (!found)
                    found = _gearsAssemblies.SelectMany(SafeGetTypes).Any(t => t != null && t.Name == fullOrSimpleName);

                AdnLogger.Debug($"Gears feature '{fullOrSimpleName}' availability: {found}");
                return found;
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error checking Gears feature '{fullOrSimpleName}': {ex.Message}");
                return false;
            }
        }

        public static bool HasColorSelectorType()
        {
            // Look up the interface by name in the GearsAPI assembly
            return Type.GetType(
                "GearsAPI.Settings.Global.IColorSelectorGlobalSetting, GearsAPI",
                throwOnError: false
            ) != null;
        }

        public static string GetGearsVersion()
        {
            if (_gearsAssemblies.Count == 0) return "Not installed";
            try
            {
                var asm = _gearsAssemblies.FirstOrDefault(a => a.GetName().Name == "GearsAPI") ?? _gearsAssemblies[0];
                var v = asm.GetName().Version;
                var s = v != null ? v.ToString() : "ModMetaData unknown";
                AdnLogger.Debug($"Gears version detected: {s}");
                return s;
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error getting Gears version: {ex.Message}");
                return "ModMetaData unknown";
            }
        }

        public static string GetGearsInfo()
        {
            if (_gearsAssemblies.Count == 0) return "Gears not detected";
            try
            {
                var asm = _gearsAssemblies.FirstOrDefault(a => a.GetName().Name == "GearsAPI") ?? _gearsAssemblies[0];
                var name = asm.GetName();
                return $"{name.Name} {name.Version} loaded from {asm.Location}";
            }
            catch (Exception ex)
            {
                return $"Gears detected but info unavailable: {ex.Message}";
            }
        }

        public static void ClearCache()
        {
            _detectionCached = false;
            _cachedResult = false;
            _gearsAssemblies.Clear();
            AdnLogger.Debug("Gears detection cache cleared");
        }

        public static Assembly GetGearsAssembly()
        {
            if (!DetectGearsAvailability() || _gearsAssemblies.Count == 0) return null;
            return _gearsAssemblies.FirstOrDefault(a => a.GetName().Name == "GearsAPI") ?? _gearsAssemblies[0];
        }

        private static void LogLoaderExceptions(ReflectionTypeLoadException ex)
        {
            if (ex.LoaderExceptions == null) return;
            foreach (var e in ex.LoaderExceptions)
                if (e != null) AdnLogger.Error($"Gears loader exception: {e.Message}");
        }

        public static bool ValidateCompatibility()
        {
            if (!DetectGearsAvailability()) return false;
            try
            {
                var version = GetGearsVersion();
                if (version == "ModMetaData unknown" || version == "Not installed")
                {
                    AdnLogger.Warning("Unable to determine Gears version - compatibility uncertain");
                    return true;
                }
                AdnLogger.Debug($"Gears version {version} appears compatible");
                return true;
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error validating Gears compatibility: {ex.Message}");
                return false;
            }
        }
    }
}
