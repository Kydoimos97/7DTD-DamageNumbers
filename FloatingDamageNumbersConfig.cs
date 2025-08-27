// FloatingDamageNumbersConfig.cs
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Collections;

public static class FloatingDamageNumbersConfig
{
    // Config file path
    private static readonly string ConfigPath = Path.Combine(
        Application.dataPath, "..", "Mods", "Angel_DamageNumbers", "FloatingDamageNumbersConfig.xml");

    // === DEBUG SETTINGS ===
    public static bool EnableDebugLogging = false;

    // === DAMAGE NUMBER SETTINGS ===
    public static int MinimumDamageThreshold = 1;
    public static float DamageNumberCooldown = 0.1f;
    public static int FontSize = 20;
    public static float TextLifetime = 1.2f;
    public static float FloatSpeed = 0.85f;
    public static Vector3 TextOffset = new Vector3(0.0f, 1.5f, 0.0f);

    // === COLOR SETTINGS ===
    public static Color NormalDamageColor = Color.white;
    public static Color HeadshotDamageColor = new Color(0.9f, 0.706f, 0f); // #e6b400 gold
    public static Color KillDamageColor = new Color(1f, 0.267f, 0.267f); // #FF4444 red
    public static Color HeadshotKillDamageColor = new Color(0.545f, 0f, 0f); // #8B0000 dark red

    // === CROSSHAIR MARKER SETTINGS ===
    public static bool EnableCrosshairMarkers = true;
    public static float MarkerDuration = 0.35f;
    public static int MarkerFontSize = 28;
    public static string NormalHitMarker = "x";
    public static string KillMarker = "x";
    public static string HeadshotMarker = "x";
    public static string HeadshotKillMarker = "X";
    public static Color NormalMarkerColor = Color.white;
    public static Color KillMarkerColor = new Color(1f, 0.267f, 0.267f); // #FF4444 red
    public static Color HeadshotMarkerColor = new Color(0.9f, 0.706f, 0f); // #e6b400 gold
    public static Color HeadshotKillMarkerColor = new Color(0.545f, 0f, 0f); // #8B0000 dark red

    // === ADVANCED SETTINGS ===
    public static bool PlayerDamageOnly = true;
    public static bool RandomizePosition = true;
    public static float PositionRandomness = 0.25f;
    public static bool ScaleTextByDamage = false;
    public static float MinScale = 0.8f;
    public static float MaxScale = 1.3f;
    public static int MaxDamageForScale = 100;

    // Debug logging helper
    public static void DebugLog(string message)
    {
        if (EnableDebugLogging)
        {
            Debug.Log($"[Angel-DamageNumbers] {message}");
        }
    }

    // Load configuration asynchronously to avoid blocking game startup
    public static void LoadConfigAsync()
    {
        CoroutineRunner.Instance.StartCoroutine(LoadConfigCoroutine());
    }

    private static IEnumerator LoadConfigCoroutine()
    {
        yield return null;

        if (!File.Exists(ConfigPath))
        {
            Debug.Log("[Angel-DamageNumbers] Config file not found, creating default config...");
            yield return CreateAndLogConfig(); // nested coroutine
            yield break;
        }

        try
        {
            Debug.Log("[Angel-DamageNumbers] Loading existing config...");
            LoadExistingConfig();
            Debug.Log($"[Angel-DamageNumbers] Config loaded! Debug logging: {(EnableDebugLogging ? "ENABLED" : "disabled")}");
            DebugLog("Debug logging is now active!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Angel-DamageNumbers] Config load failed: {ex.Message}");
        }
    }

    private static IEnumerator CreateAndLogConfig()
    {
        yield return CreateDefaultConfigAsync();
        Debug.Log("[Angel-DamageNumbers] Default config created successfully!");
    }

