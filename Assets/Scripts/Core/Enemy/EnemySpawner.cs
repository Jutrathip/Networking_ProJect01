using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab ของ Enemy ที่จะ Spawn")]
    public GameObject enemyPrefab;

    [Header("ตำแหน่งที่จะใช้ Spawn (หากปล่อยว่าง จะ Spawn ที่ตำแหน่ง Spawner)")]
    public Transform[] spawnPoints;

    [Header("เวลาที่จะเว้นระหว่างการ Spawn แต่ละครั้ง (วินาที)")]
    public float spawnInterval = 5f;

    void Start()
    {
        // เริ่ม Coroutine ให้มันวน Spawn เป็นระยะ ๆ
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // รอให้เกมโหลดฉาก และสั่ง Spawn ครั้งแรก (ไม่บังคับ)
        yield return new WaitForSeconds(1f);

        while (true)
        {
            SpawnAllEnemies();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// สร้าง Enemy ที่ทุกตำแหน่งใน spawnPoints
    /// </summary>
    void SpawnAllEnemies()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            // วนลูปทุกจุด
            foreach (var point in spawnPoints)
            {
                Instantiate(
                    enemyPrefab,
                    point.position,
                    point.rotation
                );
            }
        }
        else
        {
            // ถ้าไม่มี spawnPoints ให้ Spawn ที่ตำแหน่งของ Spawner แทน
            Instantiate(
                enemyPrefab,
                transform.position,
                transform.rotation
            );
        }
    }
}
