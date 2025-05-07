// Scripts/Core/BaseHealth.cs
using UnityEngine;
using Unity.Netcode;

public class BaseHealth : NetworkBehaviour
{
    public enum BaseType { Player, Enemy }
    public BaseType baseType;

    [Header("ลาก Prefab หรือ Scene Panel ลง Inspector")]
    public GameObject gameOverPanelPrefab;
    public GameObject gameWinPanelPrefab;

    [Header("ค่าพลังชีวิตสูงสุด")]
    public float maxHealth = 50f;

    // Server เป็นผู้เขียนค่าพลังชีวิตเท่านั้น
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        /* initialValue */        0f,
        /* readPerm */            NetworkVariableReadPermission.Everyone,
        /* writePerm */           NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // บน Server ตั้งค่าเริ่มต้น
            currentHealth.Value = maxHealth;
        }
        // Panels จะ instantiate ใน UIManager
    }

    /// <summary>
    /// External calls (เช่น จากกระสุน) ให้เรียกเมธอดนี้เสมอ
    /// </summary>
    public void TakeDamage(float damage)
    {
        // ให้ Server เป็นคนคำนวณเท่านั้น
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
        {
            HandleDie();
        }
    }

    private void HandleDie()
    {
        // บน Server สั่งให้ UIManager แสดง
        if (UIManager.Instance != null)
        {
            if (baseType == BaseType.Player)
                UIManager.Instance.ShowGameOver();
            else
                UIManager.Instance.ShowGameWin();
        }
        // Despawn ตัวฐานบน Server → replicate ไปทุก Client
        GetComponent<NetworkObject>().Despawn(destroy: true);
    }
}
