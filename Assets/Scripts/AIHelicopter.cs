using UnityEngine;

public class AIHelicopter : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] waypoints;
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float arrivalThreshold = 0.5f;
    public bool loopPath = true;

    private int currentWaypointIndex = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        // Move forward
        transform.position += direction * speed * Time.deltaTime;

        // Rotate smoothly toward the direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Check distance to waypoint
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distance < arrivalThreshold)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = loopPath ? 0 : waypoints.Length - 1;
            }
        }
    }
}
