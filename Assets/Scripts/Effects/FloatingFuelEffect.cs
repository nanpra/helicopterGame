using UnityEngine;

public class FloatingFuelEffect : MonoBehaviour
{
    public float floatAmplitude = 0.5f;  // How high it moves
    public float floatFrequency = 1f;    // How fast it moves

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position += new Vector3(0f, yOffset, 0f);
        transform.Rotate(Vector3.up * 20f * Time.deltaTime); // slow spin
    }
}

