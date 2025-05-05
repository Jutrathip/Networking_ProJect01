using System;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;            // ค่าความเสียหายทั่วไป
    [SerializeField] private float damageAmount = 10f;  // ค่าความเสียหายที่ใช้กับ Base หรือ Wall

    private ulong ownerClientId;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 1) Skip ถ้าเป็นกระสุนของคนยิงเอง (Network)
        if (col.attachedRigidbody != null &&
            col.attachedRigidbody.TryGetComponent<NetworkObject>(out var netObj) &&
            netObj.OwnerClientId == ownerClientId)
        {
            return;
        }

        // 2) ถ้าโดน BaseHealth (Player/Enemy Base)
        if (col.TryGetComponent<BaseHealth>(out var baseHealth))
        {
            baseHealth.TakeDamage(damageAmount);
            Destroy(gameObject);
            return;
        }

        // 3) ถ้าโดน Enemy
        if (col.TryGetComponent<EnemyHealth>(out var enemy))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 4) ถ้าโดนกำแพงที่ทำลายได้
        if (col.TryGetComponent<DestructibleWall2D>(out var wall))
        {
            wall.TakeDamage(damageAmount);
            Destroy(gameObject);
            return;
        }

        // 5) ถ้าโดน Health ทั่วไป
        if (col.TryGetComponent<Health>(out var health))
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }
}
