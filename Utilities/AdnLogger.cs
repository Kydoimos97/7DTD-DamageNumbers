using System.Runtime.CompilerServices;

// for MethodImpl

// only if you still want Debug.Debug fallback

namespace Utilities
{
    public static class AdnLogger
    {
        // default off; set once at startup from your config, or toggle via console command
        private static volatile bool _enabled;

        // Fast-path check; avoids string formatting when disabled
        private static bool On
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _enabled;
        }

        // Call this once (e.g., after you load your XML config or on GameStartDone)
        public static void SetEnabled(bool enabled)
        {
            _enabled = enabled;
        }

        // Convenience toggles
        public static void Enable()
        {
            _enabled = true;
        }

        public static void Disable()
        {
            _enabled = false;
        }

        public static void Error(string message)
        {
            global::Log.Error($"[Angel-DamageNumbers] ERROR: {message}");
        }

        public static void Warning(string message)
        {
            global::Log.Warning($"[Angel-DamageNumbers] WARNING: {message}");
        }

        public static void Debug(string message)
        {
            if (!On) return;
            global::Log.Out($"[Angel-DamageNumbers] DEBUG: {message}");
            // UnityEngine.Debug.Debug($"[Angel-DamageNumbers] {message}"); <-- Expensive
        }

        public static void Log(string message)
        {
            global::Log.Out($"[Angel-DamageNumbers] INFO: {message}");
            // UnityEngine.Debug.Debug($"[Angel-DamageNumbers] {message}"); <-- Expensive
        }
    }
}