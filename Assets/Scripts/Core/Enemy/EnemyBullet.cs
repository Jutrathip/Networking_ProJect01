// EnemyBullet.cs
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class EnemyBullet : NetworkBehaviour
{
    [Header("ความเสียหายต่อเป้าหมาย")]
    [SerializeField] private int damage = 10;

    [Header("Prefab ของ Particle System ที่จะแสดงตอนถูกทำลาย")]
    [SerializeField] private GameObject destroyEffectPrefab;

    private NetworkObject netObj;

    private void Awake()
    {
        netObj = GetComponent<NetworkObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ให้เฉพาะ Server (Host) ประมวลผลการชนและทำลายกระสุน
        if (!IsServer) return;

        bool didHit = false;

        // 1) พังกำแพง
        if (other.TryGetComponent<DestructibleWall2D>(out var wall))
        {
            wall.TakeDamage(damage);
            didHit = true;
        }
        // 2) พังฐาน
        else if (other.TryGetComponent<BaseHealth>(out var baseHealth))
        {
            baseHealth.TakeDamage(damage);
            didHit = true;
        }
        // 3) โจมตีผู้เล่น
        else if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>()?.TakeDamage(damage);
            didHit = true;
        }

        if (!didHit) return;

        // สั่งสร้าง Particle Effect บนทุก Client (รวม Host)
        SpawnDestroyEffectClientRpc(transform.position);

        // Server/Host สั่ง despawn กระสุน (replicate ไป Client)
        if (netObj.IsSpawned)
            netObj.Despawn(destroy: true);
    }

    [ClientRpc]
    private void SpawnDestroyEffectClientRpc(Vector3 position)
    {
        if (destroyEffectPrefab == null) return;

        var effect = Instantiate(destroyEffectPrefab, position, Quaternion.identity);
        if (effect.TryGetComponent<ParticleSystem>(out var ps))
            Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
        else
            Destroy(effect, 5f);
    }
}
