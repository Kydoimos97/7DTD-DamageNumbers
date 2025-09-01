// GearsIntegration.cs (for Config folder)

using System;
using System.Globalization;
using Config;
using GearsAPI.Settings;
using GearsAPI.Settings.Global;
using GearsAPI.Settings.World;
using UnityEngine;
using Utilities;

namespace Gears
{
    public class GearsIntegration : IGearsModApi
    {
        private ISliderGlobalSetting _damageNumberCooldown;
        private ISwitchGlobalSetting _enableCrosshairMarkers;

        // Correct typed setting references for efficient access
        private ISwitchGlobalSetting _enableDebugLogging;
        private ISwitchGlobalSetting _enableOutline;
        private ISliderGlobalSetting _floatSpeed;
        private ISelectorGlobalSetting _fontName;
        private ISliderGlobalSetting _fontSize;
        private IModGlobalSettings _globalSettings;
        private IColorSelectorGlobalSetting _headshotDamageColor;
        private IColorSelectorGlobalSetting _headshotKillDamageColor;
        private IGlobalValueSetting _headshotKillMarker;
        private IColorSelectorGlobalSetting _headshotKillMarkerColor;
        private IGlobalValueSetting _headshotMarker;
        private IColorSelectorGlobalSetting _headshotMarkerColor;
        private IColorSelectorGlobalSetting _killDamageColor;
        private IGlobalValueSetting _killMarker;
        private IColorSelectorGlobalSetting _killMarkerColor;
        private ISliderGlobalSetting _markerDuration;
        private ISliderGlobalSetting _markerFontSize;
        private ISliderGlobalSetting _maxDamageForScale;
        private ISliderGlobalSetting _maxScale;
        private ISliderGlobalSetting _minimumDamageThreshold;
        private ISliderGlobalSetting _minScale;
        private IGearsMod _modInstance;
        private IColorSelectorGlobalSetting _normalDamageColor;
        private IGlobalValueSetting _normalHitMarker;
        private IColorSelectorGlobalSetting _normalMarkerColor;
        private IColorSelectorGlobalSetting _outlineColor;
        private ISliderGlobalSetting _outlineThickness;
        private ISwitchGlobalSetting _playerDamageOnly;
        private ISliderGlobalSetting _positionRandomness;
        private ISwitchGlobalSetting _randomizePosition;
        private ISwitchGlobalSetting _scaleTextByDamage;
        private ISliderGlobalSetting _textLifetime;
        private IGlobalValueSetting _textOffset;

        public void InitMod(IGearsMod modInstance)
        {
            _modInstance = modInstance;

            AdnLogger.Log("Gears integration initialized!");
        }

