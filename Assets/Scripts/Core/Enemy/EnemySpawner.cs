using UnityEngine;
using Unity.Netcode;
using System.Collections;

[RequireComponent(typeof(NetworkObject))]
public class EnemySpawner : NetworkBehaviour
{
    [Header("Prefab ของ Enemy ที่จะ Spawn (ต้องมี NetworkObject)")]
    public GameObject enemyPrefab;

    [Header("ตำแหน่งที่จะใช้ Spawn (หากปล่อยว่าง จะ Spawn ที่ตำแหน่ง Spawner)")]
    public Transform[] spawnPoints;

    [Header("เวลาที่จะเว้นระหว่างการ Spawn แต่ละครั้ง (วินาที)")]
    public float spawnInterval = 5f;

    public override void OnNetworkSpawn()
    {
        // ให้ Server/Host เริ่ม Coroutine สปอนน์ศัตรู
        if (!IsServer) return;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // รอ 1 วินาที ก่อนสปอนน์ครั้งแรก
        yield return new WaitForSeconds(1f);

        while (true)
        {
            SpawnAllEnemies();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnAllEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("ไม่มี Prefab ของ Enemy กำหนดใน Inspector");
            return;
        }

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            foreach (var point in spawnPoints)
            {
                SpawnEnemyAt(point.position, point.rotation);
            }
        }
        else
        {
            // ไม่มีจุดสปอนน์ ให้ใช้ตำแหน่งของตัว Spawner
            SpawnEnemyAt(transform.position, transform.rotation);
        }
    }

    private void SpawnEnemyAt(Vector3 position, Quaternion rotation)
    {
        // สร้างบน Server
        var instance = Instantiate(enemyPrefab, position, rotation);
        var netObj = instance.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();  // replicate ให้ Clients ทราบ
        }
        else
        {
            Debug.LogError($"Enemy prefab '{enemyPrefab.name}' ไม่มีคอมโพเนนต์ NetworkObject!");
            Destroy(instance);
        }
    }
}
