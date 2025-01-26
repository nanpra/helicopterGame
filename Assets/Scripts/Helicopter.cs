using UnityEngine;
using UnityEngine.UI;

public class Helicopter : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 10f;
    public float rotationSpeed = 5f;
    public float tiltAmount = 15f;
    public float verticalSpeed = 5f;

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
        //DownwardForce();
        HandleInput();
        MoveHelicopter();
        RotateTowardsInput();
    }

    private void HandleInput()
    {
        Vector2 input  = joystick.InputVector;
        inputDirection = new Vector3(input.x, input.y , 1f).normalized;
        if (inputDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(new Vector3(inputDirection.x, 0f, inputDirection.z), Vector3.up);
        }
    }

    private void MoveHelicopter()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.Self);
        if (!gasOver)
        {
            float verticalMovement = inputDirection.y * verticalSpeed * Time.deltaTime;
            transform.Translate(Vector3.up * verticalMovement, Space.World);
        }
    }

    private void RotateTowardsInput()
    {
        if (Mathf.Abs(inputDirection.x) > 0.1f)
        {
            float turnDirection = inputDirection.x;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            float tilt = -inputDirection.x * tiltAmount;
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, tilt);
        }
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

    private void DecreaseFuel()
    {
        fuelSlider.value -= fuelConsumptionRate * Time.deltaTime;
        if(fuelSlider.value <= 0f)
        {
            gasOver = true;
        }
    }
}