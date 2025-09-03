using System;
using System.Linq;
using AngelDamageNumbers.Config;
using AngelDamageNumbers.Utilities;
using GearsAPI.Settings.Global;

namespace AngelDamageNumbers.Gears;

public class GearsSettings
{
    public ISliderGlobalSetting DamageNumberCooldown = null!;
    public ISwitchGlobalSetting EnableCrosshairMarkers = null!;
    public ISwitchGlobalSetting EnableDebugLogging = null!;
    public ISwitchGlobalSetting EnableOutline = null!;
    public ISliderGlobalSetting FloatSpeed = null!;
    public ISelectorGlobalSetting FontName = null!;
    public ISliderGlobalSetting FontSize = null!;
    public IColorSelectorGlobalSetting NormalDamageColor = null!;
    public ISelectorGlobalSetting NormalHitMarker = null!;
    public IColorSelectorGlobalSetting NormalMarkerColor = null!;
    public IColorSelectorGlobalSetting OutlineColor = null!;
    public ISliderGlobalSetting OutlineThickness = null!;
    public ISwitchGlobalSetting PlayerDamageOnly = null!;
    public ISliderGlobalSetting PositionRandomness = null!;
    public ISwitchGlobalSetting RandomizePosition = null!;
    public ISwitchGlobalSetting ScaleTextByDamage = null!;
    public ISliderGlobalSetting TextLifetime = null!;
    public IColorSelectorGlobalSetting HeadshotDamageColor = null!;
    public IColorSelectorGlobalSetting HeadshotKillDamageColor = null!;
    public ISelectorGlobalSetting HeadshotKillMarker = null!;
    public IColorSelectorGlobalSetting HeadshotKillMarkerColor = null!;
    public ISelectorGlobalSetting HeadshotMarker = null!;
    public IColorSelectorGlobalSetting HeadshotMarkerColor = null!;
    public IColorSelectorGlobalSetting KillDamageColor = null!;
    public ISelectorGlobalSetting KillMarker = null!;
    public IColorSelectorGlobalSetting KillMarkerColor = null!;
    public ISliderGlobalSetting MarkerDuration = null!;
    public ISliderGlobalSetting MarkerFontSize = null!;
    public ISliderGlobalSetting MaxDamageForScale = null!;
    public ISliderGlobalSetting MaxScale = null!;
    public ISliderGlobalSetting MinimumDamageThreshold = null!;
    public ISliderGlobalSetting MinScale = null!;

    private IGlobalModSettingsTab _fontSettings;
    private IGlobalModSettingsTab _numberSettingsTab;
    private IGlobalModSettingsTab _crosshairSettingsTab;
    private IGlobalModSettingsTab _advancedTab;

    public GearsSettings(IModGlobalSettings settingsInstance)
    {
        AdnLogger.Debug("[Gears] requesting tab...");
        CreateTabs(settingsInstance);

        if (_fontSettings == null)
        {
            AdnLogger.Error("[Gears] FontSettingsTab returned as NULL!");
            throw new ArgumentNullException(nameof(_fontSettings));
        }
        if (_numberSettingsTab == null)
        {
            AdnLogger.Error("[Gears] NumberSettingsTab returned as NULL!");
            throw new ArgumentNullException(nameof(_numberSettingsTab));
        }
        if (_crosshairSettingsTab == null)
        {
            AdnLogger.Error("[Gears] CrossHairTab returned as NULL!");
            throw new ArgumentNullException(nameof(_crosshairSettingsTab));
        }
        if (_advancedTab == null)
        {
            AdnLogger.Error("[Gears] AdvancedTab returned as NULL!");
            throw new ArgumentNullException(nameof(_advancedTab));
        }

        CreateNumberBehaviorSettings(_numberSettingsTab);
        CreateNumberScaleSettings(_numberSettingsTab);
        CreateNumberColorSettings(_numberSettingsTab);

        CreateCrosshairStyleSettings(_crosshairSettingsTab);
        CreateCrosshairBehaviorSettings(_crosshairSettingsTab);
        CreateCrosshairColorSettings(_crosshairSettingsTab);

        CreateFontSettings(_fontSettings);

        CreateAdvancedDebugSettings(_advancedTab);
        CreateAdvancedAdvancedSettings(_advancedTab);
    }

