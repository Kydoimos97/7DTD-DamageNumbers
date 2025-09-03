using System;
using System.Globalization;
using AngelDamageNumbers.Config;
using AngelDamageNumbers.Utilities;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable UnusedParameter.Local

namespace AngelDamageNumbers.Gears;

public class GearsCallBacks
{
    private static GearsSettings S
        => GearsIntegration.Settings ?? throw new InvalidOperationException("[Gears] Settings not initialized.");

        public GearsCallBacks()
        {
            AdnLogger.Debug("Creating Gears Callbacks");
            try
            {
                CreateCallBacks();
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error while creating Gears Callbacks! {ex}");
                throw;
            }

            AdnLogger.Debug("Applying Gears Settings to Config");
            try
            {
                ApplyGearsSettingsToConfig();
            }
            catch (Exception ex)
            {
                AdnLogger.Error($"Error while applying Gears Settings! {ex}");
                throw;
            }
        }

        private void CreateCallBacks()
        {
            S.EnableDebugLogging.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.EnableDebugLogging = bool.Parse(newValue);
                AdnLogger.Debug($"Debug logging {(ConfigurationService.Current.EnableDebugLogging ? "enabled" : "disabled")}");
                SettingsSaver.Schedule();
            };

            S.MinimumDamageThreshold.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MinimumDamageThreshold = int.Parse(newValue);
                AdnLogger.Debug($"Minimum damage threshold changed to {ConfigurationService.Current.MinimumDamageThreshold}");
                SettingsSaver.Schedule();
            };

            S.DamageNumberCooldown.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.DamageNumberCooldown = float.Parse(newValue, NumberStyles.Float, CultureInfo.InvariantCulture);
                AdnLogger.Debug($"Damage cooldown changed to {ConfigurationService.Current.DamageNumberCooldown}s");
                SettingsSaver.Schedule();
            };

            S.FontSize.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.FontSize = int.Parse(newValue);
                AdnLogger.Debug($"Font size changed to {ConfigurationService.Current.FontSize}");
                SettingsSaver.Schedule();
            };

            S.TextLifetime.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.TextLifetime = float.Parse(newValue, NumberStyles.Float, CultureInfo.InvariantCulture);
                AdnLogger.Debug($"Text lifetime changed to {ConfigurationService.Current.TextLifetime}s");
                SettingsSaver.Schedule();
            };

            S.FloatSpeed.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.FloatSpeed = float.Parse(newValue, NumberStyles.Float, CultureInfo.InvariantCulture);
                AdnLogger.Debug($"Float speed changed to {ConfigurationService.Current.FloatSpeed}");
                SettingsSaver.Schedule();
            };

            // Color settings - these use CurrentColor property
            if (S.NormalDamageColor != null)
                S.NormalDamageColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.NormalDamageColor = S.NormalDamageColor.CurrentColor;
                    AdnLogger.Debug("Normal damage color changed");
                    SettingsSaver.Schedule();
                };

            if (S.HeadshotDamageColor != null)
                S.HeadshotDamageColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.HeadshotDamageColor = S.HeadshotDamageColor.CurrentColor;
                    AdnLogger.Debug("Headshot damage color changed");
                    SettingsSaver.Schedule();
            };

            if (S.KillDamageColor != null)
                S.KillDamageColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.KillDamageColor = S.KillDamageColor.CurrentColor;
                    AdnLogger.Debug("Kill damage color changed");
                    SettingsSaver.Schedule();
            };

            if (S.HeadshotKillDamageColor != null)
                S.HeadshotKillDamageColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.HeadshotKillDamageColor = S.HeadshotKillDamageColor.CurrentColor;
                    AdnLogger.Debug("Headshot kill damage color changed");
                    SettingsSaver.Schedule();
            };

            // Crosshair marker settings
            S.EnableCrosshairMarkers.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.EnableCrosshairMarkers = bool.Parse(newValue);
                AdnLogger.Debug($"Crosshair markers {(ConfigurationService.Current.EnableCrosshairMarkers ? "enabled" : "disabled")}");
                SettingsSaver.Schedule();
            };

            S.MarkerDuration.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MarkerDuration = float.Parse(newValue, NumberStyles.Float, CultureInfo.InvariantCulture);
                AdnLogger.Debug($"Marker duration changed to {ConfigurationService.Current.MarkerDuration}s");
                SettingsSaver.Schedule();
            };

            S.MarkerFontSize.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MarkerFontSize = int.Parse(newValue);
                AdnLogger.Debug($"Marker font size changed to {ConfigurationService.Current.MarkerFontSize}");
                SettingsSaver.Schedule();
            };

            S.NormalHitMarker.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.NormalHitMarker = newValue;
                AdnLogger.Debug($"Normal hit marker changed to '{newValue}'");
                SettingsSaver.Schedule();
            };

            S.KillMarker.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.KillMarker = newValue;
                AdnLogger.Debug($"Kill marker changed to '{newValue}'");
                SettingsSaver.Schedule();
            };

            S.HeadshotMarker.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.HeadshotMarker = newValue;
                AdnLogger.Debug($"Headshot marker changed to '{newValue}'");
                SettingsSaver.Schedule();
            };

            S.HeadshotKillMarker.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.HeadshotKillMarker = newValue;
                AdnLogger.Debug($"Headshot kill marker changed to '{newValue}'");
                SettingsSaver.Schedule();
            };

            // Marker colors
            if (S.NormalMarkerColor != null)
                S.NormalMarkerColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.NormalMarkerColor = S.NormalMarkerColor.CurrentColor;
                    AdnLogger.Debug("Normal marker color changed");
                    SettingsSaver.Schedule();
            };

            if (S.KillMarkerColor != null)
                S.KillMarkerColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.KillMarkerColor = S.KillMarkerColor.CurrentColor;
                    AdnLogger.Debug("Kill marker color changed");
                    SettingsSaver.Schedule();
            };

            if (S.HeadshotMarkerColor != null)
                S.HeadshotMarkerColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.HeadshotMarkerColor = S.HeadshotMarkerColor.CurrentColor;
                    AdnLogger.Debug("Headshot marker color changed");
                    SettingsSaver.Schedule();
            };

            if (S.HeadshotKillMarkerColor != null)
                S.HeadshotKillMarkerColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.HeadshotKillMarkerColor = S.HeadshotKillMarkerColor.CurrentColor;
                    AdnLogger.Debug("Headshot kill marker color changed");
                    SettingsSaver.Schedule();
            };

            // Text styling settings
            // S.FontName.OnSettingChanged += (setting, newValue) =>
            // {
            //     ConfigurationService.Current.FontName = newValue;
            //     AdnLogger.Debug($"Font name changed to {newValue}");
            //     var testFont = FontUtils.GetConfiguredTMPFont();
            //     if (testFont == null)
            //         AdnLogger.Warning("Font system returned null - text display may not work properly");
            //     else
            //         AdnLogger.Debug($"Font system validated - Using font: {testFont.name}");
            //     SettingsSaver.Schedule();
            // };

            S.EnableOutline.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.EnableOutline = bool.Parse(newValue);
                AdnLogger.Debug($"Text outline {(ConfigurationService.Current.EnableOutline ? "enabled" : "disabled")}");
                SettingsSaver.Schedule();
            };

            if (S.OutlineColor != null)
                S.OutlineColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.OutlineColor = S.OutlineColor.CurrentColor;
                    AdnLogger.Debug("Outline color changed");
                    SettingsSaver.Schedule();
            };

            S.OutlineThickness.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.OutlineThickness = float.Parse(newValue, NumberStyles.Float, CultureInfo.InvariantCulture);
                AdnLogger.Debug($"Outline thickness changed to {ConfigurationService.Current.OutlineThickness}");
                SettingsSaver.Schedule();
            };

            // Advanced settings
            S.PlayerDamageOnly.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.PlayerDamageOnly = bool.Parse(newValue);
                AdnLogger.Debug($"Player damage only {(ConfigurationService.Current.PlayerDamageOnly ? "enabled" : "disabled")}");
                SettingsSaver.Schedule();
            };

            S.RandomizePosition.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.RandomizePosition = bool.Parse(newValue);
                AdnLogger.Debug($"Position randomization {(ConfigurationService.Current.RandomizePosition ? "enabled" : "disabled")}");
                SettingsSaver.Schedule();
            };

            S.PositionRandomness.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.PositionRandomness = float.Parse(newValue, NumberStyles.Float, CultureInfo.InvariantCulture);
                AdnLogger.Debug($"Position randomness changed to {ConfigurationService.Current.PositionRandomness}");
                SettingsSaver.Schedule();
            };

            S.ScaleTextByDamage.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.ScaleTextByDamage = bool.Parse(newValue);
                AdnLogger.Debug($"Scale text by damage {(ConfigurationService.Current.ScaleTextByDamage ? "enabled" : "disabled")}");
                SettingsSaver.Schedule();
            };

            S.MinScale.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MinScale = float.Parse(newValue, NumberStyles.Float, CultureInfo.InvariantCulture);
                AdnLogger.Debug($"Minimum scale changed to {ConfigurationService.Current.MinScale}");
                SettingsSaver.Schedule();
            };

            S.MaxScale.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MaxScale = float.Parse(newValue, NumberStyles.Float, CultureInfo.InvariantCulture);
                AdnLogger.Debug($"Maximum scale changed to {ConfigurationService.Current.MaxScale}");
                SettingsSaver.Schedule();
            };

            S.MaxDamageForScale.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MaxDamageForScale = int.Parse(newValue);
                AdnLogger.Debug($"Max damage for scale changed to {ConfigurationService.Current.MaxDamageForScale}");
                SettingsSaver.Schedule();
            };
        }

        private void ApplyGearsSettingsToConfig()
        {
            // Apply all current Gears settings to our config system
            if (S.EnableDebugLogging != null)
                ConfigurationService.Current.EnableDebugLogging = bool.Parse(S.EnableDebugLogging.CurrentValue);

            if (S.MinimumDamageThreshold != null)
                ConfigurationService.Current.MinimumDamageThreshold = int.Parse(S.MinimumDamageThreshold.CurrentValue);

            if (S.DamageNumberCooldown != null)
                ConfigurationService.Current.DamageNumberCooldown = float.Parse(S.DamageNumberCooldown.CurrentValue, NumberStyles.Float, CultureInfo.InvariantCulture);

            if (S.FontSize != null)
                ConfigurationService.Current.FontSize = int.Parse(S.FontSize.CurrentValue);

            if (S.TextLifetime != null)
                ConfigurationService.Current.TextLifetime = float.Parse(S.TextLifetime.CurrentValue, NumberStyles.Float, CultureInfo.InvariantCulture);

            if (S.FloatSpeed != null)
                ConfigurationService.Current.FloatSpeed = float.Parse(S.FloatSpeed.CurrentValue, NumberStyles.Float, CultureInfo.InvariantCulture);

            // Apply color settings
            if (S.NormalDamageColor != null)
                ConfigurationService.Current.NormalDamageColor = S.NormalDamageColor.CurrentColor;

            if (S.HeadshotDamageColor != null)
                ConfigurationService.Current.HeadshotDamageColor = S.HeadshotDamageColor.CurrentColor;

            if (S.KillDamageColor != null)
                ConfigurationService.Current.KillDamageColor = S.KillDamageColor.CurrentColor;

            if (S.HeadshotKillDamageColor != null)
                ConfigurationService.Current.HeadshotKillDamageColor = S.HeadshotKillDamageColor.CurrentColor;

            // Apply crosshair settings
            if (S.EnableCrosshairMarkers != null)
                ConfigurationService.Current.EnableCrosshairMarkers = bool.Parse(S.EnableCrosshairMarkers.CurrentValue);

            if (S.MarkerDuration != null)
                ConfigurationService.Current.MarkerDuration = float.Parse(S.MarkerDuration.CurrentValue, NumberStyles.Float, CultureInfo.InvariantCulture);

            if (S.MarkerFontSize != null)
                ConfigurationService.Current.MarkerFontSize = int.Parse(S.MarkerFontSize.CurrentValue);

            if (S.NormalHitMarker != null)
                ConfigurationService.Current.NormalHitMarker = S.NormalHitMarker.CurrentValue;

            if (S.KillMarker != null)
                ConfigurationService.Current.KillMarker = S.KillMarker.CurrentValue;

            if (S.HeadshotMarker != null)
                ConfigurationService.Current.HeadshotMarker = S.HeadshotMarker.CurrentValue;

            if (S.HeadshotKillMarker != null)
                ConfigurationService.Current.HeadshotKillMarker = S.HeadshotKillMarker.CurrentValue;

            if (S.NormalMarkerColor != null)
                ConfigurationService.Current.NormalMarkerColor = S.NormalMarkerColor.CurrentColor;

            if (S.KillMarkerColor != null)
                ConfigurationService.Current.KillMarkerColor = S.KillMarkerColor.CurrentColor;

            if (S.HeadshotMarkerColor != null)
                ConfigurationService.Current.HeadshotMarkerColor = S.HeadshotMarkerColor.CurrentColor;

            if (S.HeadshotKillMarkerColor != null)
                ConfigurationService.Current.HeadshotKillMarkerColor = S.HeadshotKillMarkerColor.CurrentColor;

            // Apply text styling settings
            // if (S.FontName != null)
            //     ConfigurationService.Current.FontName = S.FontName.CurrentValue;

            if (S.EnableOutline != null)
                ConfigurationService.Current.EnableOutline = bool.Parse(S.EnableOutline.CurrentValue);

            if (S.OutlineColor != null)
                ConfigurationService.Current.OutlineColor = S.OutlineColor.CurrentColor;

            if (S.OutlineThickness != null)
                ConfigurationService.Current.OutlineThickness = float.Parse(S.OutlineThickness.CurrentValue, NumberStyles.Float, CultureInfo.InvariantCulture);

            // Apply advanced settings
            if (S.PlayerDamageOnly != null)
                ConfigurationService.Current.PlayerDamageOnly = bool.Parse(S.PlayerDamageOnly.CurrentValue);

            if (S.RandomizePosition != null)
                ConfigurationService.Current.RandomizePosition = bool.Parse(S.RandomizePosition.CurrentValue);

            if (S.PositionRandomness != null)
                ConfigurationService.Current.PositionRandomness = float.Parse(S.PositionRandomness.CurrentValue, NumberStyles.Float, CultureInfo.InvariantCulture);

            if (S.ScaleTextByDamage != null)
                ConfigurationService.Current.ScaleTextByDamage = bool.Parse(S.ScaleTextByDamage.CurrentValue);

            if (S.MinScale != null)
                ConfigurationService.Current.MinScale = float.Parse(S.MinScale.CurrentValue, NumberStyles.Float, CultureInfo.InvariantCulture);

            if (S.MaxScale != null)
                ConfigurationService.Current.MaxScale = float.Parse(S.MaxScale.CurrentValue, NumberStyles.Float, CultureInfo.InvariantCulture);

            if (S.MaxDamageForScale != null)
                ConfigurationService.Current.MaxDamageForScale = int.Parse(S.MaxDamageForScale.CurrentValue);
        }
}