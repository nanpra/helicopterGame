using TMPro;
using UnityEngine;

public class UiPulseEffect : MonoBehaviour
{
    public TextMeshProUGUI dangerText;
    public float blinkSpeed = 2f;         // Speed of alpha blink
    public float scaleSpeed = 2f;         // Speed of pulsing scale
    public float scaleAmount = 1.2f;      // How large it scales
    public bool blinking = true;

    private Color originalColor;
    private Vector3 originalScale;

    void Start()
    {
        if (dangerText == null)
        {
            dangerText = GetComponent<TextMeshProUGUI>();
        }

        originalColor = dangerText.color;
        originalScale = dangerText.transform.localScale;
    }

    void Update()
    {
        if (!blinking) return;

        // Blink alpha (transparency)
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        Color newColor = originalColor;
        newColor.a = alpha;
        dangerText.color = newColor;

        // Pulse scale
        float scale = Mathf.Lerp(1f, scaleAmount, Mathf.PingPong(Time.time * scaleSpeed, 1f));
        dangerText.transform.localScale = originalScale * scale;
    }

    public void StartBlinking() => blinking = true;

    public void StopBlinking()
    {
        blinking = false;
        dangerText.color = originalColor;
        dangerText.transform.localScale = originalScale;
    }
}
