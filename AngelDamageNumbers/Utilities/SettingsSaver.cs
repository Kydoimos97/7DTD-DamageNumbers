using System.Collections;
using AngelDamageNumbers.Config;
using AngelDamageNumbers.Managers;
using UnityEngine;

namespace AngelDamageNumbers.Utilities
{
    public static class SettingsSaver
    {
        private static Coroutine? _pending;
        private const float DebounceSeconds = 10.0f;

        // when the next save is allowed; each Schedule() pushes this out
        private static float _deadlineRealtime;

        private static bool _suppress;

        public static void Suppress(bool on) => _suppress = on;

        /// <summary>
        /// Request a save. Multiple calls within DebounceSeconds coalesce into one write.
        /// </summary>
        public static void Schedule()
        {
            if (_suppress) return;

            // push the deadline out DebounceSeconds from now
            var now = Time.unscaledTime; // realtime
            _deadlineRealtime = now + DebounceSeconds;

            if (_pending == null)
                _pending = CoroutineManager.Instance.StartCoroutine(DebouncedSave());
        }

        public static void FlushNow()
        {
            if (_pending != null)
            {
                CoroutineManager.Instance.StopCoroutine(_pending);
                _pending = null;
            }
            XmlHandler.SaveSettings();
        }

        private static IEnumerator DebouncedSave()
        {
            // wait until no new calls have arrived for DebounceSeconds
            while (Time.unscaledTime < _deadlineRealtime)
                yield return null; // frame-by-frame, realtime

            XmlHandler.SaveSettings();
            _pending = null;
        }
    }
}