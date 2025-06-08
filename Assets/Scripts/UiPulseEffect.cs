using UnityEngine;

public class UiPulseEffect : MonoBehaviour
{
    public Material uiMaterial; // assign the material used by the UI element
    public Color baseEmissionColor = Color.cyan;
    public float pulseSpeed = 2f;
    public float intensityMultiplier = 2f;

    private void Start()
    {
        if (uiMaterial == null)
        {
            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
                uiMaterial = renderer.material;
        }

        uiMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        Color emission = baseEmissionColor * Mathf.LinearToGammaSpace(pulse * intensityMultiplier);
        uiMaterial.SetColor("_EmissionColor", emission);
    }
}
