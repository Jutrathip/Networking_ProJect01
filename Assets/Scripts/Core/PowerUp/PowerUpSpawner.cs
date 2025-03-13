using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab; // Power-up ที่ต้องการ Spawn
    public Transform[] spawnPoints; // จุดที่สามารถเกิด Power-up ได้
    public float spawnInterval = 15f; // ความถี่ในการ Spawn
    public float startDelay = 30f; // รอ 30 วิ ก่อนเริ่ม Spawn

    private void Start()
    {
        StartCoroutine(SpawnPowerUps());
    }

    IEnumerator SpawnPowerUps()
    {
        yield return new WaitForSeconds(startDelay); // รอ 30 วินาทีแรก
        while (true)
        {
            SpawnPowerUp();
            yield return new WaitForSeconds(spawnInterval); // Spawn ทุก 15 วิ
        }
    }

    void SpawnPowerUp()
    {
        if (spawnPoints.Length == 0 || powerUpPrefab == null) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(powerUpPrefab, spawnPoint.position, Quaternion.identity);
    }
}
