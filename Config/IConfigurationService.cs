using Gears;
using UnityEngine;
using Utilities;

namespace Config
{
    public interface IConfigurationService
    {
        // Debug settings
        bool EnableDebugLogging { get; set; }

        // Damage number settings
        int MinimumDamageThreshold { get; set; }
        float DamageNumberCooldown { get; set; }
        int FontSize { get; set; }
        float TextLifetime { get; set; }
        float FloatSpeed { get; set; }
        Vector3 TextOffset { get; set; }

        // Color settings
        Color NormalDamageColor { get; set; }
        Color HeadshotDamageColor { get; set; }
        Color KillDamageColor { get; set; }
        Color HeadshotKillDamageColor { get; set; }

        // Crosshair marker settings
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

        // Advanced settings
        bool PlayerDamageOnly { get; set; }
        bool RandomizePosition { get; set; }
        float PositionRandomness { get; set; }
        bool ScaleTextByDamage { get; set; }
        float MinScale { get; set; }
        float MaxScale { get; set; }
        int MaxDamageForScale { get; set; }

        // Text styling settings
        bool EnableOutline { get; set; }
        Color OutlineColor { get; set; }
        float OutlineThickness { get; set; }
        string FontName { get; set; }

        // Configuration management
        void LoadConfiguration();
        void SaveConfiguration();
        void ResetToDefaults();
        string GetConfigurationInfo();
        string GetSettingsSummary();
        void ValidateSettings();
    }
    public static class ConfigurationService
    {
        private static IConfigurationService _current;

        public static IConfigurationService Current => _current ?? (_current = CreateConfigService());

        private static IConfigurationService CreateConfigService()
        {
            if (GearsManager.IsGearsAvailable)
            {
                AdnLogger.Log("Using Gears configuration service");
                return new GearsConfigurationService();
            }
            else
            {
                AdnLogger.Log("Using XML configuration service");
                return new XmlConfigurationService();
            }
        }
        public static void RefreshConfigurationService()
        {
            _current = null;
            _current = CreateConfigService();
        }
        public static void CleanupStatics()
        {
            _current = null;
            AdnLogger.Debug("ConfigurationService static references cleaned up");
        }
    }
    public class GearsConfigurationService : IConfigurationService
    {
        // Delegate to existing ConfigurationService.Current for now - in a full refactor, this would directly access Gears
        public bool EnableDebugLogging
        {
            get => ConfigurationService.Current.EnableDebugLogging;
            set => ConfigurationService.Current.EnableDebugLogging = value;
        }

        public int MinimumDamageThreshold
        {
            get => ConfigurationService.Current.MinimumDamageThreshold;
            set => ConfigurationService.Current.MinimumDamageThreshold = value;
        }

        public float DamageNumberCooldown
        {
            get => ConfigurationService.Current.DamageNumberCooldown;
            set => ConfigurationService.Current.DamageNumberCooldown = value;
        }

        public int FontSize
        {
            get => ConfigurationService.Current.FontSize;
            set => ConfigurationService.Current.FontSize = value;
        }

        public float TextLifetime
        {
            get => ConfigurationService.Current.TextLifetime;
            set => ConfigurationService.Current.TextLifetime = value;
        }

        public float FloatSpeed
        {
            get => ConfigurationService.Current.FloatSpeed;
            set => ConfigurationService.Current.FloatSpeed = value;
        }

        public Vector3 TextOffset
        {
            get => ConfigurationService.Current.TextOffset;
            set => ConfigurationService.Current.TextOffset = value;
        }

        public Color NormalDamageColor
        {
            get => ConfigurationService.Current.NormalDamageColor;
            set => ConfigurationService.Current.NormalDamageColor = value;
        }

        public Color HeadshotDamageColor
        {
            get => ConfigurationService.Current.HeadshotDamageColor;
            set => ConfigurationService.Current.HeadshotDamageColor = value;
        }

        public Color KillDamageColor
        {
            get => ConfigurationService.Current.KillDamageColor;
            set => ConfigurationService.Current.KillDamageColor = value;
        }

        public Color HeadshotKillDamageColor
        {
            get => ConfigurationService.Current.HeadshotKillDamageColor;
            set => ConfigurationService.Current.HeadshotKillDamageColor = value;
        }

        public bool EnableCrosshairMarkers
        {
            get => ConfigurationService.Current.EnableCrosshairMarkers;
            set => ConfigurationService.Current.EnableCrosshairMarkers = value;
        }

        public float MarkerDuration
        {
            get => ConfigurationService.Current.MarkerDuration;
            set => ConfigurationService.Current.MarkerDuration = value;
        }

        public int MarkerFontSize
        {
            get => ConfigurationService.Current.MarkerFontSize;
            set => ConfigurationService.Current.MarkerFontSize = value;
        }

        public string NormalHitMarker
        {
            get => ConfigurationService.Current.NormalHitMarker;
            set => ConfigurationService.Current.NormalHitMarker = value;
        }

        public string KillMarker
        {
            get => ConfigurationService.Current.KillMarker;
            set => ConfigurationService.Current.KillMarker = value;
        }

