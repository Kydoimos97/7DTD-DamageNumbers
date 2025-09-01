using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    public static class CameraUtils
    {
        private static Camera _cache;

        public static Camera GetBestCamera(bool refresh = false, bool log = false)
        {
            if (!refresh && IsUsable(_cache)) return _cache;

            // 1) Prefer the tagged MainCamera if usable
            var main = Camera.main;
            if (main != null && IsUsable(main))
            {
                _cache = main;
                if (log) AdnLogger.Debug($"Using Camera.main: {main.name}");
                return _cache;
            }

            // 2) Pick any enabled, active camera on display 0 with highest depth
            var all = Camera.allCameras;
            if (all != null && all.Length > 0)
            {
                Camera best = null;
                var bestDepth = float.NegativeInfinity;

                for (var i = 0; i < all.Length; i++)
                {
                    var c = all[i];
                    if (c != null && IsUsable(c) && c.targetDisplay == 0 && c.depth >= bestDepth)
                    {
                        bestDepth = c.depth;
                        best = c;
                    }
                }

                if (best != null)
                {
                    _cache = best;
                    if (log) AdnLogger.Debug($"Using best-by-depth: {best.name} (depth {best.depth})");
                    return _cache;
                }
            }

            // 3) Last resort: any camera Unity knows about, even if inactive
            try
            {
                var any = Object.FindObjectsOfType<Camera>(true);
                if (any != null && any.Length > 0)
                {
                    // Prefer active cameras
                    foreach (var t in any)
                        if (t != null && t.gameObject.activeInHierarchy)
                        {
                            _cache = t;
                            if (log) AdnLogger.Debug($"Fallback active camera: {_cache.name}");
                            return _cache;
                        }

                    // If no active cameras, take any camera
                    _cache = any[0];
                    if (log) AdnLogger.Debug($"Fallback inactive camera: {_cache?.name ?? "null"}");
                    return _cache;
                }
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error finding fallback camera: {ex.Message}");
            }

            _cache = null;
            if (log) AdnLogger.Debug("No cameras found");
            return _cache;
        }

        private static bool IsUsable(Camera c)
        {
            return c != null &&
                   c.enabled &&
                   c.gameObject != null &&
                   c.gameObject.activeInHierarchy;
        }


        public static void CleanupStatics()
        {
            _cache = null;
            AdnLogger.Debug("CameraUtils static references cleaned up");
        }

        public static Camera GetUICamera()
        {
            var cam = GetBestCamera();

            // Validate the camera is suitable for UI work
            if (cam != null && cam.orthographic)
            {
                AdnLogger.Debug($"Using orthographic camera for UI: {cam.name}");
                return cam;
            }

            if (cam != null && !cam.orthographic)
            {
                AdnLogger.Debug($"Using perspective camera for UI: {cam.name}");
                return cam;
            }

            AdnLogger.Warning("No suitable camera found for UI work");
            return cam;
        }

        public static Camera GetWorldCamera()
        {
            var cam = GetBestCamera();

            if (cam != null)
            {
                AdnLogger.Debug($"Using camera for world calculations: {cam.name}");
                return cam;
            }

            AdnLogger.Warning("No camera found for world space calculations");
            return null;
        }

        public static bool HasUsableCamera()
        {
            return GetBestCamera() != null;
        }
    }
}