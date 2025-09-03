using System;
using AngelDamageNumbers.Gears;
using AngelDamageNumbers.Utilities;
using UnityEngine;

namespace AngelDamageNumbers.Config
{
    public interface IConfigurationService
    {
        // Debug
        bool EnableDebugLogging { get; set; }

        // Damage numbers
        int MinimumDamageThreshold { get; set; }
        float DamageNumberCooldown { get; set; }
        int FontSize { get; set; }
        float TextLifetime { get; set; }
        float FloatSpeed { get; set; }
        Vector3 TextOffset { get; set; }

        // Colors
        Color NormalDamageColor { get; set; }
        Color HeadshotDamageColor { get; set; }
        Color KillDamageColor { get; set; }
        Color HeadshotKillDamageColor { get; set; }

        // Crosshair markers
        bool EnableCrosshairMarkers { get; set; }
        float MarkerDuration { get; set; }
        int MarkerFontSize { get; set; }
        string NormalHitMarker { get; set; }
        string KillMarker { get; set; }
        string HeadshotMarker { get; set; }
        string HeadshotKillMarker { get; set; }
        Color NormalMarkerColor { get; set; }
        Color KillMarkerColor { get; set; }
        Color HeadshotMarkerColor { get; set; }
        Color HeadshotKillMarkerColor { get; set; }

        // Advanced
        bool PlayerDamageOnly { get; set; }
        bool RandomizePosition { get; set; }
        float PositionRandomness { get; set; }
        bool ScaleTextByDamage { get; set; }
        float MinScale { get; set; }
        float MaxScale { get; set; }
        int MaxDamageForScale { get; set; }

        // Text styling
        bool EnableOutline { get; set; }
        Color OutlineColor { get; set; }
        float OutlineThickness { get; set; }
        string FontName { get; set; }

        // Lifecycle
        void LoadConfiguration();
        void SaveConfiguration();
        void ResetToDefaults();
        string GetConfigurationInfo();
        string GetSettingsSummary();
        void ValidateSettings();
    }

    public static class ConfigurationService
    {
        private static IConfigurationService _current = null!;
        public static IConfigurationService Current => _current;

        private static IConfigurationService Create()
        {
            if (GearsManager.IsGearsAvailable)
            {
                AdnLogger.Log("Using Gears configuration service");
                return new GearsConfigurationService();
            }

            AdnLogger.Log("Using XML configuration service");
            return new XmlConfigurationService();
        }

        public static void RefreshConfigurationService() => _current = Create();

        public static void CleanupStatics()
        {
            _current = null!;
            AdnLogger.Debug("ConfigurationService static references cleaned up");
        }
    }

    // Keep all validation / summary logic here so neither service needs to call ConfigurationService.Current
    internal static class SettingsHelpers
    {
        public static void ApplyDefaults()
    {
        // === DEBUG SETTINGS ===
        SettingsState.EnableDebugLogging = false;

        // === DAMAGE NUMBER SETTINGS ===
        SettingsState.MinimumDamageThreshold = 2;
        SettingsState.DamageNumberCooldown = 0.0f;
        SettingsState.FontSize = 20;
        SettingsState.TextLifetime = 1.5f;
        SettingsState.FloatSpeed = 0.85f;
        SettingsState.TextOffset = new Vector3(0.0f, 1.5f, 0.0f);

        // === COLOR SETTINGS ===
        SettingsState.NormalDamageColor = new Color(0.75f, 0.75f, 0.75f);         // #949494
        SettingsState.HeadshotDamageColor = new Color(0.7f, 0.5f, 0.0f);          // #B88511
        SettingsState.KillDamageColor = new Color(0.5f, 0.0f, 0.0f);              // #750606
        SettingsState.HeadshotKillDamageColor = new Color(0.4f, 0.0f, 0.0f);      // #470202

        // === CROSSHAIR MARKER SETTINGS ===
        SettingsState.EnableCrosshairMarkers = true;
        SettingsState.MarkerDuration = 0.35f;
        SettingsState.MarkerFontSize = 30;
        SettingsState.NormalHitMarker = "×";
        SettingsState.KillMarker = "×";
        SettingsState.HeadshotMarker = "×";
        SettingsState.HeadshotKillMarker = "X";
        SettingsState.NormalMarkerColor = new Color(0.75f, 0.75f, 0.75f);         // #949494
        SettingsState.KillMarkerColor = new Color(0.7f, 0.5f, 0.0f);              // #750606
        SettingsState.HeadshotMarkerColor = new Color(0.5f, 0.0f, 0.0f);          // #B88511
        SettingsState.HeadshotKillMarkerColor = new Color(0.4f, 0.0f, 0.0f);      // #470202

        // === ADVANCED SETTINGS ===
        SettingsState.PlayerDamageOnly = true;
        SettingsState.RandomizePosition = true;
        SettingsState.PositionRandomness = 0.25f;
        SettingsState.ScaleTextByDamage = true;
        SettingsState.MinScale = 1.0f;
        SettingsState.MaxScale = 2.0f;
        SettingsState.MaxDamageForScale = 100;

        // === TEXT STYLING SETTINGS ===
        SettingsState.EnableOutline = true;
        SettingsState.OutlineColor = Color.black;
        SettingsState.OutlineThickness = 0.2f;
        SettingsState.FontName = "LiberationSans";
    }


