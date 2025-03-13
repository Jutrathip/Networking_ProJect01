using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject destroyEffect; // เอฟเฟกต์ตอนกำแพงถูกทำลาย

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet")) // ตรวจจับกระสุนของ Player
        {
            DestroyWall();
            Destroy(collision.gameObject); // ทำลายกระสุน
        }
    }

    void DestroyWall()
    {
        if (destroyEffect)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity); // สร้างเอฟเฟกต์ระเบิด
        }
        Destroy(gameObject); // ทำลายกำแพง
    }
}
