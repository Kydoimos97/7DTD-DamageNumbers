using System.Collections.Concurrent;
using AngelDamageNumbers.UI;
using AngelDamageNumbers.Utilities;
using Config;
using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable UnusedMember.Local

namespace AngelDamageNumbers.Managers
{
    [HarmonyPatch(typeof(EntityAlive), "DamageEntity")]
    public class HarmonyManager
    {
        private static readonly ConcurrentDictionary<int, float> LastDamageTime = new ConcurrentDictionary<int, float>();

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
            AdnLogger.Debug($"DamageEntity postfix called - Entity: {__instance?.GetType().Name}, Damage: {strength}");

            var gameManager = GameManager.Instance;
            if (gameManager?.World == null)
            {
                AdnLogger.Debug("GameManager or World not available, aborting");
                return;
            }

            var primaryPlayer = gameManager.World.GetPrimaryPlayer();
            if (primaryPlayer == null)
            {
                AdnLogger.Debug("Primary player not found, aborting");
                return;
            }

            // Don't show damage numbers for the player themselves
            if (__instance == primaryPlayer)
            {
                AdnLogger.Debug("Damage was to player themselves, skipping");
                return;
            }

            // Check minimum damage threshold
            if (strength < ConfigurationService.Current.MinimumDamageThreshold)
            {
                AdnLogger.Debug($"Damage {strength} below threshold {ConfigurationService.Current.MinimumDamageThreshold}, skipping");
                return;
            }

            if (__instance == null)
            {
                AdnLogger.Debug("Patch received null __instance, skipping");
                return;
            }

            var damageDealer = __instance.world?.GetEntity(damageSource.getEntityId());
            AdnLogger.Debug($"Damage dealer: {damageDealer?.GetType().Name} (ID: {damageSource.getEntityId()})");

            // If player damage only is enabled, check if player caused the damage
            if (ConfigurationService.Current.PlayerDamageOnly && damageDealer != primaryPlayer)
            {
                AdnLogger.Debug("PlayerDamageOnly is enabled and damage wasn't caused by player, skipping");
                return;
            }

            // Thread-safe rate limiting check
            var entityId = __instance.entityId;
            var currentTime = Time.time;

            if (LastDamageTime.TryGetValue(entityId, out var lastTime))
            {
                var timeSinceLastDamage = currentTime - lastTime;
                if (timeSinceLastDamage < ConfigurationService.Current.DamageNumberCooldown)
                {
                    AdnLogger.Debug($"Rate limited - {timeSinceLastDamage:F2}s since last damage (cooldown: {ConfigurationService.Current.DamageNumberCooldown}s)");
                    return;
                }
            }

            LastDamageTime.AddOrUpdate(entityId, currentTime, (key, oldValue) => currentTime);

            // Get damage info
            var isDead = __instance.IsDead();
            EnumBodyPartHit? bodyPart = damageSource.GetEntityDamageBodyPart(__instance);
            var isHeadshot = bodyPart.HasValue && bodyPart.GetValueOrDefault() == EnumBodyPartHit.Head;

            AdnLogger.Debug($"Damage info - Dead: {isDead}, Headshot: {isHeadshot}, BodyPart: {bodyPart}");

            // Determine damage text color
            var damageColor = GetDamageColor(isDead, isHeadshot);
            AdnLogger.Debug($"Damage color determined: {FontUtils.ColorToString(damageColor)}");

            // Calculate text offset with optional randomization
            var textOffset = ConfigurationService.Current.TextOffset;
            if (ConfigurationService.Current.RandomizePosition)
            {
                var randomX = Random.Range(-ConfigurationService.Current.PositionRandomness, ConfigurationService.Current.PositionRandomness);
                var randomZ = Random.Range(-ConfigurationService.Current.PositionRandomness, ConfigurationService.Current.PositionRandomness);
                textOffset += new Vector3(randomX, 0f, randomZ);
                AdnLogger.Debug($"Randomized text offset: {textOffset}");
            }

            // Show floating damage number
            AdnLogger.Debug($"Creating damage text: -{strength} at {textOffset}");
            AdnDamageText.Show("-" + strength, __instance, textOffset, damageColor, strength);

            // Show crosshair marker only if the player caused the damage
            if (damageDealer == primaryPlayer && ConfigurationService.Current.EnableCrosshairMarkers)
            {
                AdnLogger.Debug("Showing crosshair marker");
                ShowCrosshairMarker(isDead, isHeadshot);
            }
            else if (!ConfigurationService.Current.EnableCrosshairMarkers)
            {
                AdnLogger.Debug("Crosshair markers disabled in config");
            }
        }

        private static Color GetDamageColor(bool isDead, bool isHeadshot)
        {
            if (isDead && isHeadshot)
                return ConfigurationService.Current.HeadshotKillDamageColor;
            if (isDead)
                return ConfigurationService.Current.KillDamageColor;
            if (isHeadshot)
                return ConfigurationService.Current.HeadshotDamageColor;
            return ConfigurationService.Current.NormalDamageColor;
        }

        private static void ShowCrosshairMarker(bool isDead, bool isHeadshot)
        {
            string marker;
            Color markerColor;

            if (isDead && isHeadshot)
            {
                marker = ConfigurationService.Current.HeadshotKillMarker;
                markerColor = ConfigurationService.Current.HeadshotKillMarkerColor;
            }
            else if (isDead)
            {
                marker = ConfigurationService.Current.KillMarker;
                markerColor = ConfigurationService.Current.KillMarkerColor;
            }
            else if (isHeadshot)
            {
                marker = ConfigurationService.Current.HeadshotMarker;
                markerColor = ConfigurationService.Current.HeadshotMarkerColor;
            }
            else
            {
                marker = ConfigurationService.Current.NormalHitMarker;
                markerColor = ConfigurationService.Current.NormalMarkerColor;
            }

            AdnLogger.Debug($"Crosshair marker: '{marker}' with color {FontUtils.ColorToString(markerColor)}");
            CrosshairManager.Instance?.ShowMarker(marker, markerColor);
        }

        public static void CleanupStatics()
        {
            LastDamageTime.Clear();
            AdnLogger.Debug("HarmonyManager static references cleaned up");
        }
    }
}