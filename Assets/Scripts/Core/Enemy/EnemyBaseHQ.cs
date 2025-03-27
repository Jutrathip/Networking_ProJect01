using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBaseHQ : MonoBehaviour
{
    public GameObject explosionEffect;
    public string nextSceneName; // ✅ ใส่ชื่อ Scene ที่ต้องการไป

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet")) 
        {
            DestroyBase();
        }
    }

    void DestroyBase()
    {
        if (explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Debug.Log("Base Destroyed! Moving to Scene: " + nextSceneName);
        Destroy(gameObject);

        LoadNextScene();
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName)) // ✅ เช็คว่ามีชื่อ Scene ไหม
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("No next scene assigned!");
        }
    }
}
