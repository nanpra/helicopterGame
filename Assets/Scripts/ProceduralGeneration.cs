using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Map Settings")]
    public string segmentTag = "Segment";
    public int initialSegments = 5;
    public float segmentLength = 50f;
    public Transform helicopter;

    [Header("Obstacle and Fuel Settings")]
    public string obstacleTag = "Obstacle";
    public string fuelPickupTag = "FuelPickup";
    public int maxObstaclesPerSegment = 3; 
    public int maxFuelPickupsPerSegment = 2; 

    private Queue<GameObject> activeSegments = new Queue<GameObject>();
    private float spawnZ = 0f;

    private void Start()
    {
        for (int i = 0; i < initialSegments; i++)
        {
            SpawnSegment();
        }
    }

    private void Update()
    {
        // Spawn new segments as the helicopter moves forward
        if (helicopter.position.z > spawnZ - (initialSegments * segmentLength))
        {
            SpawnSegment();
            RemoveOldestSegment();
        }
    }

    private void SpawnSegment()
    {
        // Spawn a segment from the pool
        GameObject newSegment = PoolingObjects.Instance.SpawnFromPool(segmentTag, new Vector3(0, 0, spawnZ), Quaternion.identity);
        activeSegments.Enqueue(newSegment);

        // Add random obstacles and pickups
        AddObstaclesAndPickups(newSegment);

        // Update spawn position for the next segment
        spawnZ += segmentLength;
    }

    private void AddObstaclesAndPickups(GameObject segment)
    {
        // Randomly place obstacles
        for (int i = 0; i < maxObstaclesPerSegment; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-10f, 10f),  // Random X position
                0f,                      // Ground level
                Random.Range(-segmentLength / 2, segmentLength / 2) // Random Z position within the segment
            );
            PoolingObjects.Instance.SpawnFromPool(obstacleTag, segment.transform.position + randomPosition, Quaternion.identity);
        }

        // Randomly place fuel pickups
        for (int i = 0; i < maxFuelPickupsPerSegment; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-10f, 10f),
                Random.Range(2f, 5f),     // Floating in the air
                Random.Range(-segmentLength / 2, segmentLength / 2) // Random Z position within the segment
            );
            PoolingObjects.Instance.SpawnFromPool(fuelPickupTag, segment.transform.position + randomPosition, Quaternion.identity);
        }
    }

    private void RemoveOldestSegment()
    {
        if (activeSegments.Count > 0)
        {
            GameObject oldestSegment = activeSegments.Dequeue();
            oldestSegment.SetActive(false);
        }
    }
}