    // Load configuration immediately (for backwards compatibility)
    public static void LoadConfig()
    {
        try
        {
            if (!File.Exists(ConfigPath))
            {
                Debug.Log("[Angel-DamageNumbers] Config file not found, creating default config...");
                CreateDefaultConfig();
                Debug.Log("[Angel-DamageNumbers] Default config created successfully!");
                return;
            }
            Debug.Log("[Angel-DamageNumbers] Loading existing config...");
            LoadExistingConfig();
            Debug.Log($"[Angel-DamageNumbers] Config loaded! Debug logging: {(EnableDebugLogging ? "ENABLED" : "disabled")}");
            DebugLog("Debug logging is now active!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Angel-DamageNumbers] Config load failed: {ex.Message}");
        }
    }

    private static void LoadExistingConfig()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(ConfigPath);

        // Load debug settings first
        EnableDebugLogging = GetBoolValue(doc, "Debug/EnableDebugLogging", EnableDebugLogging);

        // Load damage number settings
        MinimumDamageThreshold = GetIntValue(doc, "DamageNumbers/MinimumDamageThreshold", MinimumDamageThreshold);
        DamageNumberCooldown = GetFloatValue(doc, "DamageNumbers/DamageNumberCooldown", DamageNumberCooldown);
        FontSize = GetIntValue(doc, "DamageNumbers/FontSize", FontSize);
        TextLifetime = GetFloatValue(doc, "DamageNumbers/TextLifetime", TextLifetime);
        FloatSpeed = GetFloatValue(doc, "DamageNumbers/FloatSpeed", FloatSpeed);
        TextOffset = GetVector3Value(doc, "DamageNumbers/TextOffset", TextOffset);

        // Load color settings
        NormalDamageColor = GetColorValue(doc, "Colors/NormalDamageColor", NormalDamageColor);
        HeadshotDamageColor = GetColorValue(doc, "Colors/HeadshotDamageColor", HeadshotDamageColor);
        KillDamageColor = GetColorValue(doc, "Colors/KillDamageColor", KillDamageColor);
        HeadshotKillDamageColor = GetColorValue(doc, "Colors/HeadshotKillDamageColor", HeadshotKillDamageColor);

        // Load crosshair settings
        EnableCrosshairMarkers = GetBoolValue(doc, "CrosshairMarkers/EnableCrosshairMarkers", EnableCrosshairMarkers);
        MarkerDuration = GetFloatValue(doc, "CrosshairMarkers/MarkerDuration", MarkerDuration);
        MarkerFontSize = GetIntValue(doc, "CrosshairMarkers/MarkerFontSize", MarkerFontSize);
        NormalHitMarker = GetStringValue(doc, "CrosshairMarkers/NormalHitMarker", NormalHitMarker);
        KillMarker = GetStringValue(doc, "CrosshairMarkers/KillMarker", KillMarker);
        HeadshotMarker = GetStringValue(doc, "CrosshairMarkers/HeadshotMarker", HeadshotMarker);
        HeadshotKillMarker = GetStringValue(doc, "CrosshairMarkers/HeadshotKillMarker", HeadshotKillMarker);
        NormalMarkerColor = GetColorValue(doc, "CrosshairMarkers/NormalMarkerColor", NormalMarkerColor);
        KillMarkerColor = GetColorValue(doc, "CrosshairMarkers/KillMarkerColor", KillMarkerColor);
        HeadshotMarkerColor = GetColorValue(doc, "CrosshairMarkers/HeadshotMarkerColor", HeadshotMarkerColor);
        HeadshotKillMarkerColor = GetColorValue(doc, "CrosshairMarkers/HeadshotKillMarkerColor", HeadshotKillMarkerColor);