    private void CreateTabs(IModGlobalSettings settingsInstance)
    {
        _numberSettingsTab   = settingsInstance.CreateTab("Numbers", "Number");
        _crosshairSettingsTab = settingsInstance.CreateTab("Crosshair", "Crosshair");
        _fontSettings        = settingsInstance.CreateTab("Font", "Font");
        _advancedTab         = settingsInstance.CreateTab("Advanced", "Advanced");
    }

    private void CreateFontSettings(IGlobalModSettingsTab settingsTab)
    {
        var fontCategory = settingsTab.GetOrCreateCategory("Font", "Font Settings");
        var choices = FontUtils.FontMap.Keys.ToArray();
        FontName = GearsHelper.CreateSelectorSetting(
            fontCategory,
            "FontName", "Font Name",
            "Font used for floating damage numbers",
             choices,
            ConfigurationService.Current.FontName
        );

        EnableOutline = GearsHelper.CreateSwitchSetting(
            fontCategory,
            "EnableOutline", "Enable Text Outline",
            "Add outline to text for better visibility",
            "false", "true",
            ConfigurationService.Current.EnableOutline.ToString().ToLower());

        OutlineColor = GearsHelper.CreateColorSetting(
            fontCategory,
            "OutlineColor", "Outline Color",
            "Text outline color (usually black for visibility)",
            ConfigurationService.Current.OutlineColor);

        OutlineThickness = GearsHelper.CreateSliderSetting(
            fontCategory,
            "OutlineThickness", "Outline Thickness",
            "Thickness of text outline (higher = thicker)",
            0.1f, 0.1f, 1.5f,
            ConfigurationService.Current.OutlineThickness);
    }

    private void CreateNumberColorSettings(IGlobalModSettingsTab settingsTab)
    {
        var colorCategory = settingsTab.GetOrCreateCategory("Color", "Damage Number Colors");

        NormalDamageColor = GearsHelper.CreateColorSetting(
            colorCategory,
            "NormalDamageColor", "Normal Damage Color",
            "Color for normal damage numbers",
            ConfigurationService.Current.NormalDamageColor);

        HeadshotDamageColor = GearsHelper.CreateColorSetting(
            colorCategory,
            "HeadshotDamageColor", "Headshot Damage Color",
            "Color for headshot damage numbers",
            ConfigurationService.Current.HeadshotDamageColor);

        KillDamageColor = GearsHelper.CreateColorSetting(
            colorCategory,
            "KillDamageColor", "Kill Damage Color",
            "Color for killing blow damage numbers",
            ConfigurationService.Current.KillDamageColor);

        HeadshotKillDamageColor = GearsHelper.CreateColorSetting(
            colorCategory,
            "HeadshotKillDamageColor", "Headshot Kill Damage Color",
            "Color for headshot kill damage numbers",
            ConfigurationService.Current.HeadshotKillDamageColor);
    }

