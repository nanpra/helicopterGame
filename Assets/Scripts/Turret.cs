using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    [Header("Turret Settings")]
    public Transform turretHead;
    public float rotationSpeed = 5f;
    public float detectionRange = 30f;
    public float fireRate = 2f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float leadTime = 0.5f;
    public float burstIntervalTime = 2.5f;


    [Header("Target")]
    public Transform helicopter;

    private float nextFireTime;

    private void FixedUpdate()
    {
        if (helicopter == null) return;
        float distanceToHelicopter = Vector3.Distance(transform.position, helicopter.position);
        if (distanceToHelicopter <= detectionRange)
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
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(burstIntervalTime);
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 predictedPosition = PredictHelicopterPosition();
            Vector3 fireDirection = (predictedPosition - firePoint.position).normalized;

            rb.linearVelocity = fireDirection * bulletSpeed;
            Debug.Log("Bullet fired towards: " + predictedPosition);
        }
    }

    private Vector3 PredictHelicopterPosition()
    {
        if (helicopter == null) return firePoint.position;

        Rigidbody heliRb = helicopter.GetComponent<Rigidbody>();
        if (heliRb == null) return helicopter.position;
        return helicopter.position + (heliRb.linearVelocity * leadTime); //shoot ahead in time
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
