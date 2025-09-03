using System;
using AngelDamageNumbers.Config;
using AngelDamageNumbers.Managers;
using AngelDamageNumbers.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AngelDamageNumbers.UI
{
    public static class DamageTextFactory
    {
        public static GameObject? CreateDamageText(string text, EntityAlive entity, Vector3 localOffset, Color color, int damageAmount = 0)
    {
        AdnLogger.Debug($"DamageTextFactory.CreateDamageText called - Text: '{text}', Entity: {entity.GetType().Name}, Offset: {localOffset}");

        if (entity == null)
        {
            AdnLogger.Error("Cannot create damage text: entity is null");
            return null;
        }

        var gameObject = new GameObject("DamageText");
        gameObject.transform.SetParent(entity.transform, false);
        gameObject.transform.localPosition = localOffset;
        gameObject.transform.localRotation = Quaternion.identity;

        AdnLogger.Debug($"Created damage text GameObject at local position: {localOffset}");

        // Setup Canvas using UIManager
        var canvas = UIManager.CreateWorldSpaceCanvas(gameObject);
        canvas.overrideSorting = true;
        canvas.sortingOrder = AdnConstants.CanvasSortOrder;
        AdnLogger.Debug("Canvas created with WorldSpace render mode");

        // Setup RectTransform
        var rectTransform = gameObject.GetComponent<RectTransform>();
        if (rectTransform == null) rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(AdnConstants.DefaultTextWidth, AdnConstants.DefaultTextHeight);

        // Calculate scale based on damage if enabled
        var scale = AdnConstants.TextScaleModifier;
        if (ConfigurationService.Current.ScaleTextByDamage && damageAmount > 0)
        {
            var damageRatio = Mathf.Clamp01((float)damageAmount / ConfigurationService.Current.MaxDamageForScale);
            var scaleMultiplier = Mathf.Lerp(ConfigurationService.Current.MinScale, ConfigurationService.Current.MaxScale, damageRatio);
            scale *= scaleMultiplier;
            AdnLogger.Debug($"Scaled text by damage: {damageAmount} -> scale {scale} (multiplier: {scaleMultiplier})");
        }

        rectTransform.localScale = Vector3.one * scale;

        // Setup CanvasScaler
        var canvasScaler = gameObject.AddComponent<CanvasScaler>();
        canvasScaler.dynamicPixelsPerUnit = AdnConstants.CanvasPixelsPerUnit;

        // Setup TextMeshPro
        var tmpText = gameObject.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.color = color;
        tmpText.fontSize = ConfigurationService.Current.FontSize;
        tmpText.font = FontUtils.GetConfiguredTMPFont();
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.enableAutoSizing = false;
        tmpText.overflowMode = TextOverflowModes.Overflow;

        AdnLogger.Debug($"TextMeshPro component setup - Font: {(tmpText.font != null ? "OK" : "NULL")}, Size: {tmpText.fontSize}, Text: '{tmpText.text}'");

        // Add outline using TextMeshPro's built-in system
        if (ConfigurationService.Current.EnableOutline
            && ConfigurationService.Current.OutlineThickness > 0f
            // ReSharper disable once ConvertTypeCheckPatternToNullCheck
            && tmpText.font is TMP_FontAsset font)
        {
            tmpText.fontMaterial = FontUtils.GetOutlineMaterial(
                font,
                ConfigurationService.Current.OutlineColor,
                ConfigurationService.Current.OutlineThickness,
                0.1f);
        }

        // Add the animation component
        var animationComponent = gameObject.AddComponent<AdnDamageText>();
        if (animationComponent == null)
        {
            AdnLogger.Error("Failed to add AdnDamageText component");
            Object.Destroy(gameObject);
            return null;
        }

        AdnLogger.Debug("Damage text GameObject created successfully");
        return gameObject;
    }

        public static GameObject CreateMarkerText(string text, Transform parent, Color color, int fontSize)
        {
            var textObject = new GameObject("MarkerText");
            textObject.transform.SetParent(parent, false);

            var markerText = textObject.AddComponent<TextMeshProUGUI>();
            markerText.color = color;
            markerText.font = FontUtils.GetMarkerFontForSymbol(text, ConfigurationService.Current.FontName);
            markerText.text = FontUtils.GetSafeChar(text, markerText.font);
            markerText.fontSize = fontSize;
            markerText.alignment = TextAlignmentOptions.Center;
            markerText.enableAutoSizing = false;
            markerText.overflowMode = TextOverflowModes.Overflow;

            // Setup RectTransform for marker
            var rectTransform = markerText.rectTransform;
            rectTransform.pivot = new Vector2(AdnConstants.PivotCenter, AdnConstants.PivotCenter);
            rectTransform.anchorMin = new Vector2(AdnConstants.PivotCenter, AdnConstants.PivotCenter);
            rectTransform.anchorMax = new Vector2(AdnConstants.PivotCenter, AdnConstants.PivotCenter);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(AdnConstants.MarkerRectWidth, AdnConstants.MarkerRectHeight);

            AdnLogger.Debug($"Created marker text: '{text}' with color {FontUtils.ColorToString(color)}");
            return textObject;
        }
    }
}