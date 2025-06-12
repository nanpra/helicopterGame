using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;


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

    [Header("DestructionHelicopter")]
    public GameObject smokeEffect;
    public GameObject explosionEffect;
    public GameObject fireTrailPrefab;
    public bool isDestroyed = false;
    public Rigidbody[] parts;

    [Header("References")]
    public Joystick joystick;
    private CinemachineImpulseSource cinemachineImpulseSource;
    public CinemachineCamera deathCam;

    //remaining

    [SerializeField] private GameObject flyingSparklePrefabForFuel;
    [SerializeField] private GameObject flyingSparklePrefabForHealth;
    [SerializeField] private RectTransform healthBarTargetUI;
    [SerializeField] private RectTransform fuelBarTargetUI;

    private Vector3 inputDirection;
    private Quaternion targetRotation;
    [HideInInspector] public Rigidbody rb;
    public Animator propellerAnim;
    public TMP_Dropdown controlInput;

    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
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
        if (controlInput.value == 1)
            inputDirection = new Vector3(input.x, input.y, 1f).normalized;

        else if (controlInput.value == 0)
            inputDirection = new Vector3(input.x, -input.y, 1f).normalized;
            
        if (inputDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
        }
    }
    private void LimitMovement()
    {
        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.Clamp(newPosition.y, 0, 50);
        //newPosition.x = Mathf.Clamp(newPosition.x, -50, 50);
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
    public bool halfHealth;

    public void TakeDamage(float damageValue)
    {
        halfHealth = false;
        GameManager.Instance.healthSlider.value -= damageValue;
        if (GameManager.Instance.healthSlider.value <= 0.6f)
        {
            halfHealth = true;
            smokeEffect.SetActive(true);
            GameObject smokePrefab = Instantiate(smokeEffect, transform.position, Quaternion.identity);
            smokePrefab.transform.SetParent(transform);
        }




#if UNITY_ANDROID || UNITY_IOS
        if (PlayerPrefsExtra.GetBool("VibrationToggle", true))
        {
            Handheld.Vibrate();
        }

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
        if (other.CompareTag("FuelTank"))
            PickupFuel(other.gameObject);
        else if (other.CompareTag("HealthTank"))
            PickupObject(other.gameObject);
        else if (other.CompareTag("Ballon"))
            BlastBalloon(other.gameObject);
    }
    public GameObject ballonBlasteffect;
    private void BlastBalloon(GameObject ballon)
    {
        AudioManager.instance.Play("BallonPop"); 
        Instantiate(ballonBlasteffect , ballon.transform.position, Quaternion.identity);
        PoolingObjects.Instance.ReturnToPool(ballon.transform.parent.gameObject.tag, ballon.transform.parent.gameObject);

    }

    private void PickupFuel(GameObject fuel)
    {
        GameManager.Instance.fuelSlider.value += fuelPickupAmount/100;
        PoolingObjects.Instance.ReturnToPool(fuel.transform.parent.gameObject.tag, fuel.transform.parent.gameObject);

        GameObject sparkle = Instantiate(flyingSparklePrefabForFuel, fuel.transform.position, Quaternion.identity);
        sparkle.GetComponent<SparkleEffect>().Initialize(fuelBarTargetUI);
    }


    private void PickupObject(GameObject pickup)
    {
        if (!halfHealth)
            smokeEffect.gameObject.SetActive(false);

        PoolingObjects.Instance.ReturnToPool("Health", pickup);
        GameManager.Instance.healthSlider.value += 0.2f;

        GameObject sparkle = Instantiate(flyingSparklePrefabForHealth, pickup.transform.position, Quaternion.identity);
        sparkle.GetComponent<SparkleEffect>().Initialize(healthBarTargetUI);
    }



    public void DestroyHelicopter(Transform explosionPos)
    {
        deathCam.gameObject.SetActive(true);
        cinemachineImpulseSource.GenerateImpulse();
        forwardSpeed = 2;
        isDestroyed = true;
        AudioManager.instance.Play("Explode");
        AudioManager.instance.Stop("HeliSound");

        // Explosion VFX
        if (explosionEffect != null && explosionPos != null)
        {
            Instantiate(explosionEffect, explosionPos.position, Quaternion.identity);
        }

        //slow motion
        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        EnableGravityOnAllChildren();
        foreach (var rb in parts)
        {
            rb.isKinematic = false;
            rb.AddExplosionForce(2600, explosionPos.position, 100f, 10f);
        }
        StartCoroutine(RestoreTime());
    }

    void EnableGravityOnAllChildren()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child == transform) continue;

            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
            }
        }
    }


    IEnumerator RestoreTime()
    {
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        AudioManager.instance.bgSound.volume = 0.7f;
        AudioManager.instance.StopAll();
        UiManager.instance.gameplayPanel.SetActive(false);
    }
}