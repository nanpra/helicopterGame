using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret Settings")]
    public Transform turretHead;
    public float rotationSpeed = 5f;
    public float detectionRange = 30f;
    public float fireRate = 1.5f;
    public string bulletTag = "Bullet";

    [Header("Bullet Settings")]
    public Transform[] firePoints; // Add 2 fire points in inspector
    public float bulletSpeed = 20f;
    public float bulletDamage = 10f;
    public float leadTime = 0.5f;

    private float nextFireTime;
    private Transform helicopterTransform;

    private void Start()
    {
        if (GameManager.Instance.helicopterScript != null)
        {
            helicopterTransform = GameManager.Instance.helicopterScript.transform;
        }
    }

    private void FixedUpdate()
    {
        if (helicopterTransform == null) return;

        float distanceToHelicopter = Vector3.Distance(transform.position, helicopterTransform.position);
        bool helicopterInRange = distanceToHelicopter <= detectionRange;
        bool helicopterNotCrossed = helicopterTransform.position.z - 1 <= transform.position.z;

        if (helicopterInRange && helicopterNotCrossed)
        {
            RotateTowardsTarget();
            if (Time.time >= nextFireTime)
            {
                FireFromAllPoints();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 predictedPosition = PredictHelicopterPosition();
        Vector3 direction = (predictedPosition - turretHead.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        turretHead.rotation = Quaternion.Slerp(turretHead.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void FireFromAllPoints()
    {
        Vector3 predictedPosition = PredictHelicopterPosition();
        Vector3 fireDirection = (predictedPosition - firePoints[0].position).normalized;

        foreach (Transform point in firePoints)
        {
            GameObject bullet = PoolingObjects.Instance.SpawnFromPool(bulletTag, point.position, Quaternion.LookRotation(fireDirection));
            if (bullet != null)
            {
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = fireDirection * bulletSpeed;
                }
            }
        }
        AudioManager.instance.Play("TurretSound");
    }

    private Vector3 PredictHelicopterPosition()
    {
        if (helicopterTransform == null) return Vector3.zero;

        Rigidbody heliRb = helicopterTransform.GetComponent<Rigidbody>();
        if (heliRb == null) return helicopterTransform.position;

        return helicopterTransform.position + (heliRb.linearVelocity * leadTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
