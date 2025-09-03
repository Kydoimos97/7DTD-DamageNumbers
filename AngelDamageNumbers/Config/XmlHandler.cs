using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;
using AngelDamageNumbers.Managers;
using AngelDamageNumbers.Utilities;
using UnityEngine;

namespace AngelDamageNumbers.Config
{
    public class XmlHandler
    {
        private const string CurrentRootTag = "AngelDamageNumbersConfig";

        private static readonly string ConfigPath = Path.Combine(
            Application.dataPath, "..", "Mods", "Angel_DamageNumbers", "AngelDamageNumbersConfig.xml");

        public static void LoadConfigAsync()
        {
            CoroutineManager.Instance.StartCoroutine(LoadConfigCoroutine());
        }

        public static void LoadConfig()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    AdnLogger.Debug("Config file not found, creating default config...");
                    CreateDefaultConfig();
                    AdnLogger.Debug("Default config created successfully!");
                    return;
                }

                AdnLogger.Debug("Loading existing config...");
                LoadExistingConfig();
                AdnLogger.Debug($"Config loaded! Debug logging: {(SettingsState.EnableDebugLogging ? "ENABLED" : "disabled")}");
                AdnLogger.Debug("Debug logging is now active!");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Config load failed: {ex.Message}");
            }
        }

        private static IEnumerator LoadConfigCoroutine()
        {
            yield return null;

            if (!File.Exists(ConfigPath))
            {
                AdnLogger.Debug("Config file not found, creating default config...");
                yield return CreateAndLogConfig();
                yield break;
            }

            try
            {
                AdnLogger.Debug("Loading existing config...");
                LoadExistingConfig();
                AdnLogger.Debug($"Config loaded! Debug logging: {(SettingsState.EnableDebugLogging ? "ENABLED" : "disabled")}");
                AdnLogger.Debug("Debug logging is now active!");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Config load failed: {ex.Message}");
            }
        }

        private static IEnumerator CreateAndLogConfig()
        {
            yield return CreateDefaultConfigAsync();
            AdnLogger.Debug("Default config created successfully!");
        }

        private static void LoadExistingConfig()
        {
            // Only try migration + load if the file exists
            if (!File.Exists(ConfigPath))
            {
                AdnLogger.Log($"No existing config found at {ConfigPath}, using defaults.");
                return;
                // Now load the (possibly migrated) config normally

            }

            var doc = new XmlDocument();
            doc.Load(ConfigPath);

            // Load debug settings first
            SettingsState.EnableDebugLogging = GetBoolValue(doc, CurrentRootTag, "Debug/EnableDebugLogging", SettingsState.EnableDebugLogging);

            // Load damage number settings
            SettingsState.MinimumDamageThreshold = GetIntValue(doc, CurrentRootTag, "DamageNumbers/MinimumDamageThreshold", SettingsState.MinimumDamageThreshold);
            SettingsState.DamageNumberCooldown = GetFloatValue(doc, CurrentRootTag, "DamageNumbers/DamageNumberCooldown", SettingsState.DamageNumberCooldown);
            SettingsState.FontSize = GetIntValue(doc, CurrentRootTag, "DamageNumbers/FontSize", SettingsState.FontSize);
            SettingsState.TextLifetime = GetFloatValue(doc, CurrentRootTag, "DamageNumbers/TextLifetime", SettingsState.TextLifetime);
            SettingsState.FloatSpeed = GetFloatValue(doc, CurrentRootTag, "DamageNumbers/FloatSpeed", SettingsState.FloatSpeed);
            SettingsState.TextOffset = GetVector3Value(doc, CurrentRootTag, "DamageNumbers/TextOffset", SettingsState.TextOffset);

            // Load color settings
            SettingsState.NormalDamageColor = GetColorValue(doc, CurrentRootTag, "Colors/NormalDamageColor", SettingsState.NormalDamageColor);
            SettingsState.HeadshotDamageColor = GetColorValue(doc, CurrentRootTag, "Colors/HeadshotDamageColor", SettingsState.HeadshotDamageColor);
            SettingsState.KillDamageColor = GetColorValue(doc, CurrentRootTag, "Colors/KillDamageColor", SettingsState.KillDamageColor);
            SettingsState.HeadshotKillDamageColor = GetColorValue(doc, CurrentRootTag, "Colors/HeadshotKillDamageColor", SettingsState.HeadshotKillDamageColor);

            // Load crosshair settings
            SettingsState.EnableCrosshairMarkers = GetBoolValue(doc, CurrentRootTag, "CrosshairMarkers/EnableCrosshairMarkers", SettingsState.EnableCrosshairMarkers);
            SettingsState.MarkerDuration = GetFloatValue(doc, CurrentRootTag, "CrosshairMarkers/MarkerDuration", SettingsState.MarkerDuration);
            SettingsState.MarkerFontSize = GetIntValue(doc, CurrentRootTag, "CrosshairMarkers/MarkerFontSize", SettingsState.MarkerFontSize);
            SettingsState.NormalHitMarker = GetStringValue(doc, CurrentRootTag, "CrosshairMarkers/NormalHitMarker", SettingsState.NormalHitMarker);
            SettingsState.KillMarker = GetStringValue(doc, CurrentRootTag, "CrosshairMarkers/KillMarker", SettingsState.KillMarker);
            SettingsState.HeadshotMarker = GetStringValue(doc, CurrentRootTag, "CrosshairMarkers/HeadshotMarker", SettingsState.HeadshotMarker);
            SettingsState.HeadshotKillMarker = GetStringValue(doc, CurrentRootTag, "CrosshairMarkers/HeadshotKillMarker", SettingsState.HeadshotKillMarker);
            SettingsState.NormalMarkerColor = GetColorValue(doc, CurrentRootTag, "CrosshairMarkers/NormalMarkerColor", SettingsState.NormalMarkerColor);
            SettingsState.KillMarkerColor = GetColorValue(doc, CurrentRootTag, "CrosshairMarkers/KillMarkerColor", SettingsState.KillMarkerColor);
            SettingsState.HeadshotMarkerColor = GetColorValue(doc, CurrentRootTag, "CrosshairMarkers/HeadshotMarkerColor", SettingsState.HeadshotMarkerColor);
            SettingsState.HeadshotKillMarkerColor = GetColorValue(doc, CurrentRootTag, "CrosshairMarkers/HeadshotKillMarkerColor", SettingsState.HeadshotKillMarkerColor);

            // Load text styling settings (may not exist in old format)
            SettingsState.FontName = GetStringValue(doc, CurrentRootTag, "TextStyling/FontName", SettingsState.FontName);
            SettingsState.EnableOutline = GetBoolValue(doc, CurrentRootTag, "TextStyling/EnableOutline", SettingsState.EnableOutline);
            SettingsState.OutlineColor = GetColorValue(doc, CurrentRootTag, "TextStyling/OutlineColor", SettingsState.OutlineColor);
            SettingsState.OutlineThickness = GetFloatValue(doc, CurrentRootTag, "TextStyling/OutlineThickness", SettingsState.OutlineThickness);

            // Load advanced settings
            SettingsState.PlayerDamageOnly = GetBoolValue(doc, CurrentRootTag, "Advanced/PlayerDamageOnly", SettingsState.PlayerDamageOnly);
            SettingsState.RandomizePosition = GetBoolValue(doc, CurrentRootTag, "Advanced/RandomizePosition", SettingsState.RandomizePosition);
            SettingsState.PositionRandomness = GetFloatValue(doc, CurrentRootTag, "Advanced/PositionRandomness", SettingsState.PositionRandomness);
            SettingsState.ScaleTextByDamage = GetBoolValue(doc, CurrentRootTag, "Advanced/ScaleTextByDamage", SettingsState.ScaleTextByDamage);
            SettingsState.MinScale = GetFloatValue(doc, CurrentRootTag, "Advanced/MinScale", SettingsState.MinScale);
            SettingsState.MaxScale = GetFloatValue(doc, CurrentRootTag, "Advanced/MaxScale", SettingsState.MaxScale);
            SettingsState.MaxDamageForScale = GetIntValue(doc, CurrentRootTag, "Advanced/MaxDamageForScale", SettingsState.MaxDamageForScale);
        }

        private static IEnumerator CreateDefaultConfigAsync()
        {
            yield return null;

            var configCreated = false;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath) ?? string.Empty);
                yield return null;

                var doc = CreateConfigDocument();
                yield return null;

                doc.Save(ConfigPath);
                configCreated = true;
            }
            finally
            {
                if (!configCreated || !File.Exists(ConfigPath)) AdnLogger.Error("Failed to create default config file. Using built-in defaults.");
            }
        }

        private static void CreateDefaultConfig()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath) ?? string.Empty);
                var doc = CreateConfigDocument();
                doc.Save(ConfigPath);
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Failed to create default config: {ex.Message}");
            }
        }

        private static XmlDocument CreateConfigDocument()
        {
            var doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declaration);

            var root = doc.CreateElement(CurrentRootTag);
            doc.AppendChild(root);

            // Add version element first
            var versionElement = doc.CreateElement("ModMetaData");
            versionElement.InnerText = ModMetaData.ConfigVersion;
            root.AppendChild(versionElement);

            // Add main comment
            var comment = doc.CreateComment(@"
    Angel's Enhanced Damage Numbers Mod Configuration
    
    Colors: Use hex format like #FFFFFF (white), #FF0000 (red), #e6b400 (gold)
            Can also use 8-digit format #RRGGBBAA for transparency: #FF000080 (semi-transparent red)
    
    Symbols: You can use Unicode symbols like × ☠ ✖ ⚡ ★ ♦ ● ▲ ✓ ➤
    
    Most IDEs will show color previews for hex values and provide color pickers!
    ");
            root.AppendChild(comment);

            // Create all sections
            CreateDebugSection(doc, root);
            CreateDamageNumbersSection(doc, root);
            CreateColorsSection(doc, root);
            CreateCrosshairMarkersSection(doc, root);
            CreateTextStylingSection(doc, root);
            CreateAdvancedSection(doc, root);

            return doc;
        }

        private static void CreateDebugSection(XmlDocument doc, XmlElement root)
        {
            var debugSection = doc.CreateElement("Debug");
            AddElement(doc, debugSection, "EnableDebugLogging", SettingsState.EnableDebugLogging.ToString(),
                "Enable debug messages in Unity console - set to true to troubleshoot issues (default: false)");
            root.AppendChild(debugSection);
        }

        private static void CreateDamageNumbersSection(XmlDocument doc, XmlElement root)
        {
            var damageSection = doc.CreateElement("DamageNumbers");
            AddElement(doc, damageSection, "MinimumDamageThreshold", SettingsState.MinimumDamageThreshold.ToString(),
                "Minimum damage to show numbers (0 = show all damage)");
            AddElement(doc, damageSection, "DamageNumberCooldown", SettingsState.DamageNumberCooldown.ToString(CultureInfo.InvariantCulture),
                "Minimum time between damage numbers in seconds (prevents spam)");
            AddElement(doc, damageSection, "FontSize", SettingsState.FontSize.ToString(),
                "Size of damage text (default: 20)");
            AddElement(doc, damageSection, "TextLifetime", SettingsState.TextLifetime.ToString(CultureInfo.InvariantCulture),
                "How long text is visible in seconds (default: 1.2)");
            AddElement(doc, damageSection, "FloatSpeed", SettingsState.FloatSpeed.ToString(CultureInfo.InvariantCulture),
                "Speed text floats upward (default: 0.85)");
            AddElement(doc, damageSection, "TextOffset",
                $"{SettingsState.TextOffset.x},{SettingsState.TextOffset.y},{SettingsState.TextOffset.z}",
                "Offset from entity position in X,Y,Z format (default: 0,1.5,0)");
            root.AppendChild(damageSection);
        }

        private static void CreateColorsSection(XmlDocument doc, XmlElement root)
        {
            var colorsSection = doc.CreateElement("Colors");
            AddColorElement(doc, colorsSection, "NormalDamageColor", SettingsState.NormalDamageColor, "Normal damage color (default: white)");
            AddColorElement(doc, colorsSection, "HeadshotDamageColor", SettingsState.HeadshotDamageColor, "Headshot damage color (default: gold)");
            AddColorElement(doc, colorsSection, "KillDamageColor", SettingsState.KillDamageColor, "Killing blow color (default: red)");
            AddColorElement(doc, colorsSection, "HeadshotKillDamageColor", SettingsState.HeadshotKillDamageColor, "Headshot kill color (default: dark red)");
            root.AppendChild(colorsSection);
        }

        private static void CreateCrosshairMarkersSection(XmlDocument doc, XmlElement root)
        {
            var markersSection = doc.CreateElement("CrosshairMarkers");
            AddElement(doc, markersSection, "EnableCrosshairMarkers", SettingsState.EnableCrosshairMarkers.ToString(), "Enable/disable crosshair hit markers (default: true)");
            AddElement(doc, markersSection, "MarkerDuration", SettingsState.MarkerDuration.ToString(CultureInfo.InvariantCulture), "How long markers are visible in seconds (default: 0.35)");
            AddElement(doc, markersSection, "MarkerFontSize", SettingsState.MarkerFontSize.ToString(), "Size of crosshair marker symbols (default: 28)");
            AddElement(doc, markersSection, "NormalHitMarker", SettingsState.NormalHitMarker, "Symbol for normal hits (default: x)");
            AddElement(doc, markersSection, "KillMarker", SettingsState.KillMarker, "Symbol for kills (default: x)");
            AddElement(doc, markersSection, "HeadshotMarker", SettingsState.HeadshotMarker, "Symbol for headshots (default: x)");
            AddElement(doc, markersSection, "HeadshotKillMarker", SettingsState.HeadshotKillMarker, "Symbol for headshot kills (default: X)");
            AddColorElement(doc, markersSection, "NormalMarkerColor", SettingsState.NormalMarkerColor, "Normal hit marker color (default: white)");
            AddColorElement(doc, markersSection, "KillMarkerColor", SettingsState.KillMarkerColor, "Kill marker color (default: red)");
            AddColorElement(doc, markersSection, "HeadshotMarkerColor", SettingsState.HeadshotMarkerColor, "Headshot marker color (default: gold)");
            AddColorElement(doc, markersSection, "HeadshotKillMarkerColor", SettingsState.HeadshotKillMarkerColor, "Headshot kill marker color (default: dark red)");
            root.AppendChild(markersSection);
        }

        private static void CreateTextStylingSection(XmlDocument doc, XmlElement root)
        {
            var stylingSection = doc.CreateElement("TextStyling");
            AddElement(doc, stylingSection, "FontName", SettingsState.FontName, "Font name");
            AddElement(doc, stylingSection, "EnableOutline", SettingsState.EnableOutline.ToString(), "Enable text outline for better visibility (default: true)");
            AddColorElement(doc, stylingSection, "OutlineColor", SettingsState.OutlineColor, "Outline color (default: black)");
            AddElement(doc, stylingSection, "OutlineThickness", SettingsState.OutlineThickness.ToString(CultureInfo.InvariantCulture), "Outline thickness - higher values = thicker outline (default: 1.0)");
            root.AppendChild(stylingSection);
        }

        private static void CreateAdvancedSection(XmlDocument doc, XmlElement root)
        {
            var advancedSection = doc.CreateElement("Advanced");
            AddElement(doc, advancedSection, "PlayerDamageOnly", SettingsState.PlayerDamageOnly.ToString(), "Only show damage caused by player (default: true)");
            AddElement(doc, advancedSection, "RandomizePosition", SettingsState.RandomizePosition.ToString(), "Slightly randomize text position to prevent overlap (default: true)");
            AddElement(doc, advancedSection, "PositionRandomness", SettingsState.PositionRandomness.ToString(CultureInfo.InvariantCulture), "Amount of position randomization (default: 0.25)");
            AddElement(doc, advancedSection, "ScaleTextByDamage", SettingsState.ScaleTextByDamage.ToString(), "Scale text size based on damage amount (default: false)");
            AddElement(doc, advancedSection, "MinScale", SettingsState.MinScale.ToString(CultureInfo.InvariantCulture), "Minimum text scale multiplier when scaling by damage (default: 0.8)");
            AddElement(doc, advancedSection, "MaxScale", SettingsState.MaxScale.ToString(CultureInfo.InvariantCulture), "Maximum text scale multiplier when scaling by damage (default: 1.3)");
            AddElement(doc, advancedSection, "MaxDamageForScale", SettingsState.MaxDamageForScale.ToString(), "Damage amount that gives maximum scale (default: 100)");
            root.AppendChild(advancedSection);
        }

        private static void AddElement(XmlDocument doc, XmlElement parent, string name, string value, string comment)
        {
            if (!string.IsNullOrEmpty(comment))
            {
                var commentNode = doc.CreateComment($" {comment} ");
                parent.AppendChild(commentNode);
            }

            var element = doc.CreateElement(name);
            element.InnerText = value;
            parent.AppendChild(element);
        }

        private static void AddColorElement(XmlDocument doc, XmlElement parent, string name, Color color, string comment)
        {
            var colorValue = ColorUtils.ColorToHex(color);
            AddElement(doc, parent, name, colorValue, comment);
        }

        // Helper methods for reading XML values with dynamic root path
        private static int GetIntValue(XmlDocument doc, string rootPath, string xpath, int defaultValue)
        {
            var node = doc.SelectSingleNode($"/{rootPath}/{xpath}");
            return node != null && int.TryParse(node.InnerText, out var result) ? result : defaultValue;
        }

        private static float GetFloatValue(XmlDocument doc, string rootPath, string xpath, float defaultValue)
        {
            var node = doc.SelectSingleNode($"/{rootPath}/{xpath}");
            return node != null && float.TryParse(node.InnerText, out var result) ? result : defaultValue;
        }

        private static bool GetBoolValue(XmlDocument doc, string rootPath, string xpath, bool defaultValue)
        {
            var node = doc.SelectSingleNode($"/{rootPath}/{xpath}");
            return node != null && bool.TryParse(node.InnerText, out var result) ? result : defaultValue;
        }

        private static string GetStringValue(XmlDocument doc, string rootPath, string xpath, string defaultValue)
        {
            var node = doc.SelectSingleNode($"/{rootPath}/{xpath}");
            return node?.InnerText ?? defaultValue;
        }

        private static Vector3 GetVector3Value(XmlDocument doc, string rootPath, string xpath, Vector3 defaultValue)
        {
            var node = doc.SelectSingleNode($"/{rootPath}/{xpath}");
            if (node != null)
            {
                var parts = node.InnerText.Split(',');
                if (parts.Length == 3 &&
                    float.TryParse(parts[0], out var x) &&
                    float.TryParse(parts[1], out var y) &&
                    float.TryParse(parts[2], out var z))
                    return new Vector3(x, y, z);
            }

            return defaultValue;
        }

        private static Color GetColorValue(XmlDocument doc, string rootPath, string xpath, Color defaultValue)
        {
            var node = doc.SelectSingleNode($"/{rootPath}/{xpath}");
            if (node != null) return ColorUtils.ParseColor(node.InnerText, defaultValue);
            return defaultValue;
        }

        public static void SaveSettings()
        {
            try
            {
                AdnLogger.Debug("Saving current settings to XML");

                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath) ?? string.Empty);
                var doc = CreateConfigDocument();
                doc.Save(ConfigPath);

                AdnLogger.Debug("Settings saved to XML successfully");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Failed to save settings to XML: {ex.Message}");
            }
        }

        public static void ResetToDefaults()
        {
            AdnLogger.Debug("To reset XML config, delete AngelDamageNumbersConfig.xml and restart the game");
        }
    }
}