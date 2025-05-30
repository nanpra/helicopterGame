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
    public float shakeIntensity = 0.05f;
    public float shakeDuration = 0.15f;
    public float buildingHitDamage = 35f;

    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;
    public float fuelPickupAmount = 20f;

    [Header("Joystick Reference")]
    public Joystick joystick;

    private Vector3 inputDirection;
    private Quaternion targetRotation;
    private Rigidbody rb;

    public Animator propellerAnim;

    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(GameManager.Instance.CurrentState == GameState.Playing)
        {
            HandleInput();
            MoveForward();
            LimitMovement();
            RotateTowardsInput();
        }
    }

    private void HandleInput()
    {
        Vector2 input = joystick.GetJoystickInput();
        inputDirection = new Vector3(input.x, -input.y , 1f).normalized;
        if (inputDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
        }
    }
    private void LimitMovement()
    {
        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.Clamp(newPosition.y, 0, 40);
        newPosition.x = Mathf.Clamp(newPosition.x, -50, 50);
        transform.position = newPosition;
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

    public void TakeDamage()
    {
        GameManager.Instance.healthSlider.value -= 0.1f;
        // Instantiate hit effect
        //if (hitEffect != null)
        //{
        //    Instantiate(hitEffect, transform.position, Quaternion.identity);
        //}
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif

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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Building"))
        {
            GameManager.Instance.healthSlider.value -= buildingHitDamage;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fuel"))
        {
            PickupFuel(other.gameObject);
        }
        else if (other.CompareTag("Health"))
        {
            PickupObject(other.gameObject);
        }
    }
    [SerializeField] private GameObject flyingSparklePrefabForFuel;
    [SerializeField] private GameObject flyingSparklePrefabForHealth;
    [SerializeField] private RectTransform healthBarTargetUI;
    [SerializeField] private RectTransform fuelBarTargetUI;

    private void PickupFuel(GameObject fuel)
    {
        GameManager.Instance.fuelSlider.value += fuelPickupAmount/100;
        PoolingObjects.Instance.ReturnToPool("Fuel", fuel);

        GameObject sparkle = Instantiate(flyingSparklePrefabForFuel, fuel.transform.position, Quaternion.identity);
        sparkle.GetComponent<SparkleEffect>().Initialize(fuelBarTargetUI);
    }


    private void PickupObject(GameObject pickup)
    {
        // Handle other pickups (e.g., coins, power-ups)
        PoolingObjects.Instance.ReturnToPool("Health", pickup);
        GameManager.Instance.healthSlider.value += 0.2f;

        // Spawn sparkle at pickup location
        GameObject sparkle = Instantiate(flyingSparklePrefabForHealth, pickup.transform.position, Quaternion.identity);
        sparkle.GetComponent<SparkleEffect>().Initialize(healthBarTargetUI);
    }
}