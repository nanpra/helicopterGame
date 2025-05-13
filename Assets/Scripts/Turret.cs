using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    [Header("Turret Settings")]
    public Transform turretHead;
    public float rotationSpeed = 5f;
    public float detectionRange = 30f;
    public float fireRate = 2f;
    public string bulletTag = "Bullet";

    [Header("Bullet Settings")]
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float leadTime = 0.5f;
    public float burstIntervalTime = 3f;

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
        bool helicopterNotCrossed = helicopterTransform.position.z + 6 <= transform.position.z;

        if (helicopterInRange && helicopterNotCrossed)
        {
            RotateTowardsTarget();
            if (Time.time >= nextFireTime)
            {
                StartCoroutine(FireBurst());
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

    private IEnumerator FireBurst()
    {
        for (int i = 0; i < 3; i++)
        {
            Fire();
            yield return new WaitForSeconds(0.4f);
        }
        yield return new WaitForSeconds(burstIntervalTime);
    }

    private void Fire()
    {
        GameObject bullet = PoolingObjects.Instance.SpawnFromPool(bulletTag, firePoint.position, firePoint.rotation);
        if (bullet == null) return;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 predictedPosition = PredictHelicopterPosition();
            Vector3 fireDirection = (predictedPosition - firePoint.position).normalized;
            rb.linearVelocity = fireDirection * bulletSpeed;
        }
    }

    private Vector3 PredictHelicopterPosition()
    {
        if (helicopterTransform == null) return firePoint.position;

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
