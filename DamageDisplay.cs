using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

[HarmonyPatch(typeof(EntityAlive), "DamageEntity")]
public class DamageDisplay
{
    private static Dictionary<int, float> _lastDamageTime = new Dictionary<int, float>();

    private static void Postfix(
        // ReSharper disable once InconsistentNaming
        EntityAlive __instance,
        [HarmonyArgument("_damageSource")] DamageSource damageSource,
        [HarmonyArgument("_strength")] int strength,
        // ReSharper disable once UnusedParameter.Local
        [HarmonyArgument("_criticalHit")] bool criticalHit,
        // ReSharper disable once UnusedParameter.Local
        [HarmonyArgument("_impulseScale")] float impulseScale)
    {
        FloatingDamageNumbersConfig.DebugLog($"DamageEntity postfix called - Entity: {__instance?.GetType().Name}, Damage: {strength}");

        EntityPlayerLocal primaryPlayer = GameManager.Instance?.World?.GetPrimaryPlayer();
        if (primaryPlayer == null)
        {
            FloatingDamageNumbersConfig.DebugLog("Primary player not found, aborting");
            return;
        }

        // Don't show damage numbers for the player themselves
        if (__instance == primaryPlayer)
        {
            FloatingDamageNumbersConfig.DebugLog("Damage was to player themselves, skipping");
            return;
        }

        // Check minimum damage threshold
        if (strength < FloatingDamageNumbersConfig.MinimumDamageThreshold)
        {
            FloatingDamageNumbersConfig.DebugLog($"Damage {strength} below threshold {FloatingDamageNumbersConfig.MinimumDamageThreshold}, skipping");
            return;
        }

        if (__instance == null)
        {
            FloatingDamageNumbersConfig.DebugLog("Patch received null __instance, skipping");
            return;
        }

        Entity damageDealer = __instance.world?.GetEntity(damageSource.getEntityId());
        FloatingDamageNumbersConfig.DebugLog($"Damage dealer: {damageDealer?.GetType().Name} (ID: {damageSource.getEntityId()})");

        // If player damage only is enabled, check if player caused the damage
        if (FloatingDamageNumbersConfig.PlayerDamageOnly && damageDealer != primaryPlayer)
        {
            FloatingDamageNumbersConfig.DebugLog("PlayerDamageOnly is enabled and damage wasn't caused by player, skipping");
            return;
        }

        // Rate limiting check
        int entityId = __instance.entityId;
        float currentTime = Time.time;
        if (_lastDamageTime.ContainsKey(entityId))
        {
            float timeSinceLastDamage = currentTime - _lastDamageTime[entityId];
            if (timeSinceLastDamage < FloatingDamageNumbersConfig.DamageNumberCooldown)
            {
                FloatingDamageNumbersConfig.DebugLog($"Rate limited - {timeSinceLastDamage:F2}s since last damage (cooldown: {FloatingDamageNumbersConfig.DamageNumberCooldown}s)");
                return;
            }
        }
        _lastDamageTime[entityId] = currentTime;

        // Get damage info
        bool isDead = __instance.IsDead();
        EnumBodyPartHit? bodyPart = damageSource.GetEntityDamageBodyPart(__instance);
        bool isHeadshot = bodyPart.HasValue && bodyPart.GetValueOrDefault() == EnumBodyPartHit.Head;

        FloatingDamageNumbersConfig.DebugLog($"Damage info - Dead: {isDead}, Headshot: {isHeadshot}, BodyPart: {bodyPart}");

        // Determine damage text color
        Color damageColor = GetDamageColor(isDead, isHeadshot);
        FloatingDamageNumbersConfig.DebugLog($"Damage color determined: {ColorToString(damageColor)}");

        // Calculate text offset with optional randomization
        Vector3 textOffset = FloatingDamageNumbersConfig.TextOffset;
        if (FloatingDamageNumbersConfig.RandomizePosition)
        {
            float randomX = Random.Range(-FloatingDamageNumbersConfig.PositionRandomness, FloatingDamageNumbersConfig.PositionRandomness);
            float randomZ = Random.Range(-FloatingDamageNumbersConfig.PositionRandomness, FloatingDamageNumbersConfig.PositionRandomness);
            textOffset += new Vector3(randomX, 0f, randomZ);
            FloatingDamageNumbersConfig.DebugLog($"Randomized text offset: {textOffset}");
        }

        // Show floating damage number
        FloatingDamageNumbersConfig.DebugLog($"Creating damage text: -{strength} at {textOffset}");
        FDamageText.Show("-" + strength, __instance, textOffset, damageColor, strength);

        // Show crosshair marker only if the player caused the damage
        if (damageDealer == primaryPlayer && FloatingDamageNumbersConfig.EnableCrosshairMarkers)
        {
            FloatingDamageNumbersConfig.DebugLog("Showing crosshair marker");
            ShowCrosshairMarker(isDead, isHeadshot);
        }
        else if (!FloatingDamageNumbersConfig.EnableCrosshairMarkers)
        {
            FloatingDamageNumbersConfig.DebugLog("Crosshair markers disabled in config");
        }
    }

    private static Color GetDamageColor(bool isDead, bool isHeadshot)
    {
        if (isDead && isHeadshot)
            return FloatingDamageNumbersConfig.HeadshotKillDamageColor;
        else if (isDead)
            return FloatingDamageNumbersConfig.KillDamageColor;
        else if (isHeadshot)
            return FloatingDamageNumbersConfig.HeadshotDamageColor;
        else
            return FloatingDamageNumbersConfig.NormalDamageColor;
    }

    private static void ShowCrosshairMarker(bool isDead, bool isHeadshot)
    {
        string marker;
        Color markerColor;

        if (isDead && isHeadshot)
        {
            marker = FloatingDamageNumbersConfig.HeadshotKillMarker;
            markerColor = FloatingDamageNumbersConfig.HeadshotKillMarkerColor;
        }
        else if (isDead)
        {
            marker = FloatingDamageNumbersConfig.KillMarker;
            markerColor = FloatingDamageNumbersConfig.KillMarkerColor;
        }
        else if (isHeadshot)
        {
            marker = FloatingDamageNumbersConfig.HeadshotMarker;
            markerColor = FloatingDamageNumbersConfig.HeadshotMarkerColor;
        }
        else
        {
            marker = FloatingDamageNumbersConfig.NormalHitMarker;
            markerColor = FloatingDamageNumbersConfig.NormalMarkerColor;
        }

        FloatingDamageNumbersConfig.DebugLog($"Crosshair marker: '{marker}' with color {ColorToString(markerColor)}");
        CrosshairManager.Instance?.ShowMarker(marker, markerColor);
    }

    private static string ColorToString(Color color)
    {
        return $"({color.r:F2}, {color.g:F2}, {color.b:F2}, {color.a:F2})";
    }
}