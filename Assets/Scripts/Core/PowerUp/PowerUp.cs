using UnityEngine;
using System.Collections; // ✅ ต้องเพิ่มบรรทัดนี้เพื่อใช้ IEnumerator

public class PowerUp : MonoBehaviour
{
    public float duration = 5f; // ระยะเวลาที่ Power-up มีผล

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // ถ้า Player เก็บ Power-up
        {
            StartCoroutine(ApplyEffect(collision.gameObject));
            Destroy(gameObject); // ทำลาย Power-up หลังถูกเก็บ
        }
    }

    IEnumerator ApplyEffect(GameObject player)
    {
        Debug.Log("Power-up Activated!"); // สามารถเพิ่มเอฟเฟกต์พิเศษที่นี่
        yield return new WaitForSeconds(duration);
        Debug.Log("Power-up Ended!");
    }
}