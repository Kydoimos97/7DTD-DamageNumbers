using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FDamageText : MonoBehaviour
{
    private float _lifetime;
    private float _floatSpeed;
    private static Font _defaultFont;

    public static void Show(string text, EntityAlive entity, Vector3 localOffset, Color color, int damageAmount = 0)
    {
        FloatingDamageNumbersConfig.DebugLog($"FDamageText.Show called - Text: '{text}', Entity: {entity?.GetType().Name}, Offset: {localOffset}");

        if (_defaultFont == null)
        {
            _defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            FloatingDamageNumbersConfig.DebugLog($"Loaded default font: {(_defaultFont != null ? "SUCCESS" : "FAILED")}");
        }

        GameObject gameObject = new GameObject("DamageText");
        gameObject.transform.SetParent(entity.transform, false);
        gameObject.transform.localPosition = localOffset;
        gameObject.transform.localRotation = Quaternion.identity;

        FloatingDamageNumbersConfig.DebugLog($"Created damage text GameObject at local position: {localOffset}");

        // Setup Canvas
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;
        FloatingDamageNumbersConfig.DebugLog("Canvas created with WorldSpace render mode");

        // Setup RectTransform
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200f, 100f);

        // Calculate scale based on damage if enabled
        float scale = 0.01f;
        if (FloatingDamageNumbersConfig.ScaleTextByDamage && damageAmount > 0)
        {
            float damageRatio = Mathf.Clamp01((float)damageAmount / FloatingDamageNumbersConfig.MaxDamageForScale);
            float scaleMultiplier = Mathf.Lerp(FloatingDamageNumbersConfig.MinScale, FloatingDamageNumbersConfig.MaxScale, damageRatio);
            scale *= scaleMultiplier;
            FloatingDamageNumbersConfig.DebugLog($"Scaled text by damage: {damageAmount} -> scale {scale} (multiplier: {scaleMultiplier})");
        }
        rectTransform.localScale = Vector3.one * scale;

        // Setup CanvasScaler
        CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
        canvasScaler.dynamicPixelsPerUnit = 10f;

        // Setup Text
        Text uiText = gameObject.AddComponent<Text>();
        uiText.text = text;
        uiText.color = color;
        uiText.fontSize = FloatingDamageNumbersConfig.FontSize;
        uiText.font = _defaultFont;
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.horizontalOverflow = HorizontalWrapMode.Overflow;
        uiText.verticalOverflow = VerticalWrapMode.Overflow;

        FloatingDamageNumbersConfig.DebugLog($"Text component setup - Font: {(uiText.font != null ? "OK" : "NULL")}, Size: {uiText.fontSize}, Text: '{uiText.text}'");

        // Start the animation
        FDamageText damageText = gameObject.AddComponent<FDamageText>();
        damageText._lifetime = FloatingDamageNumbersConfig.TextLifetime;
        damageText._floatSpeed = FloatingDamageNumbersConfig.FloatSpeed;
        damageText.StartFade(uiText);

        FloatingDamageNumbersConfig.DebugLog($"Started fade animation - Lifetime: {damageText._lifetime}s, Float speed: {damageText._floatSpeed}");
    }

    private void StartFade(Text uiText)
    {
        StartCoroutine(FadeAndFloat(uiText));
    }

    private IEnumerator FadeAndFloat(Text uiText)
    {
        float elapsed = 0f;
        Color originalColor = uiText.color;
        FloatingDamageNumbersConfig.DebugLog($"Starting fade/float coroutine - Original color: ({originalColor.r:F2}, {originalColor.g:F2}, {originalColor.b:F2}, {originalColor.a:F2})");

        while (elapsed < _lifetime)
        {
            elapsed += Time.deltaTime;

            // Fade out
            float alpha = 1f - (elapsed / _lifetime);
            uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            // Float upward
            transform.localPosition += Vector3.up * (_floatSpeed * Time.deltaTime);

            // Face camera
            Camera camera = FDamageTextUtils.GetCamera();
            if (camera != null)
            {
                Vector3 directionToCamera = transform.position - camera.transform.position;
                transform.rotation = Quaternion.LookRotation(directionToCamera);
            }

            yield return null;
        }

        FloatingDamageNumbersConfig.DebugLog("Damage text animation completed, destroying GameObject");
        Destroy(gameObject);
    }
}