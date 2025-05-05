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

        // วนไม่รู้จบ (คุณสามารถปรับเงื่อนไขให้หยุดได้)
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        // เลือกตำแหน่งสุ่ม (ถ้ามีหลายจุด)
        Vector3 spawnPos;
        Quaternion spawnRot = Quaternion.identity;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int idx = Random.Range(0, spawnPoints.Length);
            spawnPos = spawnPoints[idx].position;
            spawnRot = spawnPoints[idx].rotation;
        }
        else
        {
            // ไม่มีจุด Spawn เพิ่ม เติม ก็ใช้ตำแหน่งของ Spawner
            spawnPos = transform.position;
            spawnRot = transform.rotation;
        }

        // สร้าง Enemy ขึ้นมา
        Instantiate(enemyPrefab, spawnPos, spawnRot);
    }
}