        // Load advanced settings
        PlayerDamageOnly = GetBoolValue(doc, "Advanced/PlayerDamageOnly", PlayerDamageOnly);
        RandomizePosition = GetBoolValue(doc, "Advanced/RandomizePosition", RandomizePosition);
        PositionRandomness = GetFloatValue(doc, "Advanced/PositionRandomness", PositionRandomness);
        ScaleTextByDamage = GetBoolValue(doc, "Advanced/ScaleTextByDamage", ScaleTextByDamage);
        MinScale = GetFloatValue(doc, "Advanced/MinScale", MinScale);
        MaxScale = GetFloatValue(doc, "Advanced/MaxScale", MaxScale);
        MaxDamageForScale = GetIntValue(doc, "Advanced/MaxDamageForScale", MaxDamageForScale);
    }

    private static IEnumerator CreateDefaultConfigAsync()
    {
        yield return null;

        bool configCreated = false;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
            yield return null;

            XmlDocument doc = new XmlDocument();
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declaration);

            XmlElement root = doc.CreateElement("FloatingDamageNumbersConfig");
            doc.AppendChild(root);

            // Add main comment
            XmlComment comment = doc.CreateComment(@"
        Angel's Enhanced Damage Numbers Mod Configuration
        
        Colors: Use hex format like #FFFFFF (white), #FF0000 (red), #e6b400 (gold)
                Can also use 8-digit format #RRGGBBAA for transparency: #FF000080 (semi-transparent red)
        
        Symbols: You can use Unicode symbols like × ☠ ✖ ⚡ ★ ♦ ● ▲ ✓ ➤
        
        Most IDEs will show color previews for hex values and provide color pickers!
        ");
            root.AppendChild(comment);
            yield return null;

            // Debug section
            XmlElement debugSection = doc.CreateElement("Debug");
            AddElement(doc, debugSection, "EnableDebugLogging", EnableDebugLogging.ToString(), "Enable debug messages in Unity console - set to true to troubleshoot issues (default: false)");
            root.AppendChild(debugSection);
            yield return null;

            // Damage Numbers section
            XmlElement damageSection = doc.CreateElement("DamageNumbers");
            AddElement(doc, damageSection, "MinimumDamageThreshold", MinimumDamageThreshold.ToString(), "Minimum damage to show numbers (0 = show all damage)");
            AddElement(doc, damageSection, "DamageNumberCooldown", DamageNumberCooldown.ToString(), "Minimum time between damage numbers in seconds (prevents spam)");
            AddElement(doc, damageSection, "FontSize", FontSize.ToString(), "Size of damage text (default: 20)");
            AddElement(doc, damageSection, "TextLifetime", TextLifetime.ToString(), "How long text is visible in seconds (default: 1.2)");
            AddElement(doc, damageSection, "FloatSpeed", FloatSpeed.ToString(), "Speed text floats upward (default: 0.85)");
            AddElement(doc, damageSection, "TextOffset", $"{TextOffset.x},{TextOffset.y},{TextOffset.z}", "Offset from entity position in X,Y,Z format (default: 0,1.5,0)");
            root.AppendChild(damageSection);
            yield return null;

            // Colors section
            XmlElement colorsSection = doc.CreateElement("Colors");
            AddColorElement(doc, colorsSection, "NormalDamageColor", NormalDamageColor, "Normal damage color (default: white)");
            AddColorElement(doc, colorsSection, "HeadshotDamageColor", HeadshotDamageColor, "Headshot damage color (default: gold)");
            AddColorElement(doc, colorsSection, "KillDamageColor", KillDamageColor, "Killing blow color (default: red)");
            AddColorElement(doc, colorsSection, "HeadshotKillDamageColor", HeadshotKillDamageColor, "Headshot kill color (default: dark red)");
            root.AppendChild(colorsSection);
            yield return null;

            // Crosshair Markers section
            XmlElement markersSection = doc.CreateElement("CrosshairMarkers");
            AddElement(doc, markersSection, "EnableCrosshairMarkers", EnableCrosshairMarkers.ToString(), "Enable/disable crosshair hit markers (default: true)");
            AddElement(doc, markersSection, "MarkerDuration", MarkerDuration.ToString(), "How long markers are visible in seconds (default: 0.35)");
            AddElement(doc, markersSection, "MarkerFontSize", MarkerFontSize.ToString(), "Size of crosshair marker symbols (default: 28)");
            AddElement(doc, markersSection, "NormalHitMarker", NormalHitMarker, "Symbol for normal hits (default: x)");
            AddElement(doc, markersSection, "KillMarker", KillMarker, "Symbol for kills (default: x)");
            AddElement(doc, markersSection, "HeadshotMarker", HeadshotMarker, "Symbol for headshots (default: x)");
            AddElement(doc, markersSection, "HeadshotKillMarker", HeadshotKillMarker, "Symbol for headshot kills (default: X)");
            AddColorElement(doc, markersSection, "NormalMarkerColor", NormalMarkerColor, "Normal hit marker color (default: white)");
            AddColorElement(doc, markersSection, "KillMarkerColor", KillMarkerColor, "Kill marker color (default: red)");
            AddColorElement(doc, markersSection, "HeadshotMarkerColor", HeadshotMarkerColor, "Headshot marker color (default: gold)");
            AddColorElement(doc, markersSection, "HeadshotKillMarkerColor", HeadshotKillMarkerColor, "Headshot kill marker color (default: dark red)");
            root.AppendChild(markersSection);
            yield return null;

            // Advanced section
            XmlElement advancedSection = doc.CreateElement("Advanced");
            AddElement(doc, advancedSection, "PlayerDamageOnly", PlayerDamageOnly.ToString(), "Only show damage caused by player (default: true)");
            AddElement(doc, advancedSection, "RandomizePosition", RandomizePosition.ToString(), "Slightly randomize text position to prevent overlap (default: true)");
            AddElement(doc, advancedSection, "PositionRandomness", PositionRandomness.ToString(), "Amount of position randomization (default: 0.25)");
            AddElement(doc, advancedSection, "ScaleTextByDamage", ScaleTextByDamage.ToString(), "Scale text size based on damage amount (default: false)");
            AddElement(doc, advancedSection, "MinScale", MinScale.ToString(), "Minimum text scale multiplier when scaling by damage (default: 0.8)");
            AddElement(doc, advancedSection, "MaxScale", MaxScale.ToString(), "Maximum text scale multiplier when scaling by damage (default: 1.3)");
            AddElement(doc, advancedSection, "MaxDamageForScale", MaxDamageForScale.ToString(), "Damage amount that gives maximum scale (default: 100)");
            root.AppendChild(advancedSection);
            yield return null;

            doc.Save(ConfigPath);
            configCreated = true;
        }
        finally
        {
            // Check if config was actually created successfully
            if (!configCreated || !File.Exists(ConfigPath))
            {
                Debug.LogError("[Angel-DamageNumbers] Failed to create default config file. Using built-in defaults.");
            }
        }
    }

    private static void CreateDefaultConfig()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));

            XmlDocument doc = new XmlDocument();
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declaration);

            XmlElement root = doc.CreateElement("FloatingDamageNumbersConfig");
            doc.AppendChild(root);

            // Add main comment
            XmlComment comment = doc.CreateComment(@"
    Angel's Enhanced Damage Numbers Mod Configuration
    
    Colors: Use hex format like #FFFFFF (white), #FF0000 (red), #e6b400 (gold)
            Can also use 8-digit format #RRGGBBAA for transparency: #FF000080 (semi-transparent red)
    
    Symbols: You can use Unicode symbols like × ☠ ✖ ⚡ ★ ♦ ● ▲ ✓ ➤
    
    Most IDEs will show color previews for hex values and provide color pickers!
    ");
            root.AppendChild(comment);

            // Debug section
            XmlElement debugSection = doc.CreateElement("Debug");
            AddElement(doc, debugSection, "EnableDebugLogging", EnableDebugLogging.ToString(), "Enable debug messages in Unity console - set to true to troubleshoot issues (default: false)");
            root.AppendChild(debugSection);

            // Damage Numbers section
            XmlElement damageSection = doc.CreateElement("DamageNumbers");
            AddElement(doc, damageSection, "MinimumDamageThreshold", MinimumDamageThreshold.ToString(), "Minimum damage to show numbers (0 = show all damage)");
            AddElement(doc, damageSection, "DamageNumberCooldown", DamageNumberCooldown.ToString(), "Minimum time between damage numbers in seconds (prevents spam)");
            AddElement(doc, damageSection, "FontSize", FontSize.ToString(), "Size of damage text (default: 20)");
            AddElement(doc, damageSection, "TextLifetime", TextLifetime.ToString(), "How long text is visible in seconds (default: 1.2)");
            AddElement(doc, damageSection, "FloatSpeed", FloatSpeed.ToString(), "Speed text floats upward (default: 0.85)");
            AddElement(doc, damageSection, "TextOffset", $"{TextOffset.x},{TextOffset.y},{TextOffset.z}", "Offset from entity position in X,Y,Z format (default: 0,1.5,0)");
            root.AppendChild(damageSection);

            // Colors section
            XmlElement colorsSection = doc.CreateElement("Colors");
            AddColorElement(doc, colorsSection, "NormalDamageColor", NormalDamageColor, "Normal damage color (default: white)");
            AddColorElement(doc, colorsSection, "HeadshotDamageColor", HeadshotDamageColor, "Headshot damage color (default: gold)");
            AddColorElement(doc, colorsSection, "KillDamageColor", KillDamageColor, "Killing blow color (default: red)");
            AddColorElement(doc, colorsSection, "HeadshotKillDamageColor", HeadshotKillDamageColor, "Headshot kill color (default: dark red)");
            root.AppendChild(colorsSection);

            // Crosshair Markers section
            XmlElement markersSection = doc.CreateElement("CrosshairMarkers");
            AddElement(doc, markersSection, "EnableCrosshairMarkers", EnableCrosshairMarkers.ToString(), "Enable/disable crosshair hit markers (default: true)");
            AddElement(doc, markersSection, "MarkerDuration", MarkerDuration.ToString(), "How long markers are visible in seconds (default: 0.35)");
            AddElement(doc, markersSection, "MarkerFontSize", MarkerFontSize.ToString(), "Size of crosshair marker symbols (default: 28)");
            AddElement(doc, markersSection, "NormalHitMarker", NormalHitMarker, "Symbol for normal hits (default: x)");
            AddElement(doc, markersSection, "KillMarker", KillMarker, "Symbol for kills (default: x)");
            AddElement(doc, markersSection, "HeadshotMarker", HeadshotMarker, "Symbol for headshots (default: x)");
            AddElement(doc, markersSection, "HeadshotKillMarker", HeadshotKillMarker, "Symbol for headshot kills (default: X)");
            AddColorElement(doc, markersSection, "NormalMarkerColor", NormalMarkerColor, "Normal hit marker color (default: white)");
            AddColorElement(doc, markersSection, "KillMarkerColor", KillMarkerColor, "Kill marker color (default: red)");
            AddColorElement(doc, markersSection, "HeadshotMarkerColor", HeadshotMarkerColor, "Headshot marker color (default: gold)");
            AddColorElement(doc, markersSection, "HeadshotKillMarkerColor", HeadshotKillMarkerColor, "Headshot kill marker color (default: dark red)");
            root.AppendChild(markersSection);

            // Advanced section
            XmlElement advancedSection = doc.CreateElement("Advanced");
            AddElement(doc, advancedSection, "PlayerDamageOnly", PlayerDamageOnly.ToString(), "Only show damage caused by player (default: true)");
            AddElement(doc, advancedSection, "RandomizePosition", RandomizePosition.ToString(), "Slightly randomize text position to prevent overlap (default: true)");
            AddElement(doc, advancedSection, "PositionRandomness", PositionRandomness.ToString(), "Amount of position randomization (default: 0.25)");
            AddElement(doc, advancedSection, "ScaleTextByDamage", ScaleTextByDamage.ToString(), "Scale text size based on damage amount (default: false)");
            AddElement(doc, advancedSection, "MinScale", MinScale.ToString(), "Minimum text scale multiplier when scaling by damage (default: 0.8)");
            AddElement(doc, advancedSection, "MaxScale", MaxScale.ToString(), "Maximum text scale multiplier when scaling by damage (default: 1.3)");
            AddElement(doc, advancedSection, "MaxDamageForScale", MaxDamageForScale.ToString(), "Damage amount that gives maximum scale (default: 100)");
            root.AppendChild(advancedSection);

            doc.Save(ConfigPath);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Angel-DamageNumbers] Failed to create default config: {ex.Message}");
        }
    }

    private static void AddElement(XmlDocument doc, XmlElement parent, string name, string value, string comment)
    {
        if (!string.IsNullOrEmpty(comment))
        {
            XmlComment commentNode = doc.CreateComment($" {comment} ");
            parent.AppendChild(commentNode);
        }
        XmlElement element = doc.CreateElement(name);
        element.InnerText = value;
        parent.AppendChild(element);
    }

    private static void AddColorElement(XmlDocument doc, XmlElement parent, string name, Color color, string comment)
    {
        string colorValue = ColorToHex(color);
        AddElement(doc, parent, name, colorValue, comment);
    }

    // Convert Unity Color to hex format (#RRGGBB or #RRGGBBAA)
    private static string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255);
        int g = Mathf.RoundToInt(color.g * 255);
        int b = Mathf.RoundToInt(color.b * 255);
        int a = Mathf.RoundToInt(color.a * 255);

        if (a < 255)
            return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
        else
            return $"#{r:X2}{g:X2}{b:X2}";
    }

    // Convert hex format to Unity Color
    private static Color HexToColor(string hex, Color defaultColor)
    {
        if (string.IsNullOrEmpty(hex) || !hex.StartsWith("#"))
            return defaultColor;

        hex = hex.Substring(1); // Remove #

        try
        {
            if (hex.Length == 6) // RGB format
            {
                int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                return new Color(r / 255f, g / 255f, b / 255f, 1f);
            }
            else if (hex.Length == 8) // RGBA format
            {
                int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                int a = Convert.ToInt32(hex.Substring(6, 2), 16);
                return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
            }
        }
        catch
        {
            // Fall back to default if parsing fails
        }

        return defaultColor;
    }

    // Helper methods for reading XML values
    private static int GetIntValue(XmlDocument doc, string xpath, int defaultValue)
    {
        XmlNode node = doc.SelectSingleNode($"/FloatingDamageNumbersConfig/{xpath}");
        return node != null && int.TryParse(node.InnerText, out int result) ? result : defaultValue;
    }

    private static float GetFloatValue(XmlDocument doc, string xpath, float defaultValue)
    {
        XmlNode node = doc.SelectSingleNode($"/FloatingDamageNumbersConfig/{xpath}");
        return node != null && float.TryParse(node.InnerText, out float result) ? result : defaultValue;
    }

    private static bool GetBoolValue(XmlDocument doc, string xpath, bool defaultValue)
    {
        XmlNode node = doc.SelectSingleNode($"/FloatingDamageNumbersConfig/{xpath}");
        return node != null && bool.TryParse(node.InnerText, out bool result) ? result : defaultValue;
    }

    private static string GetStringValue(XmlDocument doc, string xpath, string defaultValue)
    {
        XmlNode node = doc.SelectSingleNode($"/FloatingDamageNumbersConfig/{xpath}");
        return node?.InnerText ?? defaultValue;
    }

    private static Vector3 GetVector3Value(XmlDocument doc, string xpath, Vector3 defaultValue)
    {
        XmlNode node = doc.SelectSingleNode($"/FloatingDamageNumbersConfig/{xpath}");
        if (node != null)
        {
            string[] parts = node.InnerText.Split(',');
            if (parts.Length == 3 &&
                float.TryParse(parts[0], out float x) &&
                float.TryParse(parts[1], out float y) &&
                float.TryParse(parts[2], out float z))
            {
                return new Vector3(x, y, z);
            }
        }
        return defaultValue;
    }

    private static Color GetColorValue(XmlDocument doc, string xpath, Color defaultValue)
    {
        XmlNode node = doc.SelectSingleNode($"/FloatingDamageNumbersConfig/{xpath}");
        if (node != null)
        {
            // Try hex format first (#RRGGBB or #RRGGBBAA)
            if (node.InnerText.StartsWith("#"))
            {
                return HexToColor(node.InnerText, defaultValue);
            }

            // Fall back to R,G,B,A format for backwards compatibility
            string[] parts = node.InnerText.Split(',');
            if (parts.Length >= 3 &&
                float.TryParse(parts[0], out float r) &&
                float.TryParse(parts[1], out float g) &&
                float.TryParse(parts[2], out float b))
            {
                float a = parts.Length > 3 && float.TryParse(parts[3], out float alpha) ? alpha : 1f;
                return new Color(r, g, b, a);
            }
        }
        return defaultValue;
    }
}