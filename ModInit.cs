using UnityEngine;

public class ModInit : IModApi
{
    public void InitMod(Mod modInstance)
    {
        Debug.Log("[Angel-DamageNumbers] ModInit called");

        // Load configuration asynchronously to avoid blocking game startup
        // Use LoadConfigAsync() for non-blocking or LoadConfig() for immediate loading
        FloatingDamageNumbersConfig.LoadConfigAsync();

        // Ensure CoroutineRunner is available
        var coroutineRunner = CoroutineRunner.Instance;
        if (coroutineRunner != null)
        {
            Debug.Log("[Angel-DamageNumbers] CoroutineRunner instance created successfully");
        }
        else
        {
            Debug.LogError("[Angel-DamageNumbers] Failed to create CoroutineRunner instance");
        }

        Debug.Log("[Angel-DamageNumbers] ModInit completed");
    }
}