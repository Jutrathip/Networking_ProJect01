using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class BaseSpawner : NetworkBehaviour
{
    [Header("Prefab ของฐาน (ต้องมี NetworkObject)")]
    public GameObject playerBasePrefab;
    public GameObject enemyBasePrefab;

    [Header("ตำแหน่งสุ่มของ Player Base")]
    public Transform[] playerSpawnPoints;

    [Header("ตำแหน่งสุ่มของ Enemy Base")]
    public Transform[] enemySpawnPoints;

    public override void OnNetworkSpawn()
    {
        // ให้รันเฉพาะฝั่ง Server (รวม Host) เท่านั้น
        if (!IsServer) return;

        SpawnOne(playerBasePrefab, playerSpawnPoints);
        SpawnOne(enemyBasePrefab, enemySpawnPoints);
    }

    private void SpawnOne(GameObject prefab, Transform[] points)
    {
        if (prefab == null || points == null || points.Length == 0)
        {
            Debug.LogWarning($"ไม่มี Prefab หรือ Spawn Points ของ {prefab?.name ?? "Unknown"}");
            return;
        }

        int idx = Random.Range(0, points.Length);
        Transform chosen = points[idx];

        // สร้างบน Server
        GameObject instance = Instantiate(prefab, chosen.position, chosen.rotation);

        // สั่ง Netcode spawn ให้ทุก Client เห็นด้วย
        var netObj = instance.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
        else
        {
            Debug.LogError($"Prefab {prefab.name} ไม่มีคอมโพเนนต์ NetworkObject!");
        }
    }
}
