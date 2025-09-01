using Config;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public static class DamageTextFactory
    {
        public static GameObject CreateDamageText(string text, EntityAlive entity, Vector3 localOffset, Color color, int damageAmount = 0)
        {
            AdnLogger.Debug($"DamageTextFactory.CreateDamageText called - Text: '{text}', Entity: {entity?.GetType().Name}, Offset: {localOffset}");

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

            // Setup Text
            var uiText = gameObject.AddComponent<Text>();
            uiText.text = text;
            uiText.color = color;
            uiText.fontSize = ConfigurationService.Current.FontSize;
            uiText.font = FontUtils.GetConfiguredFont();
            uiText.alignment = TextAnchor.MiddleCenter;
            uiText.horizontalOverflow = HorizontalWrapMode.Overflow;
            uiText.verticalOverflow = VerticalWrapMode.Overflow;

            AdnLogger.Debug($"Text component setup - Font: {(uiText.font != null ? "OK" : "NULL")}, Size: {uiText.fontSize}, Text: '{uiText.text}'");

            // Add outline if enabled
            if (ConfigurationService.Current.EnableOutline)
            {
                var outline = gameObject.AddComponent<Outline>();
                outline.effectColor = ConfigurationService.Current.OutlineColor;
                outline.effectDistance = new Vector2(ConfigurationService.Current.OutlineThickness, ConfigurationService.Current.OutlineThickness);
                AdnLogger.Debug($"Added outline - Color: {FontUtils.ColorToString(outline.effectColor)}, Thickness: {ConfigurationService.Current.OutlineThickness}");
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

            var markerText = textObject.AddComponent<Text>();
            markerText.text = text;
            markerText.color = color;
            markerText.font = FontUtils.GetConfiguredFont();
            markerText.fontSize = fontSize;
            markerText.alignment = TextAnchor.MiddleCenter;

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