        public string HeadshotMarker
        {
            get => ConfigurationService.Current.HeadshotMarker;
            set => ConfigurationService.Current.HeadshotMarker = value;
        }

        public string HeadshotKillMarker
        {
            get => ConfigurationService.Current.HeadshotKillMarker;
            set => ConfigurationService.Current.HeadshotKillMarker = value;
        }

        public Color NormalMarkerColor
        {
            get => ConfigurationService.Current.NormalMarkerColor;
            set => ConfigurationService.Current.NormalMarkerColor = value;
        }

        public Color KillMarkerColor
        {
            get => ConfigurationService.Current.KillMarkerColor;
            set => ConfigurationService.Current.KillMarkerColor = value;
        }

        public Color HeadshotMarkerColor
        {
            get => ConfigurationService.Current.HeadshotMarkerColor;
            set => ConfigurationService.Current.HeadshotMarkerColor = value;
        }

        public Color HeadshotKillMarkerColor
        {
            get => ConfigurationService.Current.HeadshotKillMarkerColor;
            set => ConfigurationService.Current.HeadshotKillMarkerColor = value;
        }

        public bool PlayerDamageOnly
        {
            get => ConfigurationService.Current.PlayerDamageOnly;
            set => ConfigurationService.Current.PlayerDamageOnly = value;
        }

        public bool RandomizePosition
        {
            get => ConfigurationService.Current.RandomizePosition;
            set => ConfigurationService.Current.RandomizePosition = value;
        }

        public float PositionRandomness
        {
            get => ConfigurationService.Current.PositionRandomness;
            set => ConfigurationService.Current.PositionRandomness = value;
        }

        public bool ScaleTextByDamage
        {
            get => ConfigurationService.Current.ScaleTextByDamage;
            set => ConfigurationService.Current.ScaleTextByDamage = value;
        }

        public float MinScale
        {
            get => ConfigurationService.Current.MinScale;
            set => ConfigurationService.Current.MinScale = value;
        }

        public float MaxScale
        {
            get => ConfigurationService.Current.MaxScale;
            set => ConfigurationService.Current.MaxScale = value;
        }

        public int MaxDamageForScale
        {
            get => ConfigurationService.Current.MaxDamageForScale;
            set => ConfigurationService.Current.MaxDamageForScale = value;
        }

        public bool EnableOutline
        {
            get => ConfigurationService.Current.EnableOutline;
            set => ConfigurationService.Current.EnableOutline = value;
        }

        public Color OutlineColor
        {
            get => ConfigurationService.Current.OutlineColor;
            set => ConfigurationService.Current.OutlineColor = value;
        }

        public float OutlineThickness
        {
            get => ConfigurationService.Current.OutlineThickness;
            set => ConfigurationService.Current.OutlineThickness = value;
        }

        public string FontName
        {
            get => ConfigurationService.Current.FontName;
            set => ConfigurationService.Current.FontName = value;
        }

        public void LoadConfiguration()
        {
            XmlHandler.LoadConfigAsync();
        }

        public void SaveConfiguration()
        {
            XmlHandler.SaveSettings();
        }

        public void ResetToDefaults()
        {
            ConfigurationService.Current.ResetToDefaults();
        }

        public string GetConfigurationInfo()
        {
            return "Configuration: Gears in-game UI (Options > Mods > Angel's Enhanced Damage Numbers)";
        }

        public string GetSettingsSummary()
        {
            return ConfigurationService.Current.GetSettingsSummary();
        }

        public void ValidateSettings()
        {
            ConfigurationService.Current.ValidateSettings();
        }
    }
    public class XmlConfigurationService : IConfigurationService
    {
        // Delegate to existing ConfigurationService.Current for now - in a full refactor, this would directly access XML
        public bool EnableDebugLogging
        {
            get => ConfigurationService.Current.EnableDebugLogging;
            set => ConfigurationService.Current.EnableDebugLogging = value;
        }

        public int MinimumDamageThreshold
        {
            get => ConfigurationService.Current.MinimumDamageThreshold;
            set => ConfigurationService.Current.MinimumDamageThreshold = value;
        }

        public float DamageNumberCooldown
        {
            get => ConfigurationService.Current.DamageNumberCooldown;
            set => ConfigurationService.Current.DamageNumberCooldown = value;
        }

        public int FontSize
        {
            get => ConfigurationService.Current.FontSize;
            set => ConfigurationService.Current.FontSize = value;
        }

        public float TextLifetime
        {
            get => ConfigurationService.Current.TextLifetime;
            set => ConfigurationService.Current.TextLifetime = value;
        }

        public float FloatSpeed
        {
            get => ConfigurationService.Current.FloatSpeed;
            set => ConfigurationService.Current.FloatSpeed = value;
        }

        public Vector3 TextOffset
        {
            get => ConfigurationService.Current.TextOffset;
            set => ConfigurationService.Current.TextOffset = value;
        }

        public Color NormalDamageColor
        {
            get => ConfigurationService.Current.NormalDamageColor;
            set => ConfigurationService.Current.NormalDamageColor = value;
        }

