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
    public string healthTag = "Health";
    public string bulletTag = "Bullet";

    private List<Vector3> occupiedPositions = new List<Vector3>();
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
        while (!occupiedPositions.Contains(position) && attempts < maxAttempts)
        {
            posX = GameManager.Instance.helicopterScript.transform.position.x + Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
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
                Vector3 height = obj.transform.localScale;
                height.y *= buildingHeight;
                obj.transform.localScale = height;
                obj.transform.position = new Vector3(position.x, buildingHeight / 2, position.z);
                TrySpawnElement(obj);
            }

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
                else if (obj.CompareTag(healthTag))
                    PoolingObjects.Instance.ReturnToPool(healthTag, obj);

                activeObjects.RemoveAt(i);
            }
        }
    }

    private Dictionary<Transform, string> buildingDebugInfo = new Dictionary<Transform, string>();

    private void TrySpawnElement(GameObject building)
    {
        Transform buildingTransform = building.transform;
        float chance = Random.value;

        if (chance < 0.25f && PoolingObjects.Instance.HasAvailableObject(turretTag))
        {
            Vector3 turretPosition = GetBuildingTopPosition(building);
            GameObject turret = PoolingObjects.Instance.SpawnFromPool(turretTag, turretPosition, Quaternion.identity);
            activeObjects.Add(turret);
            buildingDebugInfo[buildingTransform] = turretTag;
        }
        else if (chance >= 0.25f && chance < 0.55f && PoolingObjects.Instance.HasAvailableObject(fuelTag))
        {
            float randomZaxisPos = Random.Range(0, 15);
            float randomheightPos = Random.Range(5, 18);
            Vector3 pickupPosition = GetBuildingTopPosition(building) + Vector3.up * randomheightPos + Vector3.forward * randomZaxisPos;
            GameObject pickup = PoolingObjects.Instance.SpawnFromPool(fuelTag, pickupPosition, Quaternion.identity);
            activeObjects.Add(pickup);
            buildingDebugInfo[buildingTransform] = fuelTag;
        }
        else if (chance >= 0.55f && chance < 0.85f && PoolingObjects.Instance.HasAvailableObject(healthTag))
        {
            float randomZaxisPos = Random.Range(0, 15);
            float randomheightPos = Random.Range(5, 18);
            Vector3 pickupPosition = GetBuildingTopPosition(building) + Vector3.up * randomheightPos + Vector3.forward * randomZaxisPos;
            GameObject pickup = PoolingObjects.Instance.SpawnFromPool(healthTag, pickupPosition, Quaternion.identity);
            activeObjects.Add(pickup);
            buildingDebugInfo[buildingTransform] = healthTag;
        }
        else
        {
            buildingDebugInfo[buildingTransform] = "None";
        }
    }

    private void OnDrawGizmos()
    {
        if (buildingDebugInfo == null) return;

        foreach (var entry in buildingDebugInfo)
        {
            Transform building = entry.Key;
            string type = entry.Value;

            switch (type)
            {
                case "Turret":
                    Gizmos.color = Color.red;
                    break;
                case "Pickup":
                    Gizmos.color = Color.green;
                    break;
                case "None":
                    Gizmos.color = Color.gray;
                    break;
            }

            Gizmos.DrawSphere(GetBuildingTopPosition(building.gameObject) + Vector3.up * 2f, 0.3f);
        }
    }
}