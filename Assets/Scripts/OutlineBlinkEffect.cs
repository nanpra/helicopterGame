using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutlineBlinkEffect : MonoBehaviour
{
    public Outline outline;
    public Color blinkColor = Color.red;
    public float blinkSpeed = 2f;

    private Color originalColor;
    private bool blinking = true;


    void Start()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline == null)
        {
            Debug.LogWarning("No Outline component found.");
            enabled = false;
            return;
        }

        originalColor = outline.effectColor;
    }

    void Update()
    {
        if (!blinking) return;

        float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        outline.effectColor = Color.Lerp(originalColor, blinkColor, t);
    }

    public void StartBlinking() => blinking = true;

    public void StopBlinking()
    {
        blinking = false;
        if (outline != null)
        {
            outline.effectColor = originalColor;
        } 
    }
}
