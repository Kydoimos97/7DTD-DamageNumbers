using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AngelDamageNumbers.Utilities
{
    public static class FontUtils
    {
        private static TMP_FontAsset? _defaultFont;

        public static string FallbackMarker = "x";

        // Cache for glyph support checks
        private static readonly Dictionary<string, bool> GlyphSupportCache = new();

        // Cache for outline materials
        private static readonly Dictionary<string, Material> OutlineMaterialCache =
            new(StringComparer.Ordinal);

        // ---------- Public API ----------
        public static string GetSafeChar(string text, TMP_FontAsset fontAsset)
        {
            var key = $"{fontAsset.GetInstanceID()}|{text}";
            if (!GlyphSupportCache.TryGetValue(key, out var supported))
            {
                supported = fontAsset.HasCharacters(text);
                GlyphSupportCache[key] = supported;
            }

            if (!supported)
            {
                AdnLogger.Warning($"[FontUtils] Glyph '{text}' missing in font '{fontAsset.name}', substituting '{FallbackMarker}'");
                return FallbackMarker;
            }

            return text;
        }

        public static TMP_FontAsset GetConfiguredTMPFont()
        {
            return GetDefaultFont();
        }

        public static TMP_FontAsset GetSelectedFont(string displayName = null)
        {
            return GetDefaultFont();
        }

        public static TMP_FontAsset GetMarkerFontForSymbol(string symbol, string displayName = null)
        {
            return GetDefaultFont();
        }

        public static readonly string[] SymbolPool =
        {
            "x", "X", "×",
            // "+", "✚", "✛", "✜",
            // "•", "·", "●", "○", "◉", "◎", "⊙",
            // "★", "☆", "✦", "✧", "◆", "◇", "♦",
            // "☠", "⚡"
        };

        public static bool IsBasicMarker(string s) =>
            !string.IsNullOrEmpty(s) && BasicMarkersSet.Contains(s);

        public static readonly string[] BasicMarkers = { "x", "X", "×", "+", "•", "●" };
        private static readonly HashSet<string> BasicMarkersSet = new(BasicMarkers, StringComparer.Ordinal);

        // ---------- Internals ----------
        private static TMP_FontAsset GetDefaultFont()
        {
            if (_defaultFont != null) return _defaultFont;

            _defaultFont = TMP_Settings.defaultFontAsset;
            if (_defaultFont == null)
            {
                AdnLogger.Error("[FontUtils] TMP default font asset is null! Text may not render.");
                throw new InvalidOperationException("No valid default font available");
            }

            AdnLogger.Log($"[FontUtils] Using default TMP font: {_defaultFont.name}");
            return _defaultFont;
        }

        // ---------- Utility ----------
        public static string ColorToString(Color color) =>
            $"({color.r:F2}, {color.g:F2}, {color.b:F2}, {color.a:F2})";

        public static void CleanupStatics()
        {
            _defaultFont = null;
            GlyphSupportCache.Clear();

            foreach (var kv in OutlineMaterialCache)
                if (kv.Value) UnityEngine.Object.Destroy(kv.Value);

            OutlineMaterialCache.Clear();
            AdnLogger.Debug("[FontUtils] Static references cleaned up");
        }

        public static Material GetOutlineMaterial(TMP_FontAsset font, Color outlineColor, float thickness, float faceDilate = 0.1f)
        {
            var baseMat = font.material ?? TMP_Settings.defaultFontAsset?.material
                          ?? new Material(Shader.Find("UI/Default"));

            var baseId = baseMat.GetInstanceID();
            var key = $"{baseId}|{outlineColor.r:F3},{outlineColor.g:F3},{outlineColor.b:F3},{outlineColor.a:F3}|{thickness:F3}|{faceDilate:F3}";

            if (OutlineMaterialCache.TryGetValue(key, out var cached) && cached)
                return cached;

            var mat = new Material(baseMat) { hideFlags = HideFlags.DontSave };
            mat.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
            mat.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
            mat.SetFloat(ShaderUtilities.ID_FaceDilate, faceDilate);

            OutlineMaterialCache[key] = mat;
            AdnLogger.Debug($"[FontUtils] Cached outline material with key={key}");
            return mat;
        }
    }
}