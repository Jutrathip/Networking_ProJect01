using UnityEngine;
using System.Collections.Generic;

public enum EnemyState
{
    Idle,
    Chase,
    AttackWall,
    AttackPlayer
}

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAIWithState : MonoBehaviour
{
    [Header("Target (ลาก Player หรือใช้ Tag 'Player')")]
    public Transform target;

    [Header("ตำแหน่งยิง (Fire Point)")]
    public Transform firePoint;

    [Header("Prefab กระสุน")]
    public GameObject bulletPrefab;

    [Header("ความเร็วเดิน")]
    public float speed = 2f;

    [Header("ความเร็วกระสุน")]
    public float bulletSpeed = 10f;

    [Header("อัตราการยิง (ครั้ง/วินาที)")]
    public float fireRate = 1f;

    [Header("ระยะเริ่มตาม (ถ้าเกินจะ Idle)")]
    public float chaseDistance = 8f;

    [Header("ระยะยิงผู้เล่น")]
    public float attackDistance = 4f;

    Rigidbody2D rb;
    EnemyState currentState = EnemyState.Idle;
    float fireCooldown;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void Start()
    {
        if (target == null)
        {
            var ply = GameObject.FindGameObjectWithTag("Player");
            if (ply != null) target = ply.transform;
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector2 pos = rb.position;
        Vector2 toPlayer = ((Vector2)target.position - pos).normalized;
        float distToPlayer = Vector2.Distance(pos, target.position);

        // 1) ตรวจกำแพงขวางทางจาก firePoint ไปหา Player
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, toPlayer, distToPlayer);
        bool wallBlocked = false;
        RaycastHit2D nearestWallHit = default;
        float nearestDist = float.MaxValue;

        foreach (var h in hits)
        {
            if (h.collider != null && h.collider.TryGetComponent<DestructibleWall2D>(out _))
            {
                if (h.distance < nearestDist)
                {
                    nearestDist = h.distance;
                    nearestWallHit = h;
                    wallBlocked = true;
                }
            }
        }

        // 2) กำหนดสถานะ
        if (distToPlayer > chaseDistance)
            currentState = EnemyState.Idle;
        else if (wallBlocked)
            currentState = EnemyState.AttackWall;
        else if (distToPlayer > attackDistance)
            currentState = EnemyState.Chase;
        else
            currentState = EnemyState.AttackPlayer;

        // 3) ทำงานตาม state
        switch (currentState)
        {
            case EnemyState.Idle:
                rb.velocity = Vector2.zero;
                break;

            case EnemyState.Chase:
                rb.velocity = toPlayer * speed;
                FaceDirection(rb.velocity);
                break;

            case EnemyState.AttackWall:
                HandleAttackWall(nearestWallHit, toPlayer);
                break;

            case EnemyState.AttackPlayer:
                HandleAttackPlayer(toPlayer);
                break;
        }
    }

    // หันหน้าไปในแนว dir (dir ต้อง normalized และไม่เป็น Vector2.zero)
    void FaceDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.001f) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void HandleAttackWall(RaycastHit2D wallHit, Vector2 toPlayer)
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            // ยิงพังกำแพง
            fireCooldown = 1f / fireRate;
            Vector2 shootDir = (wallHit.point - (Vector2)firePoint.position).normalized;
            FaceDirection(shootDir);

            var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            if (bullet.TryGetComponent<Rigidbody2D>(out var rbB))
                rbB.velocity = shootDir * bulletSpeed;
        }
        else
        {
            // Strafe เดินหลบ: เลือกทิศตั้งฉากซ้ายหรือขวากับ toPlayer
            Vector2 perp = new Vector2(-toPlayer.y, toPlayer.x);
            rb.velocity = perp.normalized * speed;
            FaceDirection(rb.velocity);
        }
    }

    void HandleAttackPlayer(Vector2 toPlayer)
    {
        fireCooldown -= Time.deltaTime;
        rb.velocity = Vector2.zero;

        if (fireCooldown <= 0f)
        {
            fireCooldown = 1f / fireRate;
            FaceDirection(toPlayer);

            var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            if (bullet.TryGetComponent<Rigidbody2D>(out var rbB))
                rbB.velocity = toPlayer * bulletSpeed;
        }
    }
}