    private void CreateNumberBehaviorSettings(IGlobalModSettingsTab settingsTab)
    {
        var behaviorCategory = settingsTab.GetOrCreateCategory("Behavior", "Damage Number Behavior");

        MinimumDamageThreshold = GearsHelper.CreateSliderSetting(
            behaviorCategory,
            "MinimumDamageThreshold", "Minimum Damage Threshold",
            "Minimum damage required to show floating numbers (0 = show all damage)",
            1f, 0f, 1000f,
            ConfigurationService.Current.MinimumDamageThreshold);

        DamageNumberCooldown = GearsHelper.CreateSliderSetting(
            behaviorCategory,
            "DamageNumberCooldown", "Damage Number Cooldown",
            "Minimum time between damage numbers in seconds (prevents spam)",
            0.1f, 0f, 10f,
            ConfigurationService.Current.DamageNumberCooldown);

        FontSize = GearsHelper.CreateSliderSetting(
            behaviorCategory,
            "FontSize", "Font Size",
            "Size of the damage number text",
            1f, 8f, 48f,
            ConfigurationService.Current.FontSize);

        TextLifetime = GearsHelper.CreateSliderSetting(
            behaviorCategory,
            "TextLifetime", "Text Lifetime",
            "How long the text is visible in seconds",
            0.1f, 0.1f, 5f,
            ConfigurationService.Current.TextLifetime);

        FloatSpeed = GearsHelper.CreateSliderSetting(
            behaviorCategory,
            "FloatSpeed", "Float Speed",
            "Speed at which text floats upward",
            0.1f, 0.1f, 3f,
            ConfigurationService.Current.FloatSpeed);
    }

    private void CreateNumberScaleSettings(IGlobalModSettingsTab settingsTab)
    {
        var otherCategory = settingsTab.GetOrCreateCategory("Scale", "Number Scale Settings");

        ScaleTextByDamage = GearsHelper.CreateSwitchSetting(
            otherCategory,
            "ScaleTextByDamage", "Scale Text by Damage",
            "Make text size vary based on damage amount",
            "false", "true",
            ConfigurationService.Current.ScaleTextByDamage.ToString().ToLower());

        MinScale = GearsHelper.CreateSliderSetting(
            otherCategory,
            "MinScale", "Minimum Scale",
            "Minimum text scale multiplier for low damage",
            0.1f, 0.1f, 1f,
            ConfigurationService.Current.MinScale);

        MaxScale = GearsHelper.CreateSliderSetting(
            otherCategory,
            "MaxScale", "Maximum Scale",
            "Maximum text scale multiplier for high damage",
            0.1f, 1f, 5f,
            ConfigurationService.Current.MaxScale);

        MaxDamageForScale = GearsHelper.CreateSliderSetting(
            otherCategory,
            "MaxDamageForScale", "Max Damage for Scale",
            "Damage amount that gives maximum text scale",
            10f, 10f, 5000f,
            ConfigurationService.Current.MaxDamageForScale);
    }

    private void CreateCrosshairColorSettings(IGlobalModSettingsTab settingsTab)
    {
        var colorCategory = settingsTab.GetOrCreateCategory("Color", "Crosshair Marker Colors");

        NormalMarkerColor = GearsHelper.CreateColorSetting(
            colorCategory,
            "NormalMarkerColor", "Normal Marker Color",
            "Color for normal hit markers",
            ConfigurationService.Current.NormalMarkerColor);

        KillMarkerColor = GearsHelper.CreateColorSetting(
            colorCategory,
            "KillMarkerColor", "Kill Marker Color",
            "Color for kill markers",
            ConfigurationService.Current.KillMarkerColor);

        HeadshotMarkerColor = GearsHelper.CreateColorSetting(
            colorCategory,
            "HeadshotMarkerColor", "Headshot Marker Color",
            "Color for headshot markers",
            ConfigurationService.Current.HeadshotMarkerColor);

        HeadshotKillMarkerColor = GearsHelper.CreateColorSetting(
            colorCategory,
            "HeadshotKillMarkerColor", "Headshot Kill Marker Color",
            "Color for headshot kill markers",
            ConfigurationService.Current.HeadshotKillMarkerColor);
    }

