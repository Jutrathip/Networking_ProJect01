// EnemyHealth.cs
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class EnemyHealth : NetworkBehaviour
{
    [Header("ค่าพลังชีวิตสูงสุด")]
    [SerializeField] private float maxHealth = 30f;

    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            currentHealth.Value = maxHealth;
    }

    /// <summary>เรียกเพื่อทำดาเมจ</summary>
    public void TakeDamage(float damage)
    {
        if (IsServer)
            ApplyDamage(damage);
        else
            TakeDamageServerRpc(damage);
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
            Die();
    }

    private void Die()
    {
        // Server/Host เป็นคนเดียวที่ despawn
        GetComponent<NetworkObject>().Despawn(destroy: true);
        // (เพิ่มเอฟเฟกต์หรือคะแนนได้ที่นี่)
    }
}
