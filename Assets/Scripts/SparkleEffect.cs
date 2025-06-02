using UnityEngine;
using UnityEngine.UI;

public class SparkleEffect : MonoBehaviour
{
    public RectTransform uiTarget;
    public float speed = 5f;
    public ParticleSystem onArrivalEffect;

    private Vector3 targetWorldPos;
    private bool initialized = false;

    public void Initialize(RectTransform targetUI)
    {
        uiTarget = targetUI;
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, uiTarget.position);
        targetWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane + 1f));
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWorldPos) < 0.1f)
        {
            if (onArrivalEffect != null)
                Instantiate(onArrivalEffect, targetWorldPos, Quaternion.identity, uiTarget);

            Destroy(gameObject);
        }
    }
}
