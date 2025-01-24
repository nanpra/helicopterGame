using UnityEngine;

public class Helicopter : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f; 
    public float controlSpeed = 3f; 
    public float rotationSpeed = 5f;
    public float tiltAmount = 15f; 

    [Header("References")]
    public Joystick joystick;

    private Vector3 targetDirection;
    private Quaternion targetRotation;

    private void Update()
    {
        Vector2 input = joystick.InputVector;
        Vector3 movementDirection = new Vector3(input.x, input.y, 1).normalized;
        targetDirection = movementDirection;
        HandleRotation();
        MoveHelicopter();
    }

    private void MoveHelicopter()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.Self);
    }

    private void HandleRotation()
    {
        if (targetDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}