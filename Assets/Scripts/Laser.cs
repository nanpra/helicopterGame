using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
    private Transform target;
    public Transform turretHead;
    public LineRenderer laserBeam;
    public float range = 15f;
    public float damage = 0.1f;
    public float fireInterval = 3f;
    public float laserDuration = 0.3f;

    private float fireTimer;
    private bool isFiring = false;

    private Transform helicopterTransform;
    void Start()
    {
        if (GameManager.Instance.helicopterScript != null)
        {
            helicopterTransform = GameManager.Instance.helicopterScript.transform;
            target = helicopterTransform;
        }
        laserBeam.enabled = false;
        fireTimer =0;
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= range)
        {
            RotateTowardsTarget();

            fireTimer -= Time.deltaTime;

            if (fireTimer <= 0f)
            {
                StartCoroutine(FireLaser());
                fireTimer = fireInterval;
            }
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = (target.position - turretHead.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        turretHead.rotation = Quaternion.Slerp(turretHead.rotation, lookRotation, Time.deltaTime * 5f);
    }

    IEnumerator FireLaser()
    {
        isFiring = true;
        laserBeam.enabled = true;
        laserBeam.SetPosition(0, laserBeam.transform.position);
        laserBeam.SetPosition(1, target.position);

        Helicopter playerHealth = target.GetComponent<Helicopter>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        yield return new WaitForSeconds(laserDuration);

        laserBeam.enabled = false;
        isFiring = false;
    }
}
