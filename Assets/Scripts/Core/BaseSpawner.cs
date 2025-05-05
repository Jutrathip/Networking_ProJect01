using UnityEngine;

public class BaseSpawner : MonoBehaviour
{
    [Header("Prefab ของฐาน")]
    public GameObject playerBasePrefab;
    public GameObject enemyBasePrefab;

    [Header("ตำแหน่งสุ่มของ Player Base")]
    public Transform[] playerSpawnPoints;

    [Header("ตำแหน่งสุ่มของ Enemy Base")]
    public Transform[] enemySpawnPoints;

    void Start()
    {
        // สปอนน์ Player Base
        SpawnOne(playerBasePrefab, playerSpawnPoints);

        // สปอนน์ Enemy Base
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

        Instantiate(prefab, chosen.position, chosen.rotation);
    }
}
