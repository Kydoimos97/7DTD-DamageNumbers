// in AngelDamageNumbers (no Gears refs)

using System;
using System.IO;
using System.Reflection;
using AngelDamageNumbers.Config;
using AngelDamageNumbers.Utilities;

namespace AngelDamageNumbers.Gears
{
    public static class GearsManager
    {
        private static bool? _gearsAvailable;

        public static bool IsGearsAvailable
        {
            get
            {
                if (_gearsAvailable.HasValue) return _gearsAvailable.Value;
                _gearsAvailable = GearsDetector.DetectGearsAvailability();
                return _gearsAvailable.Value;
            }
        }

        public static void InitializeConfiguration()
        {
            try
            {
                string err = null!; // declare upfront

                if (IsGearsAvailable && TryLoadGearsShim(out err))
                {
                    AdnLogger.Log("Using Gears configuration via shim");
                    // Still load XML as seed/defaults so your settings have starting values
                    XmlHandler.LoadConfig();
                }
                else
                {
                    if (!string.IsNullOrEmpty(err))
                        AdnLogger.Warning($"Gears shim load failed: {err}. Falling back to XML.");

                    InitializeXmlConfiguration();
                }
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Failed to initialize configuration: {ex.Message}");
                InitializeXmlConfiguration();
            }
        }

        private static bool TryLoadGearsShim(out string error)
        {
            error = null!;
            try
            {
                // Locate this core DLL’s folder
                var coreAsm = typeof(GearsManager).Assembly.Location;
                var baseDir = Path.GetDirectoryName(coreAsm);

                // Our shim is in Mods/Angel_DamageNumbers/Optional/AngelDamageNumbers.Gears.dll
                var shimPath = Path.Combine(baseDir ?? ".", "Optional", "AngelDamageNumbers.Gears.dll");
                if (!File.Exists(shimPath))
                {
                    error = $"Shim not found at {shimPath}";
                    return false;
                }

                var shimAsm = Assembly.LoadFrom(shimPath);

                // The type name below must match your shim’s public class that implements IGearsModApi
                // e.g. namespace Gears; public class GearsIntegration : IGearsModApi
                var t = shimAsm.GetType("Gears.GearsIntegration", throwOnError: true);

                Activator.CreateInstance(t);

                AdnLogger.Debug("Gears shim loaded and integration instance created.");
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static void InitializeXmlConfiguration()
        {
            try
            {
                XmlHandler.LoadConfigAsync();
                AdnLogger.Debug("XML configuration initialized successfully");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"XML configuration failed: {ex.Message}");
                throw;
            }
        }

        public static void CleanupStatics()
        {
            _gearsAvailable = null;
            GearsDetector.ClearCache();
            AdnLogger.Debug("GearsManager static references cleaned up");
        }
    }
}
