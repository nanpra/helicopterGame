using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Generation Settings")]
    public Transform helicopter;       // Reference to the player's helicopter
    public float spawnDistance = 50f;  // Distance ahead to spawn obstacles
    public float despawnDistance = 20f; // Distance behind to despawn obstacles
    public float obstacleSpacing = 10f; // Spacing between consecutive obstacles

    [Header("Obstacle Settings")]
    public string obstacleTag = "Obstacle";
    public Vector3 spawnArea = new Vector3(10, 10, 0);

    private float nextSpawnZ;
    private Transform playerLastTransform;

    private Queue<GameObject> activeObstacles = new Queue<GameObject>();

    private void Start()
    {
        nextSpawnZ = helicopter.position.z + spawnDistance;
    }

    private void Update()
    {
        GenerateObstacles();
        DespawnObstacles();
    }

    private void GenerateObstacles()
    {
        while (helicopter.position.z + spawnDistance > nextSpawnZ)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnArea.x, spawnArea.x),Random.Range(-spawnArea.y, spawnArea.y),nextSpawnZ);
            GameObject obstacle = PoolingObjects.Instance.SpawnFromPool(obstacleTag, spawnPosition, Quaternion.identity);

            if (obstacle != null)
            {
                activeObstacles.Enqueue(obstacle);
            }
            nextSpawnZ += obstacleSpacing;
        }
    }

    private void DespawnObstacles()
    {
        while (activeObstacles.Count > 0)
        {
            GameObject oldestObstacle = activeObstacles.Peek();

            if (oldestObstacle.transform.position.z < helicopter.position.z - despawnDistance)
            {
                PoolingObjects.Instance.ReturnToPool(obstacleTag, oldestObstacle);
                activeObstacles.Dequeue();
            }
            else
            {
                break;
            }
        }
    }
}