        public void OnGlobalSettingsLoaded(IModGlobalSettings globalSettings)
        {
            _globalSettings = globalSettings;

            try
            {
                CreateAllSettings();
                AdnLogger.Log("Gears settings created successfully!");
            }
            catch (MissingMethodException ex)
            {
                AdnLogger.Error($"Gears API method missing - incompatible version: {ex.Message}");
                return;
            }
            catch (InvalidOperationException ex)
            {
                AdnLogger.Error($"Invalid Gears operation: {ex.Message}");
                return;
            }
            catch (ArgumentException ex)
            {
                AdnLogger.Error($"Invalid argument when creating Gears settings: {ex.Message}");
                return;
            }

            try
            {
                SetupAllSettingChangeListeners();
                AdnLogger.Log("Gears event listeners configured!");
            }
            catch (ArgumentException ex)
            {
                AdnLogger.Error($"Invalid setting configuration: {ex.Message}");
                return;
            }
            catch (InvalidCastException ex)
            {
                AdnLogger.Error($"Setting type mismatch: {ex.Message}");
                return;
            }

            try
            {
                ApplyGearsSettingsToConfig();
                AdnLogger.Log("Gears global settings loaded and configured!");
            }
            catch (FormatException ex)
            {
                AdnLogger.Error($"Invalid setting format in Gears config: {ex.Message}");
            }
            catch (OverflowException ex)
            {
                AdnLogger.Error($"Setting value out of range: {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                AdnLogger.Error($"Required setting value is null: {ex.Message}");
            }
        }

        public void OnWorldSettingsLoaded(IModWorldSettings worldSettings)
        {
            AdnLogger.Debug("Gears world settings loaded!");
        }

        private void CreateAllSettings()
        {
            // Create a tab for our settings
            var settingsTab = _globalSettings.GetOrCreateTab("AngelDamageNumbers", "Angel's Damage Numbers");

            CreateDebugSettings(settingsTab);
            CreateDamageNumberSettings(settingsTab);
            CreateColorSettings(settingsTab);
            CreateCrosshairMarkerSettings(settingsTab);
            CreateTextStylingSettings(settingsTab);
            CreateAdvancedSettings(settingsTab);
        }

        private void CreateDebugSettings(IGlobalModSettingsTab settingsTab)
        {
            var debugCategory = settingsTab.GetOrCreateCategory("Debug", "Debug Settings");

            _enableDebugLogging = debugCategory.GetOrCreateSetting<ISwitchGlobalSetting>(
                "EnableDebugLogging",
                "Enable Debug Logging"
            );
            _enableDebugLogging.TooltipKey = "Enable debug messages in Unity console - useful for troubleshooting issues";
            _enableDebugLogging.SetSwitchValues("false", "true");
            _enableDebugLogging.DefaultValue = ConfigurationService.Current.EnableDebugLogging.ToString().ToLower();
        }

        private void CreateDamageNumberSettings(IGlobalModSettingsTab settingsTab)
        {
            var damageCategory = settingsTab.GetOrCreateCategory("DamageNumbers", "Damage Number Settings");

            _minimumDamageThreshold = damageCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "MinimumDamageThreshold",
                "Minimum Damage Threshold"
            );
            _minimumDamageThreshold.TooltipKey = "Minimum damage required to show floating numbers (0 = show all damage)";
            _minimumDamageThreshold.SetAllowedValues(1, 0, 100);
            _minimumDamageThreshold.DefaultValue = ConfigurationService.Current.MinimumDamageThreshold.ToString();

            _damageNumberCooldown = damageCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "DamageNumberCooldown",
                "Damage Number Cooldown"
            );
            _damageNumberCooldown.TooltipKey = "Minimum time between damage numbers in seconds (prevents spam)";
            _damageNumberCooldown.SetAllowedValues(0.1f, 0f, 2f);
            _damageNumberCooldown.DefaultValue = ConfigurationService.Current.DamageNumberCooldown.ToString(CultureInfo.InvariantCulture);

            _fontSize = damageCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "FontSize",
                "Font Size"
            );
            _fontSize.TooltipKey = "Size of the damage number text";
            _fontSize.SetAllowedValues(1, 8, 48);
            _fontSize.DefaultValue = ConfigurationService.Current.FontSize.ToString();

