using System;
using Config;
using Gears;
using Managers;
using UI;
using Object = UnityEngine.Object;

namespace Utilities
{
    public static class CleanUpHelper
    {
        public static bool IsCleanupPerformed { get; private set; }

        public static void PerformFullCleanup()
        {
            if (IsCleanupPerformed)
            {
                AdnLogger.Debug("Cleanup already performed, skipping");
                return;
            }

            AdnLogger.Debug("Starting comprehensive mod cleanup");

            try
            {
                CleanupUtilities();
                CleanupManagers();
                CleanupUI();
                CleanupConfiguration();
                CleanupGears();

                IsCleanupPerformed = true;
                AdnLogger.Debug("Comprehensive mod cleanup completed successfully");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error during comprehensive cleanup: {ex.Message}");
                // Continue with cleanup even if some parts fail
            }
        }

        private static void CleanupUtilities()
        {
            try
            {
                AdnLogger.Debug("Cleaning up utility classes");

                FontUtils.CleanupStatics();
                CameraUtils.CleanupStatics();

                AdnLogger.Debug("Utility cleanup completed");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error during utility cleanup: {ex.Message}");
            }
        }

        private static void CleanupManagers()
        {
            try
            {
                AdnLogger.Debug("Cleaning up manager classes");

                CrosshairManager.CleanupStatics();

                // CoroutineManager cleanup - destroy the instance
                var coroutineManager = CoroutineManager.Instance;
                if (coroutineManager != null)
                {
                    Object.Destroy(coroutineManager.gameObject);
                    AdnLogger.Debug("CoroutineManager destroyed");
                }

                AdnLogger.Debug("Manager cleanup completed");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error during manager cleanup: {ex.Message}");
            }
        }

        private static void CleanupUI()
        {
            try
            {
                AdnLogger.Debug("Cleaning up UI classes");

                HarmonyManager.CleanupStatics();
                AdnDamageText.CleanupStatics();

                AdnLogger.Debug("UI cleanup completed");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error during UI cleanup: {ex.Message}");
            }
        }

        private static void CleanupConfiguration()
        {
            try
            {
                AdnLogger.Debug("Cleaning up configuration systems");

                ConfigurationService.CleanupStatics();

                AdnLogger.Debug("Configuration cleanup completed");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error during configuration cleanup: {ex.Message}");
            }
        }

        private static void CleanupGears()
        {
            try
            {
                AdnLogger.Debug("Cleaning up Gears integration");

                GearsManager.CleanupStatics();
                GearsDetector.ClearCache();

                AdnLogger.Debug("Gears cleanup completed");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error during Gears cleanup: {ex.Message}");
            }
        }

        public static void CleanupDamageDisplay()
        {
            try
            {
                AdnLogger.Debug("Cleaning up damage display system");

                HarmonyManager.CleanupStatics();
                AdnDamageText.CleanupStatics();

                AdnLogger.Debug("Damage display cleanup completed");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error during damage display cleanup: {ex.Message}");
            }
        }

        public static void CleanupCrosshairSystem()
        {
            try
            {
                AdnLogger.Debug("Cleaning up crosshair system");

                CrosshairManager.CleanupStatics();

                AdnLogger.Debug("Crosshair system cleanup completed");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error during crosshair system cleanup: {ex.Message}");
            }
        }

        public static void EmergencyCleanup()
        {
            try
            {
                AdnLogger.Warning("Performing emergency cleanup");

                // More aggressive cleanup - destroy all GameObjects we know about
                try
                {
                    var crosshairObjects = Object.FindObjectsOfType<CrosshairManager>();
                    foreach (var obj in crosshairObjects)
                        if (obj != null)
                            Object.DestroyImmediate(obj.gameObject);
                }
                catch (Exception ex)
                {
                    AdnLogger.Error($"Error during emergency crosshair cleanup: {ex.Message}");
                }

                try
                {
                    var damageTextObjects = Object.FindObjectsOfType<AdnDamageText>();
                    foreach (var obj in damageTextObjects)
                        if (obj != null)
                            Object.DestroyImmediate(obj.gameObject);
                }
                catch (Exception ex)
                {
                    AdnLogger.Error($"Error during emergency damage text cleanup: {ex.Message}");
                }

                try
                {
                    var coroutineObjects = Object.FindObjectsOfType<CoroutineManager>();
                    foreach (var obj in coroutineObjects)
                        if (obj != null)
                            Object.DestroyImmediate(obj.gameObject);
                }
                catch (Exception ex)
                {
                    AdnLogger.Error($"Error during emergency coroutine cleanup: {ex.Message}");
                }

                // Perform regular cleanup as well
                PerformFullCleanup();

                AdnLogger.Warning("Emergency cleanup completed");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Critical error during emergency cleanup: {ex.Message}");
            }
        }

        public static void ResetCleanupState()
        {
            IsCleanupPerformed = false;
            AdnLogger.Debug("Cleanup state reset");
        }

        public static bool ValidateCleanup()
        {
            try
            {
                AdnLogger.Debug("Validating cleanup results");

                var allClean = true;

                // Check if any of our GameObjects still exist
                var crosshairManagers = Object.FindObjectsOfType<CrosshairManager>();
                if (crosshairManagers.Length > 0)
                {
                    AdnLogger.Warning($"Found {crosshairManagers.Length} CrosshairManager instances after cleanup");
                    allClean = false;
                }

                var damageTextComponents = Object.FindObjectsOfType<AdnDamageText>();
                if (damageTextComponents.Length > 0)
                {
                    AdnLogger.Warning($"Found {damageTextComponents.Length} AdnDamageText instances after cleanup");
                    allClean = false;
                }

                var coroutineManagers = Object.FindObjectsOfType<CoroutineManager>();
                if (coroutineManagers.Length > 0)
                {
                    AdnLogger.Warning($"Found {coroutineManagers.Length} CoroutineManager instances after cleanup");
                    allClean = false;
                }

                if (allClean)
                    AdnLogger.Debug("Cleanup validation passed - no mod objects found");
                else
                    AdnLogger.Warning("Cleanup validation found remaining mod objects");

                return allClean;
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error during cleanup validation: {ex.Message}");
                return false;
            }
        }
    }
}