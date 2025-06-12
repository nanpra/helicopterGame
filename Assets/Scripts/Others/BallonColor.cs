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

            // Completely random color
            Color randomColor = new Color(Random.value, Random.value, Random.value);

            // Optional: use same color for emission with boosted intensity
            Color emissionColor = randomColor * 2f;

            mat.color = randomColor;
            mat.SetColor("_EmissionColor", emissionColor);
        }
    }
}
