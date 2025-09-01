using System;
using UnityEngine;

namespace Utilities
{
    public static class ColorUtils
    {
        public static string ColorToHex(Color color)
        {
            var r = Mathf.RoundToInt(color.r * 255);
            var g = Mathf.RoundToInt(color.g * 255);
            var b = Mathf.RoundToInt(color.b * 255);
            var a = Mathf.RoundToInt(color.a * 255);

            if (a < 255)
                return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        public static Color HexToColor(string hex, Color defaultColor)
        {
            if (string.IsNullOrEmpty(hex) || !hex.StartsWith("#"))
                return defaultColor;

            hex = hex.Substring(1); // Remove #

            try
            {
                if (hex.Length == 6) // RGB format
                {
                    var r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    var g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    var b = Convert.ToInt32(hex.Substring(4, 2), 16);
                    return new Color(r / 255f, g / 255f, b / 255f, 1f);
                }

                if (hex.Length == 8) // RGBA format
                {
                    var r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    var g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    var b = Convert.ToInt32(hex.Substring(4, 2), 16);
                    var a = Convert.ToInt32(hex.Substring(6, 2), 16);
                    return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
                }
            }
            catch
            {
                // Fall back to default if parsing fails
            }

            return defaultColor;
        }


        public static Color ParseColor(string colorString, Color defaultColor)
        {
            if (string.IsNullOrEmpty(colorString))
                return defaultColor;

            // Try hex format first (#RRGGBB or #RRGGBBAA)
            if (colorString.StartsWith("#")) return HexToColor(colorString, defaultColor);

            // Fall back to R,G,B,A format for backwards compatibility
            var parts = colorString.Split(',');
            if (parts.Length >= 3 &&
                float.TryParse(parts[0], out var r) &&
                float.TryParse(parts[1], out var g) &&
                float.TryParse(parts[2], out var b))
            {
                var a = parts.Length > 3 && float.TryParse(parts[3], out var alpha) ? alpha : 1f;
                return new Color(r, g, b, a);
            }

            return defaultColor;
        }

        public static bool IsValidHexColor(string hex)
        {
            if (string.IsNullOrEmpty(hex) || !hex.StartsWith("#"))
                return false;

            var hexDigits = hex.Substring(1);

            // Must be 6 (RGB) or 8 (RGBA) characters
            if (hexDigits.Length != 6 && hexDigits.Length != 8)
                return false;

            // Must contain only hex digits
            foreach (var c in hexDigits)
                if (!((c >= '0' && c <= '9') ||
                      (c >= 'A' && c <= 'F') ||
                      (c >= 'a' && c <= 'f')))
                    return false;

            return true;
        }

        public static Color GetContrastingTextColor(Color backgroundColor)
        {
            // Calculate luminance using standard formula
            var luminance = 0.299f * backgroundColor.r + 0.587f * backgroundColor.g + 0.114f * backgroundColor.b;

            // Return black for light backgrounds, white for dark backgrounds
            return luminance > 0.5f ? Color.black : Color.white;
        }

        public static Color BlendColors(Color color1, Color color2, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            return Color.Lerp(color1, color2, ratio);
        }
    }
}