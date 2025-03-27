using UnityEngine;
using Unity.Netcode;

public class EnemyBullet : NetworkBehaviour
{
    public float lifetime = 3f;
    public int damage = 10;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.CompareTag("Player") || collision.CompareTag("Base"))
        {
            if (collision.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