    private void CreateCrosshairBehaviorSettings(IGlobalModSettingsTab settingsTab)
    {
        var behaviorCategory = settingsTab.GetOrCreateCategory("Behavior", "Crosshair Marker Behavior");

        EnableCrosshairMarkers = GearsHelper.CreateSwitchSetting(
            behaviorCategory,
            "EnableCrosshairMarkers", "Enable Crosshair Markers",
            "Show hit markers on your crosshair when hitting enemies",
            "false", "true",
            ConfigurationService.Current.EnableCrosshairMarkers.ToString().ToLower());

        MarkerDuration = GearsHelper.CreateSliderSetting(
            behaviorCategory,
            "MarkerDuration", "Marker Duration",
            "How long crosshair markers are visible in seconds",
            0.1f, 0.1f, 2f,
            ConfigurationService.Current.MarkerDuration);

        MarkerFontSize = GearsHelper.CreateSliderSetting(
            behaviorCategory,
            "MarkerFontSize", "Marker Font Size",
            "Size of crosshair marker symbols",
            1f, 12f, 60f,
            ConfigurationService.Current.MarkerFontSize);
    }

    private void CreateCrosshairStyleSettings(IGlobalModSettingsTab settingsTab)
    {
        var otherCategory = settingsTab.GetOrCreateCategory("Style", "Crosshair Style Settings");

        var allowedSymbols = new[] { "x", "X", "×", "✖", "⚡", "●", "◉", "⊙", "➤", "♦", "✓", "★", "☠" };


        NormalHitMarker = GearsHelper.CreateSelectorSetting(
            otherCategory,
            "NormalHitMarker", "Normal Hit Marker",
            "Symbol for normal hits",
            allowedSymbols,
            ConfigurationService.Current.NormalHitMarker);

        KillMarker = GearsHelper.CreateSelectorSetting(
            otherCategory,
            "KillMarker", "Kill Marker",
            "Symbol for kills",
            allowedSymbols,
            ConfigurationService.Current.KillMarker);

        HeadshotMarker = GearsHelper.CreateSelectorSetting(
            otherCategory,
            "HeadshotMarker", "Headshot Marker",
            "Symbol for headshots",
            allowedSymbols,
            ConfigurationService.Current.HeadshotMarker);

        HeadshotKillMarker = GearsHelper.CreateSelectorSetting(
            otherCategory,
            "HeadshotKillMarker", "Headshot Kill Marker",
            "Symbol for headshot kills",
            allowedSymbols,
            ConfigurationService.Current.HeadshotKillMarker);
    }

    private void CreateAdvancedDebugSettings(IGlobalModSettingsTab settingsTab)
    {
        var debugCategory = settingsTab.GetOrCreateCategory("Debug", "Debug Settings");

        EnableDebugLogging = GearsHelper.CreateSwitchSetting(
            debugCategory,
            "EnableDebugLogging", "Enable Debug Logging",
            "Enable debug messages in Unity console - useful for troubleshooting issues",
            "false", "true",
            ConfigurationService.Current.EnableDebugLogging.ToString().ToLower());
    }

    private void CreateAdvancedAdvancedSettings(IGlobalModSettingsTab settingsTab)
    {
        var advancedCategory = settingsTab.GetOrCreateCategory("Advanced", "Advanced Settings");

        PlayerDamageOnly = GearsHelper.CreateSwitchSetting(
            advancedCategory,
            "PlayerDamageOnly", "Player Damage Only",
            "Only show damage numbers for damage caused by the player",
            "false", "true",
            ConfigurationService.Current.PlayerDamageOnly.ToString().ToLower());

        RandomizePosition = GearsHelper.CreateSwitchSetting(
            advancedCategory,
            "RandomizePosition", "Randomize Position",
            "Slightly randomize text position to prevent overlap",
            "false", "true",
            ConfigurationService.Current.RandomizePosition.ToString().ToLower());

        PositionRandomness = GearsHelper.CreateSliderSetting(
            advancedCategory,
            "PositionRandomness", "Position Randomness",
            "Amount of position randomization",
            0.1f, 0f, 1f,
            ConfigurationService.Current.PositionRandomness);
    }
}