using System;
using System.Globalization;
using AngelDamageNumbers.Utilities;
using Gears.SettingsManager.Settings;
using GearsAPI.Settings.Global;
using UnityEngine;

namespace AngelDamageNumbers.Gears;

public static class GearsHelper
{
    // ---- Internals ---------------------------------------------------------

    private static T Require<T>(T value, string error) where T : class
    {
        if (value == null)
        {
            AdnLogger.Error(error);
            throw new NullReferenceException(error);
        }
        return value;
    }

    private static IGlobalModSettingsCategory RequireCategory(IGlobalModSettingsCategory category, string where)
        => Require(category, $"[Gears] Category is null in {where}.");

    // ---- Color -------------------------------------------------------------

    public static IColorSelectorGlobalSetting CreateColorSetting(
        IGlobalModSettingsCategory category,
        string key,
        string display,
        string tooltip,
        Color defaultColor)
    {
        var cat = RequireCategory(category, nameof(CreateColorSetting));

        // ColorSelectorSetting isn’t reliably created via the generic path.
        var setting = new ColorSelectorSetting(key, display)
        {
            TooltipKey = tooltip,
            DefaultColor = defaultColor
        };

        cat.AddSetting(setting);
        return setting;
    }

    public static ISliderGlobalSetting CreateSliderSetting(
        IGlobalModSettingsCategory category,
        string key,
        string display,
        string tooltip,
        float increment,
        float min,
        float max,
        float defaultValue)
    {
        var cat = RequireCategory(category, nameof(CreateSliderSetting));

        var setting = Require(
            cat.CreateSetting<ISliderGlobalSetting>(name: key, displayKey: display),
            $"[Gears] CreateSetting<ISliderGlobalSetting>(\"{key}\") returned null.");

        setting.TooltipKey = tooltip;
        setting.SetAllowedValues(increment, min, max);
        setting.DefaultValue = defaultValue.ToString(CultureInfo.InvariantCulture);

        return setting;
    }

    public static ISwitchGlobalSetting CreateSwitchSetting(
        IGlobalModSettingsCategory category,
        string key,
        string display,
        string tooltip,
        string leftValue,
        string rightValue,
        string defaultValue)
    {
        var cat = RequireCategory(category, nameof(CreateSwitchSetting));

        var setting = Require(
            cat.CreateSetting<ISwitchGlobalSetting>(name: key, displayKey: display),
            $"[Gears] CreateSetting<ISwitchGlobalSetting>(\"{key}\") returned null.");

        setting.TooltipKey = tooltip;
        setting.SetSwitchValues(leftValue, rightValue);
        // defaultValue is already a string ("true"/"false" etc.); don’t .ToString().ToLower() it again.
        setting.DefaultValue = defaultValue;

        return setting;
    }

    public static ISelectorGlobalSetting CreateSelectorSetting(
        IGlobalModSettingsCategory category,
        string key,
        string display,
        string tooltip,
        string[] allowedValues,
        string defaultValue)
    {
        var cat = RequireCategory(category, nameof(CreateSelectorSetting));

        var setting = Require(
            cat.CreateSetting<ISelectorGlobalSetting>(name: key, displayKey: display),
            $"[Gears] CreateSetting<ISelectorGlobalSetting>(\"{key}\") returned null.");

        setting.TooltipKey = tooltip;
        setting.SetAllowedValues(allowedValues);
        setting.DefaultValue = defaultValue;

        return setting;
    }
}