        public Color HeadshotDamageColor
        {
            get => ConfigurationService.Current.HeadshotDamageColor;
            set => ConfigurationService.Current.HeadshotDamageColor = value;
        }

        public Color KillDamageColor
        {
            get => ConfigurationService.Current.KillDamageColor;
            set => ConfigurationService.Current.KillDamageColor = value;
        }

        public Color HeadshotKillDamageColor
        {
            get => ConfigurationService.Current.HeadshotKillDamageColor;
            set => ConfigurationService.Current.HeadshotKillDamageColor = value;
        }

        public bool EnableCrosshairMarkers
        {
            get => ConfigurationService.Current.EnableCrosshairMarkers;
            set => ConfigurationService.Current.EnableCrosshairMarkers = value;
        }

        public float MarkerDuration
        {
            get => ConfigurationService.Current.MarkerDuration;
            set => ConfigurationService.Current.MarkerDuration = value;
        }

        public int MarkerFontSize
        {
            get => ConfigurationService.Current.MarkerFontSize;
            set => ConfigurationService.Current.MarkerFontSize = value;
        }

        public string NormalHitMarker
        {
            get => ConfigurationService.Current.NormalHitMarker;
            set => ConfigurationService.Current.NormalHitMarker = value;
        }

        public string KillMarker
        {
            get => ConfigurationService.Current.KillMarker;
            set => ConfigurationService.Current.KillMarker = value;
        }

        public string HeadshotMarker
        {
            get => ConfigurationService.Current.HeadshotMarker;
            set => ConfigurationService.Current.HeadshotMarker = value;
        }

        public string HeadshotKillMarker
        {
            get => ConfigurationService.Current.HeadshotKillMarker;
            set => ConfigurationService.Current.HeadshotKillMarker = value;
        }

        public Color NormalMarkerColor
        {
            get => ConfigurationService.Current.NormalMarkerColor;
            set => ConfigurationService.Current.NormalMarkerColor = value;
        }

        public Color KillMarkerColor
        {
            get => ConfigurationService.Current.KillMarkerColor;
            set => ConfigurationService.Current.KillMarkerColor = value;
        }

        public Color HeadshotMarkerColor
        {
            get => ConfigurationService.Current.HeadshotMarkerColor;
            set => ConfigurationService.Current.HeadshotMarkerColor = value;
        }

        public Color HeadshotKillMarkerColor
        {
            get => ConfigurationService.Current.HeadshotKillMarkerColor;
            set => ConfigurationService.Current.HeadshotKillMarkerColor = value;
        }

        public bool PlayerDamageOnly
        {
            get => ConfigurationService.Current.PlayerDamageOnly;
            set => ConfigurationService.Current.PlayerDamageOnly = value;
        }

        public bool RandomizePosition
        {
            get => ConfigurationService.Current.RandomizePosition;
            set => ConfigurationService.Current.RandomizePosition = value;
        }

        public float PositionRandomness
        {
            get => ConfigurationService.Current.PositionRandomness;
            set => ConfigurationService.Current.PositionRandomness = value;
        }

        public bool ScaleTextByDamage
        {
            get => ConfigurationService.Current.ScaleTextByDamage;
            set => ConfigurationService.Current.ScaleTextByDamage = value;
        }

        public float MinScale
        {
            get => ConfigurationService.Current.MinScale;
            set => ConfigurationService.Current.MinScale = value;
        }

        public float MaxScale
        {
            get => ConfigurationService.Current.MaxScale;
            set => ConfigurationService.Current.MaxScale = value;
        }

        public int MaxDamageForScale
        {
            get => ConfigurationService.Current.MaxDamageForScale;
            set => ConfigurationService.Current.MaxDamageForScale = value;
        }

        public bool EnableOutline
        {
            get => ConfigurationService.Current.EnableOutline;
            set => ConfigurationService.Current.EnableOutline = value;
        }

        public Color OutlineColor
        {
            get => ConfigurationService.Current.OutlineColor;
            set => ConfigurationService.Current.OutlineColor = value;
        }

        public float OutlineThickness
        {
            get => ConfigurationService.Current.OutlineThickness;
            set => ConfigurationService.Current.OutlineThickness = value;
        }

        public string FontName
        {
            get => ConfigurationService.Current.FontName;
            set => ConfigurationService.Current.FontName = value;
        }

        public void LoadConfiguration()
        {
            XmlHandler.LoadConfig();
        }

        public void SaveConfiguration()
        {
            AdnLogger.Debug("XML configuration is read-only at runtime. Modify the config file and restart.");
        }

        public void ResetToDefaults()
        {
            ConfigurationService.Current.ResetToDefaults();
        }

        public string GetConfigurationInfo()
        {
            return "Configuration: XML file (edit ConfigurationService.Current.xml in mod folder)";
        }

        public string GetSettingsSummary()
        {
            return ConfigurationService.Current.GetSettingsSummary();
        }

        public void ValidateSettings()
        {
            ConfigurationService.Current.ValidateSettings();
        }
    }
}