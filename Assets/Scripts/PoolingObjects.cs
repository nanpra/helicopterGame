using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public int size;
}

public class PoolingObjects : MonoBehaviour
{
    public static PoolingObjects Instance { get; private set; }

    [Header("Pool Settings")]
    public List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            if (poolDictionary.ContainsKey(pool.tag))
            {
                Debug.LogError($"Duplicate pool tag detected: {pool.tag}. Tags must be unique.");
                continue;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreateNewPoolObject(pool.prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    private GameObject CreateNewPoolObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        obj.transform.SetParent(transform); // Optional: Organize under the Pooling Manager
        return obj;
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = GetPooledObject(tag);

        // Reset the object before activating it
        ResetObject(objectToSpawn, position, rotation);

        return objectToSpawn;
    }

    private GameObject GetPooledObject(string tag)
    {
        Queue<GameObject> objectPool = poolDictionary[tag];

        if (objectPool.Count == 0)
        {
            Debug.LogWarning($"Pool with tag {tag} is empty. Expanding pool...");
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool != null)
            {
                objectPool.Enqueue(CreateNewPoolObject(pool.prefab));
            }
        }

        GameObject objectToSpawn = objectPool.Dequeue();
        objectPool.Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    private void ResetObject(GameObject obj, Vector3 position, Quaternion rotation)
    {
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        // Reset any Rigidbody (if present) to avoid leftover velocity
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            obj.SetActive(false);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            poolDictionary[tag].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"Cannot return object to pool. Pool with tag {tag} doesn't exist.");
            Destroy(obj);
        }
    }
}