        public static void Validate()
        {
            if (SettingsState.FontSize < 6) SettingsState.FontSize = 6;
            if (SettingsState.TextLifetime < 0.05f) SettingsState.TextLifetime = 0.05f;
            if (SettingsState.FloatSpeed < 0f) SettingsState.FloatSpeed = 0f;
            if (SettingsState.MarkerDuration < 0f) SettingsState.MarkerDuration = 0f;
            if (SettingsState.MinScale <= 0f) SettingsState.MinScale = 0.1f;
            if (SettingsState.MaxScale < SettingsState.MinScale) SettingsState.MaxScale = SettingsState.MinScale;
            if (SettingsState.MaxDamageForScale <= 0) SettingsState.MaxDamageForScale = 1;
        }

        public static string Summary()
        {
            return $"Debug:{SettingsState.EnableDebugLogging}, " +
                   $"Font:{SettingsState.FontName} {SettingsState.FontSize}px, " +
                   $"Lifetime:{SettingsState.TextLifetime}s, Float:{SettingsState.FloatSpeed}, " +
                   $"Markers:{(SettingsState.EnableCrosshairMarkers ? "On" : "Off")}";
        }
    }

    // -------- GEARS service (binds to SettingsState) --------
    public class GearsConfigurationService : IConfigurationService
    {
        // All properties ONLY touch SettingsState — safe for UI bindings
        public bool EnableDebugLogging { get => SettingsState.EnableDebugLogging; set => SettingsState.EnableDebugLogging = value; }
        public int MinimumDamageThreshold { get => SettingsState.MinimumDamageThreshold; set => SettingsState.MinimumDamageThreshold = value; }
        public float DamageNumberCooldown { get => SettingsState.DamageNumberCooldown; set => SettingsState.DamageNumberCooldown = value; }
        public int FontSize { get => SettingsState.FontSize; set => SettingsState.FontSize = value; }
        public float TextLifetime { get => SettingsState.TextLifetime; set => SettingsState.TextLifetime = value; }
        public float FloatSpeed { get => SettingsState.FloatSpeed; set => SettingsState.FloatSpeed = value; }
        public Vector3 TextOffset { get => SettingsState.TextOffset; set => SettingsState.TextOffset = value; }

        public Color NormalDamageColor { get => SettingsState.NormalDamageColor; set => SettingsState.NormalDamageColor = value; }
        public Color HeadshotDamageColor { get => SettingsState.HeadshotDamageColor; set => SettingsState.HeadshotDamageColor = value; }
        public Color KillDamageColor { get => SettingsState.KillDamageColor; set => SettingsState.KillDamageColor = value; }
        public Color HeadshotKillDamageColor { get => SettingsState.HeadshotKillDamageColor; set => SettingsState.HeadshotKillDamageColor = value; }

