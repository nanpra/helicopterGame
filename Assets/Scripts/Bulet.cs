using UnityEngine;

public class Bulet : MonoBehaviour
{

    public float bulletSpeed = 20f;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
