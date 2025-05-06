using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("ความเสียหายต่อเป้าหมาย")]
    [SerializeField] private int damage = 10;

    [Header("Prefab ของ Particle System ที่จะแสดงตอนถูกทำลาย")]
    [SerializeField] private GameObject destroyEffectPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1) พังกำแพงก่อน
        if (other.TryGetComponent<DestructibleWall2D>(out var wall))
        {
            wall.TakeDamage(damage);
            PlayDestroyEffect();
            Destroy(gameObject);
            return;
        }

        // 2) ถ้าเป็น Base (PlayerBase/EnemyBase)
        if (other.TryGetComponent<BaseHealth>(out var baseHealth))
        {
            baseHealth.TakeDamage(damage);
            PlayDestroyEffect();
            Destroy(gameObject);
            return;
        }

        // 3) ถ้าเป็นผู้เล่น
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>()?.TakeDamage(damage);
            PlayDestroyEffect();
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// สร้าง Particle Effect และทำลายตัวมันเองหลังเล่นจบ
    /// </summary>
    private void PlayDestroyEffect()
    {
        if (destroyEffectPrefab != null)
        {
            // Instantiate เอฟเฟกต์ที่ตำแหน่งกระสุนปัจจุบัน
            GameObject effect = Instantiate(
                destroyEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            // ถ้าเป็น ParticleSystem ทั้งระบบ ให้ลบตัวเอฟเฟกต์หลังจบ
            if (effect.TryGetComponent<ParticleSystem>(out var ps))
            {
                Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                // ถ้าเป็น GameObject ที่มี child เป็น ParticleSystem หลายตัว
                Destroy(effect, 5f); // กำหนดลบหลัง 5 วินาที (ปรับได้ตามต้องการ)
            }
        }
    }
}
