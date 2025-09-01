// GearsManager.cs (in Config folder)

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

        /// <summary>
        /// Only responsible for wiring the Gears UI (no XML I/O here).
        /// Call this AFTER the configuration provider loads SettingsState.
        /// </summary>
        public static void InitializeConfiguration()
        {
            try
            {
                if (!IsGearsAvailable)
                {
                    AdnLogger.Debug("Gears not available; XML provider will be used (handled by the configuration service).");
                    return;
                }

                InitializeGearsIntegration();
            }
            catch (System.Exception ex)
            {
                AdnLogger.Error($"Failed to initialize Gears UI: {ex.Message}");
                // Do not rethrow; the XML provider can still be used.
            }
        }

        private static void InitializeGearsIntegration()
        {
            // Create our Gears integration instance and hook UI <-> SettingsState.
            _gearsIntegration = new GearsIntegration();
            AdnLogger.Debug("Gears integration initialized. Settings are available in Options > Mods.");
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