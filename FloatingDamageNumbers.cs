using System;
using HarmonyLib;
using UnityEngine;

public class FloatingDamageNumbers : IModApi
{
    public void InitMod(Mod mod)
    {
        Debug.Log("[Angel-DamageNumbers] init called");

        try
        {
            var harmony = new Harmony("enhanced.angel.damage_numbers");
            harmony.PatchAll();

            // List applied patches for debugging
            var patches = harmony.GetPatchedMethods();
            foreach (var method in patches)
            {
                Debug.Log($"[Angel-DamageNumbers] Patched method: {method.DeclaringType?.Name}.{method.Name}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Angel-DamageNumbers] Harmony patching failed: {ex.Message}");
            Debug.LogError($"[Angel-DamageNumbers] Exception details: {ex}");
        }
    }
}