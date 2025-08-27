using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    private static CrosshairManager _instance;
    private Text _markerText;
    private float _timer;

    public static CrosshairManager Instance
    {
        get
        {
            if (_instance == null && FloatingDamageNumbersConfig.EnableCrosshairMarkers)
            {
                FloatingDamageNumbersConfig.DebugLog("Creating CrosshairManager instance");
                GameObject gameObject = new GameObject("CrosshairHitmarker");
                _instance = gameObject.AddComponent<CrosshairManager>();
                DontDestroyOnLoad(gameObject);
                _instance.SetupUI();
                FloatingDamageNumbersConfig.DebugLog("CrosshairManager instance created successfully");
            }
            return _instance;
        }
    }

    private void Start()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        if (_markerText != null)
        {
            FloatingDamageNumbersConfig.DebugLog("UI already setup, skipping");
            return;
        }

        FloatingDamageNumbersConfig.DebugLog("Setting up CrosshairManager UI");

        // Setup Canvas
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);

        gameObject.AddComponent<GraphicRaycaster>();
        FloatingDamageNumbersConfig.DebugLog("Canvas components added");

        // Setup Text
        GameObject textObject = new GameObject("HitmarkerText");
        textObject.transform.SetParent(canvas.transform, false);

        _markerText = textObject.AddComponent<Text>();
        _markerText.alignment = TextAnchor.MiddleCenter;
        _markerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        _markerText.fontSize = FloatingDamageNumbersConfig.MarkerFontSize;
        _markerText.color = Color.clear;

        FloatingDamageNumbersConfig.DebugLog($"Text component setup - Font: {(_markerText.font != null ? "OK" : "NULL")}, Size: {_markerText.fontSize}");

        // Setup RectTransform
        RectTransform rectTransform = _markerText.rectTransform;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(200f, 100f);

        FloatingDamageNumbersConfig.DebugLog("CrosshairManager UI setup completed");
    }

    private void Update()
    {
        if (_markerText != null && _timer > 0f)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _markerText.color = Color.clear;
                FloatingDamageNumbersConfig.DebugLog("Crosshair marker timer expired, hiding marker");
            }
        }
    }

    public void ShowMarker(string symbol, Color color, float duration = -1f)
    {
        if (!FloatingDamageNumbersConfig.EnableCrosshairMarkers)
        {
            FloatingDamageNumbersConfig.DebugLog("Crosshair markers disabled, not showing marker");
            return;
        }

        if (_markerText == null)
        {
            FloatingDamageNumbersConfig.DebugLog("Marker text not ready, setting up UI");
            SetupUI();
        }

        if (duration < 0f)
            duration = FloatingDamageNumbersConfig.MarkerDuration;

        FloatingDamageNumbersConfig.DebugLog($"Showing crosshair marker: '{symbol}' with color ({color.r:F2}, {color.g:F2}, {color.b:F2}, {color.a:F2}) for {duration:F2}s");

        _markerText.text = symbol;
        _markerText.color = color;
        _timer = duration;
    }
}