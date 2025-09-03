// GearsIntegration.cs (for Config folder)
#if GEARS
using System;
using System.Globalization;
using AngelDamageNumbers.Utilities;
using AngelDamageNumbers.Gears;
using GearsAPI.Settings;
using GearsAPI.Settings.Global;
using GearsAPI.Settings.World;
using UnityEngine;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable UnusedParameter.Local
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace AngelDamageNumbers.Gears
{
    public class GearsIntegration : IGearsModApi
    {
        private bool _registered;

        // Correct typed setting references for efficient access
        private IGearsMod _modInstance;
        public static IModGlobalSettings GlobalSettings;
        public static GearsSettings Settings { get; private set; }
        private bool _supportsColorSelector;
        private string _gearsVersion;

        public void InitMod(IGearsMod modInstance)
        {
            _modInstance = modInstance;
            _gearsVersion = GearsDetector.GetGearsVersion();
            _supportsColorSelector = GearsDetector.HasColorSelectorType();

            AdnLogger.Log($"Gears integration initialized! | Version: {_gearsVersion} | hasColorSupport: {_supportsColorSelector}");

        }

        public void OnGlobalSettingsLoaded(IModGlobalSettings? globalSettings)
        {
            AdnLogger.Log("[Gears] OnGlobalSettingsLoaded() called");
            GlobalSettings = globalSettings ?? GlobalSettings;

            if (!_registered || Settings == null || Settings.EnableDebugLogging == null)
            {
                AdnLogger.Warning("[Gears] settings not registered yet; creating lazily in OnGlobalSettingsLoaded");
                InitSettings();
                InitCallBacks();
                _registered = true;
            }

            // quick sanity log
            AdnLogger.Debug(
                $"handles: dbg={Settings?.EnableDebugLogging!=null}, thr={Settings?.MinimumDamageThreshold!=null}, cd={Settings?.DamageNumberCooldown!=null}, fs={Settings?.FontSize!=null}, life={Settings?.TextLifetime!=null}, spd={Settings?.FloatSpeed!=null}");

            AdnLogger.Log("[Gears] global settings applied");
        }

        public void OnWorldSettingsLoaded(IModWorldSettings worldSettings)
        {
            AdnLogger.Debug("Gears world settings loaded!");
        }

        private void InitCallBacks()
        {
            _ = new GearsCallBacks();
        }

        private void InitSettings()
        {
            AdnLogger.Log("[Gears] initSettings() starting");

            if (GearsIntegration.GlobalSettings == null)
            {
                AdnLogger.Error("[Gears] GlobalSettings is NULL in initSettings!");
                throw new ArgumentNullException(nameof(GlobalSettings));
            }

            try
            {
                Settings = new GearsSettings(GlobalSettings);
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"[Gears] Exception in initSettings: {ex}");
                throw;
            }
            AdnLogger.Log("[Gears] initSettings() finished successfully");
        }

    }
}
#endif