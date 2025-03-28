using UnityEngine;

public class BaseHQ : MonoBehaviour
{
    public GameObject explosionEffect; // ใส่เอฟเฟกต์ระเบิด

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet")) // ตรวจจับกระสุนศัตรู
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
        Debug.Log("Base Destroyed! Game Over!");
        Destroy(gameObject);
    }
}