        public bool EnableCrosshairMarkers { get => SettingsState.EnableCrosshairMarkers; set => SettingsState.EnableCrosshairMarkers = value; }
        public float MarkerDuration { get => SettingsState.MarkerDuration; set => SettingsState.MarkerDuration = value; }
        public int MarkerFontSize { get => SettingsState.MarkerFontSize; set => SettingsState.MarkerFontSize = value; }
        public string NormalHitMarker { get => SettingsState.NormalHitMarker; set => SettingsState.NormalHitMarker = value; }
        public string KillMarker { get => SettingsState.KillMarker; set => SettingsState.KillMarker = value; }
        public string HeadshotMarker { get => SettingsState.HeadshotMarker; set => SettingsState.HeadshotMarker = value; }
        public string HeadshotKillMarker { get => SettingsState.HeadshotKillMarker; set => SettingsState.HeadshotKillMarker = value; }
        public Color NormalMarkerColor { get => SettingsState.NormalMarkerColor; set => SettingsState.NormalMarkerColor = value; }
        public Color KillMarkerColor { get => SettingsState.KillMarkerColor; set => SettingsState.KillMarkerColor = value; }
        public Color HeadshotMarkerColor { get => SettingsState.HeadshotMarkerColor; set => SettingsState.HeadshotMarkerColor = value; }
        public Color HeadshotKillMarkerColor { get => SettingsState.HeadshotKillMarkerColor; set => SettingsState.HeadshotKillMarkerColor = value; }

        public bool PlayerDamageOnly { get => SettingsState.PlayerDamageOnly; set => SettingsState.PlayerDamageOnly = value; }
        public bool RandomizePosition { get => SettingsState.RandomizePosition; set => SettingsState.RandomizePosition = value; }
        public float PositionRandomness { get => SettingsState.PositionRandomness; set => SettingsState.PositionRandomness = value; }
        public bool ScaleTextByDamage { get => SettingsState.ScaleTextByDamage; set => SettingsState.ScaleTextByDamage = value; }
        public float MinScale { get => SettingsState.MinScale; set => SettingsState.MinScale = value; }
        public float MaxScale { get => SettingsState.MaxScale; set => SettingsState.MaxScale = value; }
        public int MaxDamageForScale { get => SettingsState.MaxDamageForScale; set => SettingsState.MaxDamageForScale = value; }

        public bool EnableOutline { get => SettingsState.EnableOutline; set => SettingsState.EnableOutline = value; }
        public Color OutlineColor { get => SettingsState.OutlineColor; set => SettingsState.OutlineColor = value; }
        public float OutlineThickness { get => SettingsState.OutlineThickness; set => SettingsState.OutlineThickness = value; }
        public string FontName { get => SettingsState.FontName; set => SettingsState.FontName = value; }

        public void LoadConfiguration()
        {
            // If you persist through XML even with Gears, let XmlHandler populate SettingsState.
            XmlHandler.LoadConfigAsync();
        }

        public void SaveConfiguration() => XmlHandler.SaveSettings();
        public void ResetToDefaults() => SettingsHelpers.ApplyDefaults();
        public string GetConfigurationInfo() => "Configuration: Gears in-game UI (Options > Mods > Angel's Enhanced Damage Numbers)";
        public string GetSettingsSummary() => SettingsHelpers.Summary();
        public void ValidateSettings() => SettingsHelpers.Validate();
    }

    // -------- XML service (also binds to SettingsState) --------
    public class XmlConfigurationService : IConfigurationService
    {
        public bool EnableDebugLogging { get => SettingsState.EnableDebugLogging; set => SettingsState.EnableDebugLogging = value; }
        public int MinimumDamageThreshold { get => SettingsState.MinimumDamageThreshold; set => SettingsState.MinimumDamageThreshold = value; }
        public float DamageNumberCooldown { get => SettingsState.DamageNumberCooldown; set => SettingsState.DamageNumberCooldown = value; }
        public int FontSize { get => SettingsState.FontSize; set => SettingsState.FontSize = value; }
        public float TextLifetime { get => SettingsState.TextLifetime; set => SettingsState.TextLifetime = value; }
        public float FloatSpeed { get => SettingsState.FloatSpeed; set => SettingsState.FloatSpeed = value; }
        public Vector3 TextOffset { get => SettingsState.TextOffset; set => SettingsState.TextOffset = value; }

