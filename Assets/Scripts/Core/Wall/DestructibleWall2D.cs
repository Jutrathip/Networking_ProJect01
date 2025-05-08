using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class DestructibleWall2D : NetworkBehaviour
{
    [Header("ค่าพลังชีวิตสูงสุดของกำแพง")]
    [SerializeField] private float maxHealth = 50f;

    // Server เป็นผู้เขียนค่าเดียว
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
    }

    /// <summary>
    /// เรียกเพื่อหักเลือด กำแพงจะถูกทำลายทั้งฝั่ง Host/Client
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (IsServer)
        {
            ApplyDamage(damage);
        }
        else
        {
            TakeDamageServerRpc(damage);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(float damage)
    {
        ApplyDamage(damage);
    }

    private void ApplyDamage(float damage)
    {
        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0f)
        {
            // ถ้าเป็นวัตถุ Netcode (Spawned) ให้ใช้ Despawn
            var netObj = GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned)
            {
                netObj.Despawn(destroy: true);
            }
            else
            {
                // มิฉะนั้นก็ใช้ Destroy ธรรมดา
                Destroy(gameObject);
            }
        }
    }
}
