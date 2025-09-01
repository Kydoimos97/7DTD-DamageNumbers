using UnityEngine;
using Utilities;

namespace Config
{
    public class SettingsState
    {
        // === DEBUG SETTINGS ===
        public static bool EnableDebugLogging = true;

        // === DAMAGE NUMBER SETTINGS ===
        public static int MinimumDamageThreshold = 2;
        public static float DamageNumberCooldown = 0.0f;
        public static int FontSize = 14;
        public static float TextLifetime = 1.5f;
        public static float FloatSpeed = 0.85f;
        public static Vector3 TextOffset = new Vector3(0.0f, 1.5f, 0.0f);

        // === COLOR SETTINGS ===
        public static Color NormalDamageColor = new Color(0.580f, 0.580f, 0.580f); // #949494
        public static Color HeadshotDamageColor = new Color(0.722f, 0.522f, 0.067f); // #B88511
        public static Color KillDamageColor = new Color(0.459f, 0.024f, 0.024f); // #750606
        public static Color HeadshotKillDamageColor = new Color(0.278f, 0.008f, 0.008f); // #470202

        // === CROSSHAIR MARKER SETTINGS ===
        public static bool EnableCrosshairMarkers = true;
        public static float MarkerDuration = 0.35f;
        public static int MarkerFontSize = 28;
        public static string NormalHitMarker = "x";
        public static string KillMarker = "x";
        public static string HeadshotMarker = "x";
        public static string HeadshotKillMarker = "X";
        public static Color NormalMarkerColor = new Color(0.580f, 0.580f, 0.580f); // #949494
        public static Color KillMarkerColor = new Color(0.459f, 0.024f, 0.024f); // #750606
        public static Color HeadshotMarkerColor = new Color(0.722f, 0.522f, 0.067f); // #B88511
        public static Color HeadshotKillMarkerColor = new Color(0.278f, 0.008f, 0.008f); // #470202

        // === ADVANCED SETTINGS ===
        public static bool PlayerDamageOnly = true;
        public static bool RandomizePosition = true;
        public static float PositionRandomness = 0.25f;
        public static bool ScaleTextByDamage = true;
        public static float MinScale = 0.5f;
        public static float MaxScale = 2.0f;
        public static int MaxDamageForScale = 100;

        // === TEXT STYLING SETTINGS ===
        public static bool EnableOutline = true;
        public static Color OutlineColor = Color.black;
        public static float OutlineThickness = 1.0f;
        public static string FontName = "Arial";

        // Debug logging helper - keeping this here since it directly uses the setting
        public static void DebugLog(string message)
        {
            if (EnableDebugLogging) AdnLogger.Debug($"{message}");
        }
    }
}