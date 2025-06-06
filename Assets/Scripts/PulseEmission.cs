using UnityEngine;

public class PulseEmission : MonoBehaviour
{
    public MeshRenderer turretRenderer;
    public Color baseColor = Color.red;
    public float pulseSpeed = 0.5f;
    private Material mat;

    void Start()
    {
        mat = turretRenderer.material;
        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        Color emission = baseColor * Mathf.LinearToGammaSpace(pulse * 20f);
        mat.SetColor("_EmissionColor", emission);
    }
}