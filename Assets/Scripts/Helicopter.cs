using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Helicopter : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 10f;
    public float rotationSpeed = 5f;
    public float tiltAmount = 15f;
    public float upwardForce = 100;

    [Header("Impact Settings")]
    //public GameObject hitEffect;
    public float shakeIntensity = 0.2f;
    public float shakeDuration = 0.2f;

    [Header("Joystick Reference")]
    public Joystick joystick;

    private Vector3 inputDirection;
    private Quaternion targetRotation;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleInput();
        MoveForward();
        RotateTowardsInput();
    }

    private void HandleInput()
    {
        Vector2 input = joystick.InputVector;
        inputDirection = new Vector3(input.x, 0f, input.y).normalized;
        if (inputDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
        }
    }

    private void MoveForward()
    {
        transform.Translate(transform.forward * forwardSpeed * Time.deltaTime, Space.World);
    }

    private void RotateTowardsInput()
    {
        if (inputDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            float tilt = Mathf.Lerp(0f, -tiltAmount, Mathf.Abs(inputDirection.x));
            transform.localRotation *= Quaternion.Euler(0f, 0f, tilt * Mathf.Sign(inputDirection.x));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage();
            GameManager.Instance.healthSlider.value -= 0.1f;
            Destroy(other.gameObject);
        }
    }

    private void TakeDamage()
    {
        // Instantiate hit effect
        //if (hitEffect != null)
        //{
        //    Instantiate(hitEffect, transform.position, Quaternion.identity);
        //}

        // Vibrate the phone
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif

        // Shake the helicopter
        StartCoroutine(ShakeHelicopter());
    }

    private IEnumerator ShakeHelicopter()
    {
        Vector3 originalPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float xOffset = Random.Range(-shakeIntensity, shakeIntensity);
            float yOffset = Random.Range(-shakeIntensity, shakeIntensity);

            transform.position = originalPosition + new Vector3(xOffset, yOffset, 0);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition; // Reset position after shake
    }

    //private void DownwardForce()
    //{
    //    Vector3 gravityForce = new Vector3(0, -downwardForce, 0);
    //    rb.AddForce(gravityForce);
    //}

    //public void GasPressed()
    //{
    //    if(!gasOver)
    //    {
    //        Vector3 force = new Vector3(0, upwardForce, 0);
    //        rb.AddForce(force);
    //    }
    //}
}