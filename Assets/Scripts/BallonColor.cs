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

            Color randomColor = new Color(Random.value, Random.value, Random.value);
            Color brightColor = randomColor * 2f; // Boost emission intensity

            mat.SetColor("_EmissionColor", brightColor);
        }
    }
}
