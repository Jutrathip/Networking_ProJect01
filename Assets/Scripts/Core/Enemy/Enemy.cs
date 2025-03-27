using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class Enemy : NetworkBehaviour
{
    public float speed = 2f;
    public Transform target;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float bulletSpeed = 5f;

    private float nextFireTime = 0f;
    private Health health;

    private void Start()
    {
        if (!IsServer) return; // ✅ Enemy ควบคุมโดย Server เท่านั้น

        target = GameObject.FindWithTag("Base")?.transform;
        health = GetComponent<Health>();

        if (health != null)
        {
            health.OnDie += HandleDeath;
        }

        StartCoroutine(FindPlayer());
    }

    private void Update()
    {
        if (!IsServer || health == null || health.CurrentHealth.Value <= 0) return;
        MoveTowardsTarget();
        TryToShoot();
    }

    void MoveTowardsTarget()
    {
        if (target)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    void TryToShoot()
    {
        if (Time.time < nextFireTime || target == null) return;

        nextFireTime = Time.time + fireRate;
        ShootServerRpc(target.position);
    }

    [ServerRpc]
    void ShootServerRpc(Vector3 targetPosition)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<NetworkObject>().Spawn(true);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = (targetPosition - firePoint.position).normalized;
            rb.velocity = direction * bulletSpeed;
        }
    }

    IEnumerator FindPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void HandleDeath(Health deadHealth)
    {
        if (IsServer)
        {
            Destroy(gameObject);
        }
    }
}
