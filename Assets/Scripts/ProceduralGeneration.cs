using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Generation Settings")]
    public Transform helicopter;
    public float despawnDistance = 20f;
    public string obstacleTag = "Obstacle";
    public Vector3 spawnArea = new Vector3(30, 0, 0);

    private Queue<GameObject> activeObstacles = new Queue<GameObject>();
    private GameObject lastSpawnedObstacle;

    private void Start()
    {
        SpawnFirstObstacle();
    }

    private void Update()
    {
        CheckForNewObstacle();
        DespawnObstacles();
    }

    private void SpawnFirstObstacle()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), helicopter.position.z + 30f);
        GameObject firstObstacle = PoolingObjects.Instance.SpawnFromPool(obstacleTag, spawnPosition, Quaternion.identity);

        if (firstObstacle != null)
        {
            activeObstacles.Enqueue(firstObstacle);
            lastSpawnedObstacle = firstObstacle;
        }
    }

    private void CheckForNewObstacle()
    {
        if (lastSpawnedObstacle == null) return;

        float obstacleLength = lastSpawnedObstacle.GetComponent<Renderer>().bounds.size.z - 10;
        float spawnTriggerZ = lastSpawnedObstacle.transform.position.z - (1.25f * obstacleLength);
        float fullSpawnZ = lastSpawnedObstacle.transform.position.z + obstacleLength;

        // spawn after 50%
        if (helicopter.position.z >= spawnTriggerZ)
        {
            SpawnNextObstacle(fullSpawnZ);
        }
    }

    private void SpawnNextObstacle(float spawnZ)
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), spawnZ);
        GameObject newObstacle = PoolingObjects.Instance.SpawnFromPool(obstacleTag, spawnPosition, Quaternion.identity);

        if (newObstacle != null)
        {
            activeObstacles.Enqueue(newObstacle);
            lastSpawnedObstacle = newObstacle;
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