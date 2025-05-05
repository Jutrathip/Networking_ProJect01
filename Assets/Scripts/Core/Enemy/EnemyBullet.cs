using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("ความเสียหายต่อตัวละคร และ Base")]
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1) ถ้าโดนกำแพงที่ทำลายได้
        if (other.TryGetComponent<DestructibleWall2D>(out var wall))
        {
            wall.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 2) ถ้าโดน Base (PlayerBase หรือ EnemyBase)
        if (other.TryGetComponent<BaseHealth>(out var baseHealth))
        {
            baseHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 3) ถ้าโดนผู้เล่น
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>()?.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }
}
