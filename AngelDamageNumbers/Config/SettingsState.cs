using System;
using System.Reflection;
using AngelDamageNumbers.Utilities;
using UnityEngine;

namespace AngelDamageNumbers.Config
{
    public class SettingsState
    {
        // === DEBUG SETTINGS ===
        public static bool EnableDebugLogging = false;

        // === DAMAGE NUMBER SETTINGS ===
        public static int MinimumDamageThreshold = 2;
        public static float DamageNumberCooldown = 0.0f;
        public static int FontSize = 20;
        public static float TextLifetime = 1.5f;
        public static float FloatSpeed = 0.85f;
        public static Vector3 TextOffset = new Vector3(0.0f, 1.5f, 0.0f);

        // === COLOR SETTINGS ===
        public static Color NormalDamageColor = new Color(0.75f, 0.75f, 0.75f); // #949494
        public static Color HeadshotDamageColor = new Color(0.7f, 0.5f, 0.0f); // #B88511
        public static Color KillDamageColor = new Color(0.5f, 0.0f, 0.0f); // #750606
        public static Color HeadshotKillDamageColor = new Color(0.4f, 0.0f, 0.0f); // #470202

        // === CROSSHAIR MARKER SETTINGS ===
        public static bool EnableCrosshairMarkers = true;
        public static float MarkerDuration = 0.35f;
        public static int MarkerFontSize = 30;
        public static string NormalHitMarker = "×";
        public static string KillMarker = "×";
        public static string HeadshotMarker = "×";
        public static string HeadshotKillMarker = "X";
        public static Color NormalMarkerColor = new Color(0.75f, 0.75f, 0.75f); // #949494
        public static Color KillMarkerColor = new Color(0.7f, 0.5f, 0.0f); // #750606
        public static Color HeadshotMarkerColor = new Color(0.5f, 0.0f, 0.0f); // #B88511
        public static Color HeadshotKillMarkerColor = new Color(0.4f, 0.0f, 0.0f); // #470202

        // === ADVANCED SETTINGS ===
        public static bool PlayerDamageOnly = true;
        public static bool RandomizePosition = true;
        public static float PositionRandomness = 0.25f;
        public static bool ScaleTextByDamage = true;
        public static float MinScale = 1.0f;
        public static float MaxScale = 2.0f;
        public static int MaxDamageForScale = 100;

        // === TEXT STYLING SETTINGS ===
        public static bool EnableOutline = true;
        public static Color OutlineColor = Color.black;
        public static float OutlineThickness = 0.2f;
        public static string FontName = "LiberationSans";

        // Debug logging helper - keeping this here since it directly uses the setting
        public static void DebugLog(string message)
        {
            if (EnableDebugLogging) AdnLogger.Debug($"{message}");
        }

        public static void ResetToDefaults()
        {
            // Use reflection or manual copying from a clean instance
            var defaultType = typeof(SettingsState);
            var fields = defaultType.GetFields(BindingFlags.Public | BindingFlags.Static);

            // Reset each field to its compiled default
            foreach (var field in fields)
            {
                if (field.FieldType.IsValueType)
                    field.SetValue(null, Activator.CreateInstance(field.FieldType));
                else if (field.FieldType == typeof(string))
                    field.SetValue(null, "x");
            }
        }

        public static void Initialize()
        {
            ResetToDefaults();
        }
    }
}