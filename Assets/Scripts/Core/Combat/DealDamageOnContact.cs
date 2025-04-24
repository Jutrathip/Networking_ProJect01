using System;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    [SerializeField] private float damageAmount = 10f;

    private ulong ownerClientId;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.attachedRigidbody == null) {return;}

        if (col.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            if (ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }
        
        if (col.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }

        if (col.TryGetComponent(out DestructibleWall2D wall))
        {
            wall.TakeDamage(damageAmount); // กำหนดค่า damageAmount ในกระสุน
        }

        if (col.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (col.TryGetComponent(out BaseHealth baseHealth))
        {
            baseHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
