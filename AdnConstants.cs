namespace Utilities
{
    public static class AdnConstants
    {
        // UI Layout Constants
        public const float DefaultTextWidth = 200f;
        public const float DefaultTextHeight = 100f;
        public const int CanvasSortOrder = 1000;
        public const float CanvasPixelsPerUnit = 10f;

        // Canvas Settings
        public const float UIScaleReferenceWidth = 1920f;
        public const float UIScaleReferenceHeight = 1080f;

        // Text Display Constants
        public const float TextScaleModifier = 0.01f;
        public const float PivotCenter = 0.5f;

        // Animation Constants
        public const float FloatDirectionY = 1f;

        // Marker Constants
        public const float MarkerRectWidth = 200f;
        public const float MarkerRectHeight = 100f;

        // Default Values (fallbacks when config fails)
        public const string DefaultFontName = "Arial";
        public const int DefaultFontSize = 14;
        public const float DefaultTextLifetime = 1.5f;
        public const float DefaultFloatSpeed = 0.85f;
        public const float DefaultMarkerDuration = 0.35f;
        public const int DefaultMarkerFontSize = 28;

        // Vector component defaults
        public const float DefaultTextOffsetX = 0.0f;
        public const float DefaultTextOffsetY = 1.5f;
        public const float DefaultTextOffsetZ = 0.0f;

        // Outline defaults
        public const float DefaultOutlineThickness = 1.0f;

        // Scale defaults
        public const float DefaultMinScale = 0.5f;
        public const float DefaultMaxScale = 2.0f;
        public const int DefaultMaxDamageForScale = 100;
        public const float DefaultPositionRandomness = 0.25f;

        // Threshold defaults
        public const int DefaultMinimumDamageThreshold = 2;
        public const float DefaultDamageCooldown = 0.0f;

        // Default marker symbols
        public const string DefaultNormalMarker = "x";
        public const string DefaultKillMarker = "x";
        public const string DefaultHeadshotMarker = "x";
        public const string DefaultHeadshotKillMarker = "X";

        // Configuration version
        public const string ConfigVersion = "3.0.0";
    }
}