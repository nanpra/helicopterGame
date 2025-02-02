using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform helicopter;
    public Vector2Int gridSize = new Vector2Int(10, 10);
    public Vector3 spawnAreaSize = new Vector3(100, 0, 2);
    public float despawnDistance = 50f;

    [Header("Object Pooling Tags")]
    public string buildingTag = "Building";
    public string turretTag = "Turret";
    public string fuelTag = "Fuel";
    public string smokeTag = "Smoke";
    public string bulletTag = "Bullet";

    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private List<GameObject> activeObjects = new List<GameObject>();

    private float nextSpawnZ = 0f;

    private void Start()
    {
        GenerateInitialObjects();
    }

    private void Update()
    {
        if (helicopter.position.z + despawnDistance > nextSpawnZ)
        {
            GenerateNewRow();
        }

        DespawnObjects();
    }

    private void GenerateInitialObjects()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                Vector3 position = GetRandomPosition(x, z);
                SpawnObject(buildingTag, position);
            }
        }

        nextSpawnZ += spawnAreaSize.z;
    }

    private void GenerateNewRow()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            float intz = nextSpawnZ / spawnAreaSize.z;
            int INTZ = (int)intz;
            Vector3 position = GetRandomPosition(x, INTZ);
            SpawnObject(buildingTag, position);
        }

        nextSpawnZ += spawnAreaSize.z;
    }

    private Vector3 GetRandomPosition(int x, int z)
    {
        float posX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float posZ = z * spawnAreaSize.z;
        Vector3 position = new Vector3(posX, 0, posZ);

        // Ensure position is not occupied by another building
        int maxAttempts = 10; // Prevents infinite loops
        int attempts = 0;
        while (occupiedPositions.Contains(position) && attempts < maxAttempts)
        {
            posX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            position = new Vector3(posX, 0, posZ);
            attempts++;
        }

        occupiedPositions.Add(position);
        return position;
    }

    private void SpawnObject(string tag, Vector3 position)
    {
        GameObject obj = PoolingObjects.Instance.SpawnFromPool(tag, position, Quaternion.identity);

        if (obj != null)
        {
            float buildingHeight = Random.Range(0.75f, 1f);
            if (tag == "Building")
            {
                GameObject yAxisScale = obj;
                Vector3 height = obj.transform.localScale;
                height.y *= buildingHeight;
                obj.transform.localScale = height;
                obj.transform.position = new Vector3(position.x, buildingHeight / 2, position.z);
                //SpawnSmoke(obj, buildingHeight);
                //activeObjects.Add(obj);
                TrySpawnPickup(obj);
                TrySpawnTurret(obj);
            }

            activeObjects.Add(obj);
        }
    }

    private void SpawnSmoke(GameObject building, float buildingHeight)
    {
        if (PoolingObjects.Instance.HasAvailableObject("Smoke"))
        {
            Vector3 smokePosition = new Vector3(building.transform.position.x, buildingHeight / 2, building.transform.position.z);
            GameObject smoke = PoolingObjects.Instance.SpawnFromPool("Smoke", smokePosition, Quaternion.identity);
        }
    }



    private void TrySpawnTurret(GameObject building)
    {
        if (Random.value < 0.3f  &&  PoolingObjects.Instance.HasAvailableObject(turretTag)) // 30% chance to spawn a turret
        {
            Vector3 turretPosition = GetBuildingTopPosition(building);
            GameObject obj = PoolingObjects.Instance.SpawnFromPool(turretTag, turretPosition, Quaternion.identity);
            activeObjects.Add(obj);
        }
    }

    private void TrySpawnPickup(GameObject building)
    {
        if (Random.value < 0.5f  &&  PoolingObjects.Instance.HasAvailableObject(fuelTag)) // 50% chance to spawn a pickup
        {
            Vector3 pickupPosition = GetBuildingTopPosition(building);
            GameObject obj = PoolingObjects.Instance.SpawnFromPool(fuelTag, pickupPosition, Quaternion.identity);
            activeObjects.Add(obj);
        }
    }

    private Vector3 GetBuildingTopPosition(GameObject building)
    {
        Renderer renderer = building.GetComponent<Renderer>();
        if (renderer != null)
        {
            return new Vector3(building.transform.position.x, renderer.bounds.max.y, building.transform.position.z);
        }
        else
        {
            return building.transform.position + new Vector3(0, 80f, 0);
        }
    }

    //private void SpawnSmoke(Vector3 spawnPosition)
    //{
    //    Vector3 smokePosition = spawnPosition + new Vector3(0, 5f, 0);
    //    PoolingObjects.Instance.SpawnFromPool(smokeTag, smokePosition, Quaternion.identity);
    //}

    private void DespawnObjects()
    {
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = activeObjects[i];

            if (obj.transform.position.z < helicopter.position.z - despawnDistance + 30)
            {
                if (obj.CompareTag(buildingTag))
                    PoolingObjects.Instance.ReturnToPool(buildingTag, obj);
                else if (obj.CompareTag(turretTag))
                    PoolingObjects.Instance.ReturnToPool(turretTag, obj);
                else if (obj.CompareTag(fuelTag))
                    PoolingObjects.Instance.ReturnToPool(fuelTag, obj);
                //else if (obj.CompareTag(smokeTag))
                //    PoolingObjects.Instance.ReturnToPool(smokeTag, obj);

                activeObjects.RemoveAt(i);
            }
        }
    }
}