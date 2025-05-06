using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] public int damage = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out DestructibleWall2D wall))
        {
            wall.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (other.TryGetComponent(out BaseHealth baseHealth))
        {
            baseHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (other.CompareTag("Player"))
        {
            // สมมติ Player มี Health script
            other.GetComponent<Health>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}