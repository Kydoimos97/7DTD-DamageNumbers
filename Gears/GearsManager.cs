// GearsManager.cs (for Config folder)

using System;
using System.IO;
using System.Reflection;
using System.Xml;
using Config;
using Utilities;

namespace Gears
{
    public static class GearsManager
    {
        private static bool? _gearsAvailable;
        private static GearsIntegration _gearsIntegration;

        public static bool IsGearsAvailable
        {
            get
            {
                if (_gearsAvailable.HasValue)
                    return _gearsAvailable.Value;

                _gearsAvailable = GearsDetector.DetectGearsAvailability();
                return _gearsAvailable.Value;
            }
        }

        public static void InitializeConfiguration()
        {
            try
            {
                if (IsGearsAvailable)
                    InitializeGearsIntegration();
                else
                    InitializeXmlConfiguration();
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Failed to initialize configuration: {ex.Message}");
                // Fallback to XML configuration
                try
                {
                    InitializeXmlConfiguration();
                    AdnLogger.Log("Fallback to XML configuration successful");
                }
                catch (Exception fallbackEx)
                {
                    AdnLogger.Error($"Fallback configuration also failed: {fallbackEx.Message}");
                    throw; // Re-throw since we can't recover
                }
            }
        }

        private static void InitializeGearsIntegration()
        {
            try
            {
                // Create our Gears integration instance
                _gearsIntegration = new GearsIntegration();

                // The GearsSettingsManager will automatically detect and call our IGearsModApi implementation
                AdnLogger.Debug("Gears integration initialized. Settings will be available in the Options > Mods menu.");

                // Still load XML config as fallback/initial values, but Gears will override
                XmlHandler.LoadConfig();
            }
            catch (ReflectionTypeLoadException ex)
            {
                AdnLogger.Error($"Failed to load Gears types: {ex.Message}");
                if (ex.LoaderExceptions != null)
                    foreach (var loaderEx in ex.LoaderExceptions)
                        if (loaderEx != null)
                            AdnLogger.Error($"Loader exception: {loaderEx.Message}");

                throw;
            }
            catch (TargetInvocationException ex)
            {
                AdnLogger.Error($"Failed to invoke Gears method: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }
            catch (MissingMethodException ex)
            {
                AdnLogger.Error($"Gears method not found - incompatible version: {ex.Message}");
                throw;
            }
        }

        private static void InitializeXmlConfiguration()
        {
            try
            {
                XmlHandler.LoadConfigAsync();
                AdnLogger.Debug("XML configuration initialized successfully");
            }
            catch (FileNotFoundException ex)
            {
                AdnLogger.Warning($"Configuration file not found: {ex.Message}. Creating default configuration.");
                // XmlHandler.LoadConfigAsync() will create a default config if none exists
            }
            catch (XmlException ex)
            {
                AdnLogger.Error($"Invalid XML configuration: {ex.Message}");
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                AdnLogger.Error($"Access denied when reading configuration: {ex.Message}");
                throw;
            }
        }

        public static void SaveCurrentSettings()
        {
            try
            {
                // For XML config, we could implement a save method if needed
                AdnLogger.Debug(IsGearsAvailable ? "Settings automatically saved by Gears" : "XML configuration is read-only at runtime. Modify the config file and restart.");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Failed to save settings: {ex.Message}");
            }
        }

        public static void ResetToDefaults()
        {
            try
            {
                if (IsGearsAvailable && _gearsIntegration != null)
                    // For Gears, we'd need to implement reset functionality
                    AdnLogger.Debug("Reset functionality: Delete the mod's settings in Gears and restart");
                else
                    // For XML, delete the config file and reload
                    AdnLogger.Debug("To reset XML config, delete ConfigurationService.Current.xml and restart the game");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Failed to reset to defaults: {ex.Message}");
            }
        }

        public static string GetConfigurationInfo()
        {
            if (IsGearsAvailable) return "Configuration: Gears in-game UI (Options > Mods > Angel's Enhanced Damage Numbers)";

            return "Configuration: XML file (edit ConfigurationService.Current.xml in mod folder)";
        }

        public static bool IsGearsFeatureAvailable(string featureName)
        {
            if (!IsGearsAvailable) return false;

            try
            {
                return GearsDetector.IsFeatureAvailable(featureName);
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error checking Gears feature '{featureName}': {ex.Message}");
                return false;
            }
        }

        public static string GetGearsVersion()
        {
            if (!IsGearsAvailable) return "Not installed";

            try
            {
                return GearsDetector.GetGearsVersion();
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error getting Gears version: {ex.Message}");
                return "ModMetaData unknown";
            }
        }

        public static void RefreshGearsDetection()
        {
            _gearsAvailable = null;
            GearsDetector.ClearCache();
        }

        public static void CleanupStatics()
        {
            _gearsIntegration = null;
            _gearsAvailable = null;
            GearsDetector.ClearCache();
            AdnLogger.Debug("GearsManager static references cleaned up");
        }
    }
}