using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class DestructibleWall2D : NetworkBehaviour
{
    [Header("ค่าพลังชีวิตสูงสุดของกำแพง")]
    [SerializeField] private float maxHealth = 50f;

    // เก็บ HP บน Network; Server เขียน, Client อ่าน
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        // บน Server ตั้งค่า HP เริ่มต้น
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
    }

    /// <summary>
    /// เรียกเมื่อต้องการทำดาเมจกำแพง
    /// </summary>
    public void TakeDamage(float damage)
    {
        // ทุกคนเรียก RPC ให้ Server หัก HP
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

    // ทำงานบน Server เท่านั้น
    private void ApplyDamage(float damage)
    {
        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0f)
        {
            // Server/Host เป็นคนเดียวที่ despawn
            GetComponent<NetworkObject>().Despawn(destroy: true);
        }
    }
}
