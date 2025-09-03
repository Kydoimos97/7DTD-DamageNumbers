using System;
using System.Collections.Generic;
using System.Linq;
using AngelDamageNumbers.Config;
using TMPro;
using UnityEngine;

namespace AngelDamageNumbers.Utilities
{
    public static class FontUtils
    {
        private static readonly string[] MarkerFallBack = { "x", "×", "●", "✓" };

        // User-facing name → Resources path (no extension)
        public static readonly Dictionary<string, string?> FontMap =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "Roboto",       "Fonts/Roboto SDF" },
                { "Open Sans",    "Fonts/OpenSans SDF" },
                { "Montserrat",   "Fonts/Montserrat SDF" },
                { "Tinos",        "Fonts/Tinos SDF" },
                { "Cousine",      "Fonts/Cousine SDF" },
                { "Ubuntu Mono",  "Fonts/UbuntuMono SDF" },
                { "Oswald",       "Fonts/Oswald SDF" },
                { "Noto Sans Symbols", "Fonts/NotoSansSymbols2 SDF" }, // available as a selectable face if you want
                { "Arial",        null } // built-in handled at runtime
            };

        private const string DefaultDisplayName = "Arial";

        // Caches
        private static readonly Dictionary<string, TMP_FontAsset> Cache =
            new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Material> OutlineMaterialCache =
            new(StringComparer.Ordinal);


        private static TMP_FontAsset? _currentFont;
        private static string? _lastDisplayName;
        private static TMP_FontAsset? _symbolsFont;  // Noto Sans Symbols 2
        private static TMP_FontAsset? _arialRuntime; // built-in Arial → TMP

        // ---------- Symbol pools ----------
        private static readonly string[] SymbolPool =
        {
            "x", "X", "×", "✖", "✕", "✗",
            "+", "✚", "✛", "✜",
            "•", "·", "●", "○", "◉", "◎", "⊙",
            "✓", "✔",
            "★", "☆", "✦", "✧", "◆", "◇", "♦",
            "→", "►", "›", "➤",
            "☠", "⚡"
        };

        // Basic markers = use selected font (if glyph exists)
        public static readonly string[] BasicMarkers = { "x", "X", "×", "+", "•", "●", "✓" };
        private static readonly HashSet<string> BasicMarkersSet = new(BasicMarkers, StringComparer.Ordinal);

        // Fancy markers = route to Noto Sans Symbols 2
        public static readonly string[] FancyMarkers =
            SymbolPool.Where(s => !BasicMarkersSet.Contains(s)).ToArray();

        public static bool IsBasicMarker(string s) => !string.IsNullOrEmpty(s) && BasicMarkersSet.Contains(s);

        // Expose what you want to show in the picker (all that are supported)
        public static string[] GetAllowedSymbolsFor(string displayName)
        {
            var selected = LoadByDisplayName(displayName);
            var symbols  = GetSymbolsFont();

            var allowed = new List<string>(SymbolPool.Length);

            // Basic: require glyphs in the selected font
            foreach (var s in BasicMarkers)
                if (selected.HasCharacters(s))
                    allowed.Add(s);

            // Fancy: require glyphs in the symbols font
            foreach (var s in FancyMarkers)
                if (symbols.HasCharacters(s))
                    allowed.Add(s);

            return allowed.Count > 0 ? allowed.ToArray() : MarkerFallBack;
        }

        public static TMP_FontAsset GetMarkerFontForSymbol(string symbol, string displayName)
        {
            if (string.IsNullOrEmpty(symbol)) return GetSelectedFont(displayName);

            if (IsBasicMarker(symbol))
            {
                var face = GetSelectedFont(displayName);
                // safety: if the selected face lacks this glyph, fall back to symbols font
                return face != null && face.HasCharacters(symbol) ? face : GetSymbolsFont();
            }

            // fancy
            return GetSymbolsFont();
        }

        // ---------- Numbers font (user selected) ----------
        public static TMP_FontAsset GetConfiguredTMPFont()
        {
            var configured = ConfigurationService.Current.FontName;
            if (_currentFont == null || !string.Equals(_lastDisplayName, configured, StringComparison.OrdinalIgnoreCase))
            {
                _lastDisplayName = configured;
                _currentFont = LoadByDisplayName(configured);
                AdnLogger.Debug($"Using TMP font: '{_currentFont.name}' (display: '{_lastDisplayName}').");
            }
            return _currentFont;
        }

        public static TMP_FontAsset GetSelectedFont(string displayName)
        {
            // Direct helper if you need a font for a specific display name (bypasses cache of current)
            return LoadByDisplayName(displayName);
        }

        // Legacy (non-TMP)
        public static Font GetConfiguredFont()
        {
            return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        public static void CleanupStatics()
        {
            _currentFont = null;
            _lastDisplayName = null;
            Cache.Clear();

            foreach (var kv in OutlineMaterialCache)
                if (kv.Value) UnityEngine.Object.Destroy(kv.Value);
            OutlineMaterialCache.Clear();

            AdnLogger.Debug("FontUtils static references cleaned up");
        }

        public static string ColorToString(Color color)
        {
            return $"({color.r:F2}, {color.g:F2}, {color.b:F2}, {color.a:F2})";
        }

        // ---------- Internals ----------
        private static TMP_FontAsset LoadByDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                displayName = DefaultDisplayName;

            if (Cache.TryGetValue(displayName, out var hit) && hit != null)
                return hit;

            if (!FontMap.TryGetValue(displayName, out var path))
            {
                AdnLogger.Warning($"Unknown font '{displayName}', falling back to '{DefaultDisplayName}'.");
                return LoadByDisplayName(DefaultDisplayName);
            }

            // Built-in Arial → TMP at runtime
            var asset = path == null ? GetArialRuntime() : Resources.Load<TMP_FontAsset>(path);

            if (asset == null)
            {
                AdnLogger.Error($"Failed to load TMP font '{displayName}', hard fallback to built-in Arial.");
                asset = GetArialRuntime();
            }

            Cache[displayName] = asset;
            return asset;
        }

        private static TMP_FontAsset GetSymbolsFont()
        {
            if (_symbolsFont != null) return _symbolsFont;

            _symbolsFont = Resources.Load<TMP_FontAsset>("Fonts/NotoSansSymbols2 SDF");
            if (_symbolsFont == null)
            {
                AdnLogger.Warning("Symbols font 'Fonts/NotoSansSymbols2 SDF' not found. Falling back to built-in Arial SDF.");
                _symbolsFont = GetArialRuntime();
            }
            return _symbolsFont;
        }

        private static TMP_FontAsset GetArialRuntime()
        {
            if (_arialRuntime != null) return _arialRuntime;

            var arial = Resources.GetBuiltinResource<Font>("Arial.ttf");
            _arialRuntime = TMP_FontAsset.CreateFontAsset(arial);
            _arialRuntime.name = "Arial SDF (Runtime)";
            return _arialRuntime;
        }

        public static Material GetOutlineMaterial(TMP_FontAsset font, Color outlineColor, float thickness, float faceDilate = 0.1f)
        {
            // base material we’ll clone (prefer the font’s material)
            var baseMat = font != null ? font.material : null;

            // Fallback if someone passes a TMP asset without a material
            if (baseMat == null)
            {
                var def = TMP_Settings.defaultFontAsset;
                if (def != null) baseMat = def.material;
            }

            // Still null? last-resort generic UI mat (keeps us from crashing)
            if (baseMat == null)
                baseMat = new Material(Shader.Find("UI/Default"));

            // Key by the **instance** of the base material + all visual params (include alpha!)
            var baseId = baseMat.GetInstanceID();
            var key = $"{baseId}|{outlineColor.r:F3},{outlineColor.g:F3},{outlineColor.b:F3},{outlineColor.a:F3}|{thickness:F3}|{faceDilate:F3}";

            if (OutlineMaterialCache.TryGetValue(key, out var cached) && cached)   // Unity null-check
                return cached;

            // Clone and configure
            var mat = new Material(baseMat) { hideFlags = HideFlags.DontSave };
            mat.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
            mat.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
            mat.SetFloat(ShaderUtilities.ID_FaceDilate, faceDilate);

            OutlineMaterialCache[key] = mat;
            AdnLogger.Debug($"[FontUtils] Cached outline mat key={key}");
            return mat;
        }

    }
}
