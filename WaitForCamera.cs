using System.Collections;
using UnityEngine;

public class WaitForCamera : MonoBehaviour
{
    public Canvas targetCanvas;
    private const float RetryInterval = 0.2f;
    private const float Timeout = 5f;

    private void Start()
    {
        Debug.Log("[Angel-DamageNumbers] WaitForCamera starting camera detection");
        StartCoroutine(WaitAndAssignCamera());
    }

    private IEnumerator WaitAndAssignCamera()
    {
        float elapsed = 0f;

        while (Camera.main == null && elapsed < Timeout)
        {
            Debug.Log("[Angel-DamageNumbers] Waiting for Camera.main...");
            yield return new WaitForSeconds(RetryInterval);
            elapsed += RetryInterval;
        }

        if (Camera.main != null && targetCanvas != null)
        {
            targetCanvas.worldCamera = Camera.main;
            Debug.Log("[Angel-DamageNumbers] Successfully assigned Camera.main to targetCanvas");
        }
        else
        {
            Debug.LogError("[Angel-DamageNumbers] Failed to assign camera - Camera or Canvas is null");
        }

        Debug.Log("[Angel-DamageNumbers] WaitForCamera component complete, destroying self");
        Destroy(this);
    }
}