// EnemyAIWithState.cs
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections.Generic;

public enum EnemyState { Idle, Chase, AttackWall, AttackBase }

[RequireComponent(typeof(NetworkObject), typeof(NetworkTransform), typeof(Rigidbody2D))]
public class EnemyAIWithState : NetworkBehaviour
{
    [Header("Target Base (use Tag 'PlayerBase')")]
    public Transform target;

    [Header("Fire Point")]
    public Transform firePoint;

    [Header("Bullet Prefab (must have NetworkObject)")]
    public GameObject bulletPrefab;

    [Header("Movement speed")]
    public float speed = 2f;

    [Header("Bullet speed")]
    public float bulletSpeed = 10f;

    [Header("Fire rate (shots/sec)")]
    public float fireRate = 1f;

    [Header("Chase distance")]
    public float chaseDistance = 8f;

    [Header("Attack distance (won't shoot beyond this)")]
    public float attackDistance = 4f;

    private Rigidbody2D rb;
    private EnemyState currentState = EnemyState.Idle;
    private float fireCooldown;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Only the server assigns the target
        if (IsServer && target == null)
        {
            var go = GameObject.FindGameObjectWithTag("PlayerBase");
            if (go != null) target = go.transform;
        }
    }

    void Update()
    {
        // Only run AI on server/host
        if (!IsServer || target == null)
            return;

        Vector2 pos = rb.position;
        Vector2 toTarget = ((Vector2)target.position - pos);
        float dist = toTarget.magnitude;
        Vector2 dir = toTarget.normalized;

        // Raycast for walls between firePoint and target
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, dir, dist);
        bool wallBlocked = false;
        RaycastHit2D nearestWall = default;
        float nearestDist = float.MaxValue;
        foreach (var h in hits)
        {
            if (h.collider != null && h.collider.TryGetComponent<DestructibleWall2D>(out _)
                && h.distance < nearestDist)
            {
                nearestDist = h.distance;
                nearestWall = h;
                wallBlocked = true;
            }
        }

        // Choose state
        if (dist > chaseDistance) currentState = EnemyState.Idle;
        else if (wallBlocked && nearestDist <= attackDistance) currentState = EnemyState.AttackWall;
        else if (dist > attackDistance) currentState = EnemyState.Chase;
        else currentState = EnemyState.AttackBase;

        // Act on state
        switch (currentState)
        {
            case EnemyState.Idle:
                rb.velocity = Vector2.zero;
                break;

            case EnemyState.Chase:
                rb.velocity = dir * speed;
                Face(dir);
                break;

            case EnemyState.AttackWall:
                HandleAttackWall(nearestWall, dir);
                break;

            case EnemyState.AttackBase:
                HandleAttackBase(dir, dist);
                break;
        }
    }

    void Face(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.001f) return;
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, ang);
    }

    void HandleAttackWall(RaycastHit2D wallHit, Vector2 dir)
    {
        fireCooldown -= Time.deltaTime;
        rb.velocity = Vector2.zero;

        if (fireCooldown <= 0f)
        {
            fireCooldown = 1f / fireRate;
            Vector2 shootDir = (wallHit.point - (Vector2)firePoint.position).normalized;
            Face(shootDir);

            var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            var netObj = bullet.GetComponent<NetworkObject>();
            netObj.Spawn();  // replicate bullet to all clients

            if (bullet.TryGetComponent<Rigidbody2D>(out var rbB))
                rbB.velocity = shootDir * bulletSpeed;
        }
    }

    void HandleAttackBase(Vector2 dir, float dist)
    {
        fireCooldown -= Time.deltaTime;
        rb.velocity = Vector2.zero;

        // Only shoot if within attackDistance
        if (fireCooldown <= 0f && dist <= attackDistance)
        {
            fireCooldown = 1f / fireRate;
            Face(dir);

            var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            var netObj = bullet.GetComponent<NetworkObject>();
            netObj.Spawn();

            if (bullet.TryGetComponent<Rigidbody2D>(out var rbB))
                rbB.velocity = dir * bulletSpeed;
        }
    }
}