        public Color NormalDamageColor { get => SettingsState.NormalDamageColor; set => SettingsState.NormalDamageColor = value; }
        public Color HeadshotDamageColor { get => SettingsState.HeadshotDamageColor; set => SettingsState.HeadshotDamageColor = value; }
        public Color KillDamageColor { get => SettingsState.KillDamageColor; set => SettingsState.KillDamageColor = value; }
        public Color HeadshotKillDamageColor { get => SettingsState.HeadshotKillDamageColor; set => SettingsState.HeadshotKillDamageColor = value; }

        public bool EnableCrosshairMarkers { get => SettingsState.EnableCrosshairMarkers; set => SettingsState.EnableCrosshairMarkers = value; }
        public float MarkerDuration { get => SettingsState.MarkerDuration; set => SettingsState.MarkerDuration = value; }
        public int MarkerFontSize { get => SettingsState.MarkerFontSize; set => SettingsState.MarkerFontSize = value; }
        public string NormalHitMarker { get => SettingsState.NormalHitMarker; set => SettingsState.NormalHitMarker = value; }
        public string KillMarker { get => SettingsState.KillMarker; set => SettingsState.KillMarker = value; }
        public string HeadshotMarker { get => SettingsState.HeadshotMarker; set => SettingsState.HeadshotMarker = value; }
        public string HeadshotKillMarker { get => SettingsState.HeadshotKillMarker; set => SettingsState.HeadshotKillMarker = value; }
        public Color NormalMarkerColor { get => SettingsState.NormalMarkerColor; set => SettingsState.NormalMarkerColor = value; }
        public Color KillMarkerColor { get => SettingsState.KillMarkerColor; set => SettingsState.KillMarkerColor = value; }
        public Color HeadshotMarkerColor { get => SettingsState.HeadshotMarkerColor; set => SettingsState.HeadshotMarkerColor = value; }
        public Color HeadshotKillMarkerColor { get => SettingsState.HeadshotKillMarkerColor; set => SettingsState.HeadshotKillMarkerColor = value; }

        public bool PlayerDamageOnly { get => SettingsState.PlayerDamageOnly; set => SettingsState.PlayerDamageOnly = value; }
        public bool RandomizePosition { get => SettingsState.RandomizePosition; set => SettingsState.RandomizePosition = value; }
        public float PositionRandomness { get => SettingsState.PositionRandomness; set => SettingsState.PositionRandomness = value; }
        public bool ScaleTextByDamage { get => SettingsState.ScaleTextByDamage; set => SettingsState.ScaleTextByDamage = value; }
        public float MinScale { get => SettingsState.MinScale; set => SettingsState.MinScale = value; }
        public float MaxScale { get => SettingsState.MaxScale; set => SettingsState.MaxScale = value; }
        public int MaxDamageForScale { get => SettingsState.MaxDamageForScale; set => SettingsState.MaxDamageForScale = value; }

        public bool EnableOutline { get => SettingsState.EnableOutline; set => SettingsState.EnableOutline = value; }
        public Color OutlineColor { get => SettingsState.OutlineColor; set => SettingsState.OutlineColor = value; }
        public float OutlineThickness { get => SettingsState.OutlineThickness; set => SettingsState.OutlineThickness = value; }
        public string FontName { get => SettingsState.FontName; set => SettingsState.FontName = value; }

        public void LoadConfiguration() => XmlHandler.LoadConfig();
        public void SaveConfiguration() => XmlHandler.SaveSettings();
        public void ResetToDefaults() => SettingsHelpers.ApplyDefaults();
        public string GetConfigurationInfo() => "Configuration: XML file (edit ConfigurationService.Current.xml in mod folder)";
        public string GetSettingsSummary() => SettingsHelpers.Summary();
        public void ValidateSettings() => SettingsHelpers.Validate();
    }

}
