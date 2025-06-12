using UnityEngine;

public class BallonColor : MonoBehaviour
{
    private MeshRenderer balloonRenderer;

    void Start()
    {
        if (balloonRenderer == null)
            balloonRenderer = GetComponent<MeshRenderer>();

        if (balloonRenderer != null)
        {
            Material mat = balloonRenderer.material;
            mat.EnableKeyword("_EMISSION");

            // Ensure each channel is in a high range for brightness
            float minBrightValue = 0.7f; // Minimum brightness for each color component
            Color brightColor = new Color(Random.Range(minBrightValue, 1f),
                                          Random.Range(minBrightValue, 1f),
                                          Random.Range(minBrightValue, 1f));

            Color boostedEmission = brightColor * 2f; // Enhance emission intensity

            mat.color = brightColor; // Optional: base color
            mat.SetColor("_EmissionColor", boostedEmission);
        }
    }

}
