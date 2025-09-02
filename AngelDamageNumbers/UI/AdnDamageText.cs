using System.Collections;
using AngelDamageNumbers.Utilities;
using Config;
using UnityEngine;
using UnityEngine.UI;

namespace AngelDamageNumbers.UI
{
    public class AdnDamageText : MonoBehaviour
    {
        private float _floatSpeed;
        private float _lifetime;

        public static void CleanupStatics()
        {
            AdnLogger.Debug("AdnDamageText static references cleaned up");
        }

        public static void Show(string text, EntityAlive entity, Vector3 localOffset, Color color, int damageAmount = 0)
        {
            var damageTextObj = DamageTextFactory.CreateDamageText(text, entity, localOffset, color, damageAmount);
            if (damageTextObj == null)
            {
                AdnLogger.Error("Failed to create damage text GameObject");
                return;
            }

            var damageText = damageTextObj.GetComponent<AdnDamageText>();
            if (damageText == null)
            {
                AdnLogger.Error("AdnDamageText component not found on created GameObject");
                Destroy(damageTextObj);
                return;
            }

            var uiText = damageTextObj.GetComponent<Text>();
            if (uiText == null)
            {
                AdnLogger.Error("Text component not found on created GameObject");
                Destroy(damageTextObj);
                return;
            }

            damageText._lifetime = ConfigurationService.Current.TextLifetime;
            damageText._floatSpeed = ConfigurationService.Current.FloatSpeed;
            damageText.StartFade(uiText);

            AdnLogger.Debug($"Damage text animation started - Lifetime: {damageText._lifetime}s, Float speed: {damageText._floatSpeed}");
        }

        private void StartFade(Text uiText)
        {
            StartCoroutine(FadeAndFloat(uiText));
        }

        private IEnumerator FadeAndFloat(Text uiText)
        {
            var elapsed = 0f;
            var originalColor = uiText.color;
            AdnLogger.Debug($"Starting fade/float coroutine - Original color: ({originalColor.r:F2}, {originalColor.g:F2}, {originalColor.b:F2}, {originalColor.a:F2})");

            while (elapsed < _lifetime)
            {
                elapsed += Time.deltaTime;

                // Fade out
                var alpha = 1f - elapsed / _lifetime;
                uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                // Float upward
                transform.localPosition += Vector3.up * (_floatSpeed * Time.deltaTime);

                // Face camera
                var camera = CameraUtils.GetBestCamera();
                if (camera != null)
                {
                    var directionToCamera = transform.position - camera.transform.position;
                    transform.rotation = Quaternion.LookRotation(directionToCamera);
                }

                yield return null;
            }

            AdnLogger.Debug("Damage text animation completed, destroying GameObject");
            Destroy(gameObject);
        }
    }
}