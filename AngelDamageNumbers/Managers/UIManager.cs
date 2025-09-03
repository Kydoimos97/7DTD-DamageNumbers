using AngelDamageNumbers.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AngelDamageNumbers.Managers
{
    public static class UIManager
    {
        public static Canvas CreateWorldSpaceCanvas(GameObject parent)
        {
            var canvas = parent.GetComponent<Canvas>();
            if (canvas == null) canvas = parent.AddComponent<Canvas>();

            canvas.renderMode = RenderMode.WorldSpace;

            // Ensure the parent has a RectTransform for world space canvas
            var rectTransform = parent.GetComponent<RectTransform>();
            if (rectTransform == null)
                // ReSharper disable once RedundantAssignment
                rectTransform = parent.AddComponent<RectTransform>();

            AdnLogger.Debug("World space canvas created successfully");
            return canvas;
        }

        public static Canvas CreateScreenSpaceCanvas(GameObject parent)
        {
            var canvas = parent.GetComponent<Canvas>();
            if (canvas == null) canvas = parent.AddComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Add CanvasScaler for responsive UI
            var canvasScaler = parent.GetComponent<CanvasScaler>();
            if (canvasScaler == null) canvasScaler = parent.AddComponent<CanvasScaler>();

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(AdnConstants.UIScaleReferenceWidth, AdnConstants.UIScaleReferenceHeight);

            // Add GraphicRaycaster for UI interaction
            var graphicRaycaster = parent.GetComponent<GraphicRaycaster>();
            if (graphicRaycaster == null)
                // ReSharper disable once RedundantAssignment
                graphicRaycaster = parent.AddComponent<GraphicRaycaster>();

            AdnLogger.Debug("Screen space overlay canvas created successfully");
            return canvas;
        }

        public static TextMeshProUGUI CreateStandardText(GameObject parent, string text, TMP_FontAsset font, int fontSize, Color color, TextAlignmentOptions alignment = TextAlignmentOptions.Center)
        {
            var textComponent = parent.GetComponent<TextMeshProUGUI>();
            if (textComponent == null) textComponent = parent.AddComponent<TextMeshProUGUI>();

            textComponent.text = text;
            textComponent.font = font ?? FontUtils.GetConfiguredTMPFont();
            textComponent.fontSize = fontSize;
            textComponent.color = color;
            textComponent.alignment = alignment;
            textComponent.enableAutoSizing = false;
            textComponent.overflowMode = TextOverflowModes.Overflow;

            AdnLogger.Debug($"Standard TMP text component created - Text: '{text}', Font: {(textComponent.font != null ? "OK" : "NULL")}, Size: {fontSize}");
            return textComponent;
        }
        private static RectTransform SetupRectTransform(GameObject gameObject, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta, Vector2 anchoredPosition)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform == null) rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.sizeDelta = sizeDelta;
            rectTransform.anchoredPosition = anchoredPosition;

            AdnLogger.Debug($"RectTransform setup - Anchors: ({anchorMin}, {anchorMax}), Pivot: {pivot}, Size: {sizeDelta}, Position: {anchoredPosition}");
            return rectTransform;
        }

        public static RectTransform SetupCenteredRectTransform(GameObject gameObject, Vector2 sizeDelta)
        {
            var center = new Vector2(AdnConstants.PivotCenter, AdnConstants.PivotCenter);
            return SetupRectTransform(gameObject, center, center, center, sizeDelta, Vector2.zero);
        }

        public static Outline AddTextOutline(GameObject textObject, Color outlineColor, float thickness)
        {
            var outline = textObject.GetComponent<Outline>();
            if (outline == null) outline = textObject.AddComponent<Outline>();

            outline.effectColor = outlineColor;
            outline.effectDistance = new Vector2(thickness, thickness);

            AdnLogger.Debug($"Text outline added - Color: {FontUtils.ColorToString(outlineColor)}, Thickness: {thickness}");
            return outline;
        }

        public static Shadow AddTextShadow(GameObject textObject, Color shadowColor, Vector2 shadowDistance)
        {
            var shadow = textObject.GetComponent<Shadow>();
            if (shadow == null) shadow = textObject.AddComponent<Shadow>();

            shadow.effectColor = shadowColor;
            shadow.effectDistance = shadowDistance;

            AdnLogger.Debug($"Text shadow added - Color: {FontUtils.ColorToString(shadowColor)}, Distance: {shadowDistance}");
            return shadow;
        }

        public static bool ValidateUISetup(GameObject uiObject, bool requireCanvas = true, bool requireText = true)
        {
            if (uiObject == null)
            {
                AdnLogger.Error("UI validation failed: GameObject is null");
                return false;
            }

            if (requireCanvas)
            {
                var canvas = uiObject.GetComponent<Canvas>();
                if (canvas == null)
                {
                    AdnLogger.Error($"UI validation failed: {uiObject.name} missing Canvas component");
                    return false;
                }
            }

            if (requireText)
            {
                var text = uiObject.GetComponent<Text>();
                if (text == null)
                {
                    AdnLogger.Error($"UI validation failed: {uiObject.name} missing Text component");
                    return false;
                }

                if (text.font == null) AdnLogger.Warning($"UI validation warning: {uiObject.name} Text component has null font");
            }

            var rectTransform = uiObject.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                AdnLogger.Error($"UI validation failed: {uiObject.name} missing RectTransform component");
                return false;
            }

            AdnLogger.Debug($"UI validation passed for {uiObject.name}");
            return true;
        }
    }
}