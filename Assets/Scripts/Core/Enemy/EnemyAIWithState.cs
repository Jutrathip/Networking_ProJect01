using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections.Generic;

public enum EnemyState { Idle, Chase, AttackWall, AttackBase }

[RequireComponent(typeof(NetworkObject), typeof(NetworkTransform), typeof(Rigidbody2D))]
public class EnemyAIWithState : NetworkBehaviour
{
    [Header("Fire Point")]
    public Transform firePoint;

    [Header("Bullet Prefab (must have NetworkObject & NetworkTransform)")]
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

    // เก็บฐานทั้งหมด (Tag = "PlayerBase")
    private List<Transform> baseTargets = new List<Transform>();
    private Transform currentTarget;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        if (IsServer)
        {
            // หา PlayerBase ทุกตัวในฉาก
            var gos = GameObject.FindGameObjectsWithTag("PlayerBase");
            foreach (var go in gos)
                baseTargets.Add(go.transform);
            ChooseNearestBase();
        }
    }

    private void Update()
    {
        // AI รันเฉพาะบน Server/Host
        if (!IsServer)
            return;

        // ไม่มี target ให้หยุด
        if (currentTarget == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 pos = rb.position;
        Vector2 toBase = (Vector2)currentTarget.position - pos;
        float distToBase = toBase.magnitude;
        Vector2 dirToBase = toBase.normalized;

        // Raycast หาเฉพาะกำแพงในระยะ attackDistance
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, dirToBase, attackDistance);
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

        // เลือก state
        if (distToBase > chaseDistance)
            currentState = EnemyState.Idle;
        else if (wallBlocked)
            currentState = EnemyState.AttackWall;
        else if (distToBase > attackDistance)
            currentState = EnemyState.Chase;
        else
            currentState = EnemyState.AttackBase;

        // ทำพฤติกรรมตาม state
        switch (currentState)
        {
            case EnemyState.Idle:
                rb.velocity = Vector2.zero;
                break;

            case EnemyState.Chase:
                rb.velocity = dirToBase * speed;
                Face(dirToBase);
                break;

            case EnemyState.AttackWall:
                HandleAttackWall(nearestWall, dirToBase);
                break;

            case EnemyState.AttackBase:
                HandleAttackBase(dirToBase);
                break;
        }
    }

    private void Face(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.001f) return;
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, ang);
    }

    private void HandleAttackWall(RaycastHit2D wallHit, Vector2 dir)
    {
        fireCooldown -= Time.deltaTime;
        rb.velocity = Vector2.zero;
        if (fireCooldown <= 0f)
        {
            fireCooldown = 1f / fireRate;
            Vector2 shootDir = ((Vector2)wallHit.point - (Vector2)firePoint.position).normalized;
            Face(shootDir);
            var b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            var netObj = b.GetComponent<NetworkObject>();
            netObj.Spawn();
            if (b.TryGetComponent<Rigidbody2D>(out var rbB))
                rbB.velocity = shootDir * bulletSpeed;
        }
    }

    private void HandleAttackBase(Vector2 dir)
    {
        fireCooldown -= Time.deltaTime;
        rb.velocity = Vector2.zero;
        if (fireCooldown <= 0f)
        {
            fireCooldown = 1f / fireRate;
            Face(dir);
            var b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            var netObj = b.GetComponent<NetworkObject>();
            netObj.Spawn();
            if (b.TryGetComponent<Rigidbody2D>(out var rbB))
                rbB.velocity = dir * bulletSpeed;
        }
    }

    private void ChooseNearestBase()
    {
        float minD = float.MaxValue;
        Vector2 pos = rb.position;
        foreach (var t in baseTargets)
        {
            float d = Vector2.Distance(pos, t.position);
            if (d < minD)
            {
                minD = d;
                currentTarget = t;
            }
        }
    }
}
