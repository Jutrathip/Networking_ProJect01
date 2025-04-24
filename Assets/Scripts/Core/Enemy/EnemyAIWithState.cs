using UnityEngine;

public enum EnemyState
{
    Idle,
    Chase,
    Attack
}

public class EnemyAIWithState : MonoBehaviour
{
    public Transform target;
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float speed = 2f;
    public float fireRate = 1f;
    public float chaseDistance = 8f;
    public float attackDistance = 4f;
    public LayerMask obstructionMask;

    private float fireCooldown;
    private EnemyState currentState;

    void Start()
    {
        currentState = EnemyState.Idle;
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) 
            {
                target = player.transform;
            }
        }

    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        Vector2 direction = (target.position - transform.position).normalized;

        // 🔁 เปลี่ยนสถานะ
        if (distance > chaseDistance)
            currentState = EnemyState.Idle;
        else if (distance > attackDistance)
            currentState = EnemyState.Chase;
        else
            currentState = EnemyState.Attack;

        // 🧠 จัดการแต่ละสถานะ
        switch (currentState)
        {
            case EnemyState.Idle:
                // ไม่ทำอะไร
                break;

            case EnemyState.Chase:
                Debug.Log("Chasing Player...");
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                break;

            case EnemyState.Attack:
                fireCooldown -= Time.deltaTime;

                RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, attackDistance, obstructionMask);
                
                if (hit.collider != null && fireCooldown <= 0f)
                {
                    fireCooldown = 1f / fireRate;

                    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                    bullet.GetComponent<Rigidbody2D>().velocity = direction * 10f;
                }
                break;
        }

        // หมุนหาผู้เล่นเสมอ
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Debug.Log("Enemy is active");

    }
}
