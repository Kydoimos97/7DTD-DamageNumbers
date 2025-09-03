using System;
using AngelDamageNumbers.Utilities;
using HarmonyLib;

namespace AngelDamageNumbers.UI
{
    public class AdnDamageNumbers : IModApi
    {
        public void InitMod(Mod mod)
        {
            AdnLogger.Debug("init called");

            try
            {
                var harmony = new Harmony("enhanced.angel.damage_numbers");
                harmony.PatchAll();

                // List applied patches for debugging
                var patches = harmony.GetPatchedMethods();
                foreach (var method in patches) AdnLogger.Debug($"Patched method: {method.DeclaringType?.Name}.{method.Name}");
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Harmony patching failed: {ex.Message}");
                AdnLogger.Error($"Exception details: {ex}");
            }
        }
    }
}