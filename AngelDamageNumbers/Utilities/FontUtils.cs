using System.Collections.Generic;
using Config;
using UnityEngine;

namespace AngelDamageNumbers.Utilities
{
    public static class FontUtils
    {
        private static readonly Font DefaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        private static Font _currentFont;
        private static string _lastFontName;

        private static readonly Dictionary<string, Font> FontCache = new Dictionary<string, Font>();
        private static bool _fontsLoaded;

        private static void LoadFonts()
        {
            if (_fontsLoaded) return;

            var fonts = Resources.FindObjectsOfTypeAll<Font>();
            foreach (var font in fonts)
                if (font != null && !string.IsNullOrEmpty(font.name))
                    FontCache[font.name.ToLowerInvariant()] = font;

            _fontsLoaded = true;
            AdnLogger.Debug($"Loaded {FontCache.Count} fonts into cache");
        }

        public static Font GetConfiguredFont()
        {
            LoadFonts();

            // Check if we need to reload the font
            if (_currentFont == null || _lastFontName != ConfigurationService.Current.FontName)
            {
                _lastFontName = ConfigurationService.Current.FontName;

                if (!string.IsNullOrEmpty(_lastFontName) &&
                    FontCache.TryGetValue(_lastFontName.ToLowerInvariant(), out var cachedFont))
                {
                    _currentFont = cachedFont;
                    AdnLogger.Debug($"Loaded configured font: {_currentFont.name}");
                }
                else
                {
                    _currentFont = DefaultFont;
                    AdnLogger.Debug($"Configured font '{ConfigurationService.Current.FontName}' not found, using Arial fallback");
                }
            }

            return _currentFont;
        }

        public static void CleanupStatics()
        {
            _currentFont = null;
            _lastFontName = null;
            FontCache.Clear();
            _fontsLoaded = false;
            AdnLogger.Debug("FontUtils static references cleaned up");
        }

        public static string ColorToString(Color color)
        {
            return $"({color.r:F2}, {color.g:F2}, {color.b:F2}, {color.a:F2})";
        }
    }
}