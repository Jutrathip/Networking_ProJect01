using UnityEngine;

public enum EnemyState
{
    Idle,
    Chase,
    Attack
}

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAIWithState : MonoBehaviour
{
    [Header("Target (ผู้เล่น)")]
    public Transform target;

    [Header("ตำแหน่งยิง")]
    public Transform firePoint;

    [Header("Prefab ของกระสุน (ต้องมีสคริปต์ EnemyBullet)")]
    public GameObject bulletPrefab;

    [Header("ความเร็วเดิน")]
    public float speed = 2f;

    [Header("ความเร็วกระสุน")]
    public float bulletSpeed = 10f;

    [Header("อัตราการยิง (ครั้ง/วินาที)")]
    public float fireRate = 1f;

    [Header("ระยะเริ่มตาม")]
    public float chaseDistance = 8f;

    [Header("ระยะโจมตี (ยิง)")]
    public float attackDistance = 4f;

    [Header("LayerMask ของ กำแพง และ ผู้เล่น (Wall, Player)")]
    public LayerMask obstructionMask;

    private float fireCooldown;
    private EnemyState currentState;

    void Start()
    {
        currentState = EnemyState.Idle;
        if (target == null)
        {
            var ply = GameObject.FindGameObjectWithTag("Player");
            if (ply) target = ply.transform;
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector2 pos = transform.position;
        Vector2 dir = ((Vector2)target.position - pos).normalized;
        float dist = Vector2.Distance(pos, target.position);

        // ตรวจ Raycast ดูว่ามีสิ่งกีดขวางในแนวยิงภายใน attackDistance
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, dir, attackDistance, obstructionMask);

        // กำหนดสถานะ
        if (dist > chaseDistance)
            currentState = EnemyState.Idle;
        else if (hit.collider != null)  // มีกำแพงหรือผู้เล่นในระยะยิง
            currentState = EnemyState.Attack;
        else
            currentState = EnemyState.Chase;

        // พฤติกรรมแต่ละสถานะ
        switch (currentState)
        {
            case EnemyState.Idle:
                // อาจต่อเติมอนิเมชัน Idle ได้
                break;

            case EnemyState.Chase:
                // ไล่ตามผู้เล่น
                transform.position = Vector2.MoveTowards(pos, target.position, speed * Time.deltaTime);
                break;

            case EnemyState.Attack:
                HandleAttack(hit, dir);
                break;
        }

        // หมุนหันไปทางผู้เล่น (หรือแนวยิง)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void HandleAttack(RaycastHit2D hit, Vector2 dir)
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown > 0f || hit.collider == null) return;

        fireCooldown = 1f / fireRate;

        // คำนวณทิศทางยิงไปยังจุดชน (hit.point)
        Vector2 shootDir = (hit.point - (Vector2)firePoint.position).normalized;

        // สปอนน์กระสุน
        var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.velocity = shootDir * bulletSpeed;
        }
    }
}
