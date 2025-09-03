using System;
using AngelDamageNumbers.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AngelDamageNumbers
{
    sealed class AdnBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            // Only one
            if (FindObjectOfType<AdnBootstrap>() != null) return;

            var go = new GameObject("[Angel-DamageNumbers Bootstrap]");
            DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<AdnBootstrap>();
        }

        private void OnEnable()
        {
            Application.quitting += OnQuitting;
            SceneManager.activeSceneChanged += OnSceneChanged;

            // In Mono builds this can fire; IL2CPP may not support ProcessExit
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
        }

        private void OnDisable()
        {
            Application.quitting -= OnQuitting;
            SceneManager.activeSceneChanged -= OnSceneChanged;
            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
            AppDomain.CurrentDomain.DomainUnload -= OnDomainUnload;
        }

        private void OnApplicationQuit() => SafeCleanup("OnApplicationQuit");
        private void OnQuitting()        => SafeCleanup("Application.quitting");
        private void OnDestroy()         => SafeCleanup("AdnBootstrap.OnDestroy");
        private void OnSceneChanged(Scene _, Scene __) { /* optional: no-op */ }
        private void OnProcessExit(object? s, EventArgs e) => SafeCleanup("ProcessExit");
        private void OnDomainUnload(object? s, EventArgs e) => SafeCleanup("DomainUnload");

        private static void SafeCleanup(string origin)
        {
            // Idempotent thanks to CleanUpHelper.IsCleanupPerformed
            try
            {
                AdnLogger.Debug($"Cleanup via {origin}");
                SettingsSaver.FlushNow();           // no coroutines during shutdown
                CleanUpHelper.PerformFullCleanup();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Angel-DamageNumbers] Cleanup error ({origin}): {ex.Message}");
            }
        }
    }
}
