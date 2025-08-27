using UnityEngine;

public static class FDamageTextUtils
{
    public static Camera GetCamera()
    {
        Camera main = Camera.main;
        if (main == null)
        {
            FloatingDamageNumbersConfig.DebugLog("Camera.main is null, searching for any camera");
            Camera[] cameras = Object.FindObjectsOfType<Camera>();
            FloatingDamageNumbersConfig.DebugLog($"Found {cameras.Length} cameras in scene");

            if (cameras.Length > 0)
            {
                FloatingDamageNumbersConfig.DebugLog($"Using first camera found: {cameras[0].name}");
                return cameras[0];
            }
            else
            {
                FloatingDamageNumbersConfig.DebugLog("No cameras found in scene!");
            }
        }
        return main;
    }
}