using UnityEngine;
using UnityEngine.UI;

public class Helicopter : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 10f;
    public float rotationSpeed = 5f; 
    public float tiltAmount = 15f;
    public float upwardForce = 100;
    public float downwardForce = 100;

    [Header("Fuel Settings")]
    public Slider fuelSlider;
    public float fuelConsumptionRate = 0.1f;

    [Header("Joystick Reference")]
    public Joystick joystick; 

    private Vector3 inputDirection;
    private Quaternion targetRotation;
    private Rigidbody rb;
    private bool gasOver;
    private void Start()
    {
        gasOver = false;
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        DecreaseFuel();
        DownwardForce();
        HandleInput();
        MoveForward();
        RotateTowardsInput();
    }

    private void HandleInput()
    {
        Vector2 input  = joystick.InputVector;

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
    private void DownwardForce()
    {
        Vector3 gravityForce = new Vector3(0, -downwardForce, 0);
        rb.AddForce(gravityForce);
    }

    public void GasPressed()
    {
        if(!gasOver)
        {
            Vector3 force = new Vector3(0, upwardForce, 0);
            rb.AddForce(force);
        }
    }

    private void DecreaseFuel()
    {
        fuelSlider.value -= fuelConsumptionRate * Time.deltaTime;
        if(fuelSlider.value <= 0f)
        {
            gasOver = true;
        }
    }
}