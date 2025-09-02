using AngelDamageNumbers.Utilities;
using Config;
using UnityEngine;
using UnityEngine.UI;

namespace AngelDamageNumbers.Managers
{
    public class CrosshairManager : MonoBehaviour
    {
        private static CrosshairManager _instance = null!;
        private Text _markerText = null!;
        private float _timer;

        public static CrosshairManager Instance
        {
            get
            {
                if (_instance == null && ConfigurationService.Current.EnableCrosshairMarkers)
                {
                    AdnLogger.Debug("Creating CrosshairManager instance");
                    var gameObject = new GameObject("CrosshairHitmarker");
                    _instance = gameObject.AddComponent<CrosshairManager>();
                    DontDestroyOnLoad(gameObject);
                    _instance.SetupUI();
                    AdnLogger.Debug("CrosshairManager instance created successfully");
                }

                return _instance;
            }
        }

        private void Start()
        {
            SetupUI();
        }

        private void Update()
        {
            if (_markerText != null && _timer > 0f)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    _markerText.color = Color.clear;
                    AdnLogger.Debug("Crosshair marker timer expired, hiding marker");
                }
            }
        }

        private void OnDestroy()
        {
            if (_markerText != null)
            {
                Destroy(_markerText.gameObject);
                _markerText = null;
            }

            if (_instance == this) _instance = null;

            AdnLogger.Debug("CrosshairManager cleaned up");
        }

        private void SetupUI()
        {
            if (_markerText != null)
            {
                AdnLogger.Debug("UI already setup, skipping");
                return;
            }

            AdnLogger.Debug("Setting up CrosshairManager UI");

            // Use shared UIManager utility
            var canvas = UIManager.CreateScreenSpaceCanvas(gameObject);
            AdnLogger.Debug("Canvas created via UIManager");

            // Create text with defaults from ConfigurationService
            var textObject = new GameObject("HitmarkerText");
            textObject.transform.SetParent(canvas.transform, false);

            _markerText = UIManager.CreateStandardText(
                textObject,
                string.Empty, // start with no text
                FontUtils.GetConfiguredFont(), // default font
                ConfigurationService.Current.MarkerFontSize, // size from config
                Color.clear // centered
            );

            // Center and size using UIManager helper
            UIManager.SetupCenteredRectTransform(
                textObject,
                new Vector2(AdnConstants.DefaultTextWidth, AdnConstants.DefaultTextHeight)
            );

            AdnLogger.Debug("CrosshairManager UI setup completed via UIManager");
        }

        public void ShowMarker(string symbol, Color color, float duration = -1f)
        {
            if (!ConfigurationService.Current.EnableCrosshairMarkers)
            {
                AdnLogger.Debug("Crosshair markers disabled, not showing marker");
                return;
            }

            if (_markerText == null)
            {
                AdnLogger.Debug("Marker text not ready, setting up UI");
                SetupUI();
            }

            if (duration < 0f)
                duration = ConfigurationService.Current.MarkerDuration;

            AdnLogger.Debug($"Showing crosshair marker: '{symbol}' with color ({color.r:F2}, {color.g:F2}, {color.b:F2}, {color.a:F2}) for {duration:F2}s");

            _markerText.text = symbol;
            _markerText.color = color;
            _timer = duration;
        }

        public static void CleanupStatics()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
                _instance = null;
            }

            AdnLogger.Debug("CrosshairManager static references cleaned up");
        }
    }
}