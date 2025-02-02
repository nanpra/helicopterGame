using UnityEngine;

public class FloatingSmoke : MonoBehaviour
{
    public float floatSpeed = 0.5f;
    public float floatRange = 1f;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        float offsetX = Mathf.Sin(Time.time * 0.3f) * 0.5f;
        float offsetZ = Mathf.Cos(Time.time * 0.3f) * 0.5f;
        transform.position += new Vector3(offsetX, 0, offsetZ) * Time.deltaTime;
    }
}