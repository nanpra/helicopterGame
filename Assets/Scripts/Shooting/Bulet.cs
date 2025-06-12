using UnityEngine;

public class Bulet : MonoBehaviour
{

    public float bulletSpeed = 20f;
    public float lifeTime = 5f;
    private string bulletTag = "Bullet";

    private void OnEnable()
    {
        Invoke("ReturnToPool", lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Helicopter>().TakeDamage(0.1f);
            AudioManager.instance.Play("BulletHit");
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        PoolingObjects.Instance.ReturnToPool(bulletTag, gameObject);
    }
}