            _textLifetime = damageCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "TextLifetime",
                "Text Lifetime"
            );
            _textLifetime.TooltipKey = "How long the text is visible in seconds";
            _textLifetime.SetAllowedValues(0.1f, 0.5f, 5f);
            _textLifetime.DefaultValue = ConfigurationService.Current.TextLifetime.ToString(CultureInfo.InvariantCulture);

            _floatSpeed = damageCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "FloatSpeed",
                "Float Speed"
            );
            _floatSpeed.TooltipKey = "Speed at which text floats upward";
            _floatSpeed.SetAllowedValues(0.1f, 0.1f, 3f);
            _floatSpeed.DefaultValue = ConfigurationService.Current.FloatSpeed.ToString(CultureInfo.InvariantCulture);

            _textOffset = damageCategory.GetOrCreateSetting<IGlobalValueSetting>(
                "TextOffset",
                "Text Offset"
            );
            _textOffset.TooltipKey = "Offset from entity position in X,Y,Z format (e.g., 0,1.5,0)";
            _textOffset.DefaultValue = $"{ConfigurationService.Current.TextOffset.x},{ConfigurationService.Current.TextOffset.y},{ConfigurationService.Current.TextOffset.z}";
        }

        private void CreateColorSettings(IGlobalModSettingsTab settingsTab)
        {
            var colorCategory = settingsTab.GetOrCreateCategory("Colors", "Color Settings");

            _normalDamageColor = colorCategory.GetOrCreateSetting<IColorSelectorGlobalSetting>(
                "NormalDamageColor",
                "Normal Damage Color"
            );
            _normalDamageColor.TooltipKey = "Color for normal damage numbers";
            _normalDamageColor.DefaultColor = ConfigurationService.Current.NormalDamageColor;

            _headshotDamageColor = colorCategory.GetOrCreateSetting<IColorSelectorGlobalSetting>(
                "HeadshotDamageColor",
                "Headshot Damage Color"
            );
            _headshotDamageColor.TooltipKey = "Color for headshot damage numbers";
            _headshotDamageColor.DefaultColor = ConfigurationService.Current.HeadshotDamageColor;

            _killDamageColor = colorCategory.GetOrCreateSetting<IColorSelectorGlobalSetting>(
                "KillDamageColor",
                "Kill Damage Color"
            );
            _killDamageColor.TooltipKey = "Color for killing blow damage numbers";
            _killDamageColor.DefaultColor = ConfigurationService.Current.KillDamageColor;

            _headshotKillDamageColor = colorCategory.GetOrCreateSetting<IColorSelectorGlobalSetting>(
                "HeadshotKillDamageColor",
                "Headshot Kill Color"
            );
            _headshotKillDamageColor.TooltipKey = "Color for headshot kill damage numbers";
            _headshotKillDamageColor.DefaultColor = ConfigurationService.Current.HeadshotKillDamageColor;
        }

        private void CreateCrosshairMarkerSettings(IGlobalModSettingsTab settingsTab)
        {
            var markerCategory = settingsTab.GetOrCreateCategory("CrosshairMarkers", "Crosshair Hit Markers");

            _enableCrosshairMarkers = markerCategory.GetOrCreateSetting<ISwitchGlobalSetting>(
                "EnableCrosshairMarkers",
                "Enable Crosshair Markers"
            );
            _enableCrosshairMarkers.TooltipKey = "Show hit markers on your crosshair when hitting enemies";
            _enableCrosshairMarkers.SetSwitchValues("false", "true");
            _enableCrosshairMarkers.DefaultValue = ConfigurationService.Current.EnableCrosshairMarkers.ToString().ToLower();

            _markerDuration = markerCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "MarkerDuration",
                "Marker Duration"
            );
            _markerDuration.TooltipKey = "How long crosshair markers are visible in seconds";
            _markerDuration.SetAllowedValues(0.1f, 0.1f, 2f);
            _markerDuration.DefaultValue = ConfigurationService.Current.MarkerDuration.ToString(CultureInfo.InvariantCulture);

            _markerFontSize = markerCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "MarkerFontSize",
                "Marker Font Size"
            );
            _markerFontSize.TooltipKey = "Size of crosshair marker symbols";
            _markerFontSize.SetAllowedValues(1, 12, 60);
            _markerFontSize.DefaultValue = ConfigurationService.Current.MarkerFontSize.ToString();

            _normalHitMarker = markerCategory.GetOrCreateSetting<IGlobalValueSetting>(
                "NormalHitMarker",
                "Normal Hit Marker"
            );
            _normalHitMarker.TooltipKey = "Symbol for normal hits (try: x, ×, ⚡, ●)";
            _normalHitMarker.DefaultValue = ConfigurationService.Current.NormalHitMarker;

            _killMarker = markerCategory.GetOrCreateSetting<IGlobalValueSetting>(
                "KillMarker",
                "Kill Marker"
            );
            _killMarker.TooltipKey = "Symbol for kills (try: X, ✖, ☠, ★)";
            _killMarker.DefaultValue = ConfigurationService.Current.KillMarker;

            _headshotMarker = markerCategory.GetOrCreateSetting<IGlobalValueSetting>(
                "HeadshotMarker",
                "Headshot Marker"
            );
            _headshotMarker.TooltipKey = "Symbol for headshots (try: ◉, ⊙, ➤, ♦)";
            _headshotMarker.DefaultValue = ConfigurationService.Current.HeadshotMarker;

            _headshotKillMarker = markerCategory.GetOrCreateSetting<IGlobalValueSetting>(
                "HeadshotKillMarker",
                "Headshot Kill Marker"
            );
            _headshotKillMarker.TooltipKey = "Symbol for headshot kills (try: ✓, ★, ☠)";
            _headshotKillMarker.DefaultValue = ConfigurationService.Current.HeadshotKillMarker;

            _normalMarkerColor = markerCategory.GetOrCreateSetting<IColorSelectorGlobalSetting>(
                "NormalMarkerColor",
                "Normal Marker Color"
            );
            _normalMarkerColor.TooltipKey = "Color for normal hit markers";
            _normalMarkerColor.DefaultColor = ConfigurationService.Current.NormalMarkerColor;

            _killMarkerColor = markerCategory.GetOrCreateSetting<IColorSelectorGlobalSetting>(
                "KillMarkerColor",
                "Kill Marker Color"
            );
            _killMarkerColor.TooltipKey = "Color for kill markers";
            _killMarkerColor.DefaultColor = ConfigurationService.Current.KillMarkerColor;

            _headshotMarkerColor = markerCategory.GetOrCreateSetting<IColorSelectorGlobalSetting>(
                "HeadshotMarkerColor",
                "Headshot Marker Color"
            );
            _headshotMarkerColor.TooltipKey = "Color for headshot markers";
            _headshotMarkerColor.DefaultColor = ConfigurationService.Current.HeadshotMarkerColor;

            _headshotKillMarkerColor = markerCategory.GetOrCreateSetting<IColorSelectorGlobalSetting>(
                "HeadshotKillMarkerColor",
                "Headshot Kill Marker Color"
            );
            _headshotKillMarkerColor.TooltipKey = "Color for headshot kill markers";
            _headshotKillMarkerColor.DefaultColor = ConfigurationService.Current.HeadshotKillMarkerColor;
        }

        private void CreateTextStylingSettings(IGlobalModSettingsTab settingsTab)
        {
            var styleCategory = settingsTab.GetOrCreateCategory("TextStyling", "Text Styling");

            _fontName = styleCategory.GetOrCreateSetting<ISelectorGlobalSetting>(
                "FontName",
                "Font Name"
            );
            _fontName.TooltipKey = "Font name for damage text";
            _fontName.SetAllowedValues("Arial", "Times New Roman", "Verdana", "Courier New");
            _fontName.DefaultValue = ConfigurationService.Current.FontName;

            _enableOutline = styleCategory.GetOrCreateSetting<ISwitchGlobalSetting>(
                "EnableOutline",
                "Enable Text Outline"
            );
            _enableOutline.TooltipKey = "Add outline to text for better visibility";
            _enableOutline.SetSwitchValues("false", "true");
            _enableOutline.DefaultValue = ConfigurationService.Current.EnableOutline.ToString().ToLower();

            _outlineColor = styleCategory.GetOrCreateSetting<IColorSelectorGlobalSetting>(
                "OutlineColor",
                "Outline Color"
            );
            _outlineColor.TooltipKey = "Text outline color (usually black for visibility)";
            _outlineColor.DefaultColor = ConfigurationService.Current.OutlineColor;

            _outlineThickness = styleCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "OutlineThickness",
                "Outline Thickness"
            );
            _outlineThickness.TooltipKey = "Thickness of text outline (higher = thicker)";
            _outlineThickness.SetAllowedValues(0.1f, 0.5f, 3f);
            _outlineThickness.DefaultValue = ConfigurationService.Current.OutlineThickness.ToString(CultureInfo.InvariantCulture);
        }

        private void CreateAdvancedSettings(IGlobalModSettingsTab settingsTab)
        {
            var advancedCategory = settingsTab.GetOrCreateCategory("Advanced", "Advanced Settings");

            _playerDamageOnly = advancedCategory.GetOrCreateSetting<ISwitchGlobalSetting>(
                "PlayerDamageOnly",
                "Player Damage Only"
            );
            _playerDamageOnly.TooltipKey = "Only show damage numbers for damage caused by the player";
            _playerDamageOnly.SetSwitchValues("false", "true");
            _playerDamageOnly.DefaultValue = ConfigurationService.Current.PlayerDamageOnly.ToString().ToLower();

            _randomizePosition = advancedCategory.GetOrCreateSetting<ISwitchGlobalSetting>(
                "RandomizePosition",
                "Randomize Position"
            );
            _randomizePosition.TooltipKey = "Slightly randomize text position to prevent overlap";
            _randomizePosition.SetSwitchValues("false", "true");
            _randomizePosition.DefaultValue = ConfigurationService.Current.RandomizePosition.ToString().ToLower();

            _positionRandomness = advancedCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "PositionRandomness",
                "Position Randomness"
            );
            _positionRandomness.TooltipKey = "Amount of position randomization";
            _positionRandomness.SetAllowedValues(0.1f, 0f, 1f);
            _positionRandomness.DefaultValue = ConfigurationService.Current.PositionRandomness.ToString(CultureInfo.InvariantCulture);

            _scaleTextByDamage = advancedCategory.GetOrCreateSetting<ISwitchGlobalSetting>(
                "ScaleTextByDamage",
                "Scale Text by Damage"
            );
            _scaleTextByDamage.TooltipKey = "Make text size vary based on damage amount";
            _scaleTextByDamage.SetSwitchValues("false", "true");
            _scaleTextByDamage.DefaultValue = ConfigurationService.Current.ScaleTextByDamage.ToString().ToLower();

            _minScale = advancedCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "MinScale",
                "Minimum Scale"
            );
            _minScale.TooltipKey = "Minimum text scale multiplier for low damage";
            _minScale.SetAllowedValues(0.1f, 0.3f, 1f);
            _minScale.DefaultValue = ConfigurationService.Current.MinScale.ToString(CultureInfo.InvariantCulture);

            _maxScale = advancedCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "MaxScale",
                "Maximum Scale"
            );
            _maxScale.TooltipKey = "Maximum text scale multiplier for high damage";
            _maxScale.SetAllowedValues(0.1f, 1f, 5f);
            _maxScale.DefaultValue = ConfigurationService.Current.MaxScale.ToString(CultureInfo.InvariantCulture);

            _maxDamageForScale = advancedCategory.GetOrCreateSetting<ISliderGlobalSetting>(
                "MaxDamageForScale",
                "Max Damage for Scale"
            );
            _maxDamageForScale.TooltipKey = "Damage amount that gives maximum text scale";
            _maxDamageForScale.SetAllowedValues(10, 50, 500);
            _maxDamageForScale.DefaultValue = ConfigurationService.Current.MaxDamageForScale.ToString();
        }

        private void SetupAllSettingChangeListeners()
        {
            // Helper to set up event handlers for the settings
            _enableDebugLogging.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.EnableDebugLogging = bool.Parse(newValue);
                AdnLogger.Debug($"Debug logging {(ConfigurationService.Current.EnableDebugLogging ? "enabled" : "disabled")}");
            };

            _minimumDamageThreshold.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MinimumDamageThreshold = int.Parse(newValue);
                AdnLogger.Debug($"Minimum damage threshold changed to {ConfigurationService.Current.MinimumDamageThreshold}");
            };

            _damageNumberCooldown.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.DamageNumberCooldown = float.Parse(newValue);
                AdnLogger.Debug($"Damage cooldown changed to {ConfigurationService.Current.DamageNumberCooldown}s");
            };

            _fontSize.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.FontSize = int.Parse(newValue);
                AdnLogger.Debug($"Font size changed to {ConfigurationService.Current.FontSize}");
            };

            _textLifetime.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.TextLifetime = float.Parse(newValue);
                AdnLogger.Debug($"Text lifetime changed to {ConfigurationService.Current.TextLifetime}s");
            };

            _floatSpeed.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.FloatSpeed = float.Parse(newValue);
                AdnLogger.Debug($"Float speed changed to {ConfigurationService.Current.FloatSpeed}");
            };

            _textOffset.OnSettingChanged += (setting, newValue) =>
            {
                if (TryParseVector3(newValue, out var offset))
                {
                    ConfigurationService.Current.TextOffset = offset;
                    AdnLogger.Debug($"Text offset changed to {offset}");
                }
                else
                {
                    AdnLogger.Error($"Invalid text offset format: {newValue}");
                }
            };

            // Color settings - these use CurrentColor property
            if (_normalDamageColor != null)
                _normalDamageColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.NormalDamageColor = _normalDamageColor.CurrentColor;
                    AdnLogger.Debug("Normal damage color changed");
                };

            if (_headshotDamageColor != null)
                _headshotDamageColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.HeadshotDamageColor = _headshotDamageColor.CurrentColor;
                    AdnLogger.Debug("Headshot damage color changed");
                };

            if (_killDamageColor != null)
                _killDamageColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.KillDamageColor = _killDamageColor.CurrentColor;
                    AdnLogger.Debug("Kill damage color changed");
                };

            if (_headshotKillDamageColor != null)
                _headshotKillDamageColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.HeadshotKillDamageColor = _headshotKillDamageColor.CurrentColor;
                    AdnLogger.Debug("Headshot kill damage color changed");
                };

            // Crosshair marker settings
            _enableCrosshairMarkers.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.EnableCrosshairMarkers = bool.Parse(newValue);
                AdnLogger.Debug($"Crosshair markers {(ConfigurationService.Current.EnableCrosshairMarkers ? "enabled" : "disabled")}");
            };

            _markerDuration.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MarkerDuration = float.Parse(newValue);
                AdnLogger.Debug($"Marker duration changed to {ConfigurationService.Current.MarkerDuration}s");
            };

            _markerFontSize.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MarkerFontSize = int.Parse(newValue);
                AdnLogger.Debug($"Marker font size changed to {ConfigurationService.Current.MarkerFontSize}");
            };

            _normalHitMarker.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.NormalHitMarker = newValue;
                AdnLogger.Debug($"Normal hit marker changed to '{newValue}'");
            };

            _killMarker.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.KillMarker = newValue;
                AdnLogger.Debug($"Kill marker changed to '{newValue}'");
            };

            _headshotMarker.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.HeadshotMarker = newValue;
                AdnLogger.Debug($"Headshot marker changed to '{newValue}'");
            };

            _headshotKillMarker.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.HeadshotKillMarker = newValue;
                AdnLogger.Debug($"Headshot kill marker changed to '{newValue}'");
            };

            // Marker colors
            if (_normalMarkerColor != null)
                _normalMarkerColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.NormalMarkerColor = _normalMarkerColor.CurrentColor;
                    AdnLogger.Debug("Normal marker color changed");
                };

            if (_killMarkerColor != null)
                _killMarkerColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.KillMarkerColor = _killMarkerColor.CurrentColor;
                    AdnLogger.Debug("Kill marker color changed");
                };

            if (_headshotMarkerColor != null)
                _headshotMarkerColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.HeadshotMarkerColor = _headshotMarkerColor.CurrentColor;
                    AdnLogger.Debug("Headshot marker color changed");
                };

            if (_headshotKillMarkerColor != null)
                _headshotKillMarkerColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.HeadshotKillMarkerColor = _headshotKillMarkerColor.CurrentColor;
                    AdnLogger.Debug("Headshot kill marker color changed");
                };

            // Text styling settings
            _fontName.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.FontName = newValue;
                AdnLogger.Debug($"Font name changed to {newValue}");
            };

            _enableOutline.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.EnableOutline = bool.Parse(newValue);
                AdnLogger.Debug($"Text outline {(ConfigurationService.Current.EnableOutline ? "enabled" : "disabled")}");
            };

            if (_outlineColor != null)
                _outlineColor.OnSettingChanged += (setting, newValue) =>
                {
                    ConfigurationService.Current.OutlineColor = _outlineColor.CurrentColor;
                    AdnLogger.Debug("Outline color changed");
                };

            _outlineThickness.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.OutlineThickness = float.Parse(newValue);
                AdnLogger.Debug($"Outline thickness changed to {ConfigurationService.Current.OutlineThickness}");
            };

            // Advanced settings
            _playerDamageOnly.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.PlayerDamageOnly = bool.Parse(newValue);
                AdnLogger.Debug($"Player damage only {(ConfigurationService.Current.PlayerDamageOnly ? "enabled" : "disabled")}");
            };

            _randomizePosition.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.RandomizePosition = bool.Parse(newValue);
                AdnLogger.Debug($"Position randomization {(ConfigurationService.Current.RandomizePosition ? "enabled" : "disabled")}");
            };

            _positionRandomness.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.PositionRandomness = float.Parse(newValue);
                AdnLogger.Debug($"Position randomness changed to {ConfigurationService.Current.PositionRandomness}");
            };

            _scaleTextByDamage.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.ScaleTextByDamage = bool.Parse(newValue);
                AdnLogger.Debug($"Scale text by damage {(ConfigurationService.Current.ScaleTextByDamage ? "enabled" : "disabled")}");
            };

            _minScale.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MinScale = float.Parse(newValue);
                AdnLogger.Debug($"Minimum scale changed to {ConfigurationService.Current.MinScale}");
            };

            _maxScale.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MaxScale = float.Parse(newValue);
                AdnLogger.Debug($"Maximum scale changed to {ConfigurationService.Current.MaxScale}");
            };

            _maxDamageForScale.OnSettingChanged += (setting, newValue) =>
            {
                ConfigurationService.Current.MaxDamageForScale = int.Parse(newValue);
                AdnLogger.Debug($"Max damage for scale changed to {ConfigurationService.Current.MaxDamageForScale}");
            };
        }

        private void ApplyGearsSettingsToConfig()
        {
            // Apply all current Gears settings to our config system
            if (_enableDebugLogging != null)
                ConfigurationService.Current.EnableDebugLogging = bool.Parse(_enableDebugLogging.CurrentValue);

            if (_minimumDamageThreshold != null)
                ConfigurationService.Current.MinimumDamageThreshold = int.Parse(_minimumDamageThreshold.CurrentValue);

            if (_damageNumberCooldown != null)
                ConfigurationService.Current.DamageNumberCooldown = float.Parse(_damageNumberCooldown.CurrentValue);

            if (_fontSize != null)
                ConfigurationService.Current.FontSize = int.Parse(_fontSize.CurrentValue);

            if (_textLifetime != null)
                ConfigurationService.Current.TextLifetime = float.Parse(_textLifetime.CurrentValue);

            if (_floatSpeed != null)
                ConfigurationService.Current.FloatSpeed = float.Parse(_floatSpeed.CurrentValue);

            if (_textOffset != null && TryParseVector3(_textOffset.CurrentValue, out var offset))
                ConfigurationService.Current.TextOffset = offset;

            // Apply color settings
            if (_normalDamageColor != null)
                ConfigurationService.Current.NormalDamageColor = _normalDamageColor.CurrentColor;

            if (_headshotDamageColor != null)
                ConfigurationService.Current.HeadshotDamageColor = _headshotDamageColor.CurrentColor;

            if (_killDamageColor != null)
                ConfigurationService.Current.KillDamageColor = _killDamageColor.CurrentColor;

            if (_headshotKillDamageColor != null)
                ConfigurationService.Current.HeadshotKillDamageColor = _headshotKillDamageColor.CurrentColor;

            // Apply crosshair settings
            if (_enableCrosshairMarkers != null)
                ConfigurationService.Current.EnableCrosshairMarkers = bool.Parse(_enableCrosshairMarkers.CurrentValue);

            if (_markerDuration != null)
                ConfigurationService.Current.MarkerDuration = float.Parse(_markerDuration.CurrentValue);

            if (_markerFontSize != null)
                ConfigurationService.Current.MarkerFontSize = int.Parse(_markerFontSize.CurrentValue);

            if (_normalHitMarker != null)
                ConfigurationService.Current.NormalHitMarker = _normalHitMarker.CurrentValue;

            if (_killMarker != null)
                ConfigurationService.Current.KillMarker = _killMarker.CurrentValue;

            if (_headshotMarker != null)
                ConfigurationService.Current.HeadshotMarker = _headshotMarker.CurrentValue;

            if (_headshotKillMarker != null)
                ConfigurationService.Current.HeadshotKillMarker = _headshotKillMarker.CurrentValue;

            if (_normalMarkerColor != null)
                ConfigurationService.Current.NormalMarkerColor = _normalMarkerColor.CurrentColor;

            if (_killMarkerColor != null)
                ConfigurationService.Current.KillMarkerColor = _killMarkerColor.CurrentColor;

            if (_headshotMarkerColor != null)
                ConfigurationService.Current.HeadshotMarkerColor = _headshotMarkerColor.CurrentColor;

            if (_headshotKillMarkerColor != null)
                ConfigurationService.Current.HeadshotKillMarkerColor = _headshotKillMarkerColor.CurrentColor;

            // Apply text styling settings
            if (_fontName != null)
                ConfigurationService.Current.FontName = _fontName.CurrentValue;

            if (_enableOutline != null)
                ConfigurationService.Current.EnableOutline = bool.Parse(_enableOutline.CurrentValue);

            if (_outlineColor != null)
                ConfigurationService.Current.OutlineColor = _outlineColor.CurrentColor;

            if (_outlineThickness != null)
                ConfigurationService.Current.OutlineThickness = float.Parse(_outlineThickness.CurrentValue);

            // Apply advanced settings
            if (_playerDamageOnly != null)
                ConfigurationService.Current.PlayerDamageOnly = bool.Parse(_playerDamageOnly.CurrentValue);

            if (_randomizePosition != null)
                ConfigurationService.Current.RandomizePosition = bool.Parse(_randomizePosition.CurrentValue);

            if (_positionRandomness != null)
                ConfigurationService.Current.PositionRandomness = float.Parse(_positionRandomness.CurrentValue);

            if (_scaleTextByDamage != null)
                ConfigurationService.Current.ScaleTextByDamage = bool.Parse(_scaleTextByDamage.CurrentValue);

            if (_minScale != null)
                ConfigurationService.Current.MinScale = float.Parse(_minScale.CurrentValue);

            if (_maxScale != null)
                ConfigurationService.Current.MaxScale = float.Parse(_maxScale.CurrentValue);

            if (_maxDamageForScale != null)
                ConfigurationService.Current.MaxDamageForScale = int.Parse(_maxDamageForScale.CurrentValue);
        }

        private bool TryParseVector3(string vectorString, out Vector3 result)
        {
            result = Vector3.zero;
            if (string.IsNullOrEmpty(vectorString)) return false;

            var parts = vectorString.Split(',');
            if (parts.Length == 3 &&
                float.TryParse(parts[0], out var x) &&
                float.TryParse(parts[1], out var y) &&
                float.TryParse(parts[2], out var z))
            {
                result = new Vector3(x, y, z);
                return true;
            }

            return false;
        }
    }
}