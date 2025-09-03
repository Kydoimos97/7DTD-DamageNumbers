using System;
using UniLinq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AngelDamageNumbers.Utilities
{
    public static class CameraUtils
    {
        private static Camera? _cache;

        public static Camera? GetBestCamera(bool refresh = false, bool log = false)
        {
            if (!refresh && IsUsable(_cache)) return _cache;

            if (log) AdnLogger.Debug("Searching for best camera...");

            // Clear cache when refreshing
            if (refresh) _cache = null;

            // 1) Try main camera first
            if (Camera.main != null && IsUsable(Camera.main))
            {
                _cache = Camera.main;
                if (log) AdnLogger.Debug($"Using main camera: {_cache.name}");
                return _cache;
            }

            // 2) Find best active camera by depth
            var allCameras = Camera.allCameras;
            if (allCameras != null && allCameras.Length > 0)
            {
                Camera? bestCamera = null;
                var bestDepth = float.NegativeInfinity;

                foreach (var camera in allCameras)
                {
                    if (IsUsable(camera) && camera.depth > bestDepth)
                    {
                        bestCamera = camera;
                        bestDepth = camera.depth;
                    }
                }

                if (bestCamera != null)
                {
                    _cache = bestCamera;
                    if (log) AdnLogger.Debug($"Using best active camera: {_cache.name} (depth: {bestDepth})");
                    return _cache;
                }
            }

            // 3) Last resort: any camera Unity knows about, even if inactive
            try
            {
                var allCamerasIncludingInactive = UnityEngine.Object.FindObjectsOfType<Camera>(true);
                if (allCamerasIncludingInactive?.Length > 0)
                {
                    // Prefer active cameras first
                    var activeCamera = allCamerasIncludingInactive.FirstOrDefault(c => c != null && c.gameObject.activeInHierarchy);
                    if (activeCamera != null)
                        {
                        _cache = activeCamera;
                        if (log) AdnLogger.Debug($"Using any active camera: {_cache.name}");
                            return _cache;
                        }

                    // If no active cameras, take any camera
                    var anyCamera = allCamerasIncludingInactive.FirstOrDefault(c => c != null);
                    if (anyCamera != null)
                    {
                        _cache = anyCamera;
                        if (log) AdnLogger.Debug($"Using any camera (inactive): {_cache.name}");
                    return _cache;
                }
            }
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Exception finding cameras: {ex.Message}");
            }

            AdnLogger.Warning("No usable camera found");
            return null;
        }

        private static bool IsUsable(Camera? camera)
        {
            return camera != null &&
                   camera.enabled &&
                   camera.gameObject != null &&
                   camera.gameObject.activeInHierarchy;
        }

        public static void CleanupStatics()
        {
            _cache = null;
            AdnLogger.Debug("CameraUtils static references cleaned up");
        }

        public static Camera? GetUICamera()
        {
            var camera = GetBestCamera();

            if (camera == null)
            {
                AdnLogger.Warning("No camera available for UI rendering");
                return null;
            }

            // UI cameras typically render everything
            if (camera.cullingMask == -1 || (camera.cullingMask & (1 << LayerMask.NameToLayer("UI"))) != 0)
            {
                return camera;
            }

            AdnLogger.Debug($"Camera {camera.name} may not render UI layer properly");
            return camera;
        }

        public static Camera? GetWorldCamera()
        {
            var camera = GetBestCamera();

            if (camera == null)
            {
                AdnLogger.Warning("No camera available for world rendering");
            return null;
        }

            return camera;
        }
    }
}