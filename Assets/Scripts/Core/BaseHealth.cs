using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public enum BaseType { Player, Enemy }
    public BaseType baseType;

    [Header("ลาก Prefab ของ Game Over Panel")]
    public GameObject gameOverPanelPrefab;

    [Header("ลาก Prefab ของ Game Win Panel")]
    public GameObject gameWinPanelPrefab;

    private GameObject gameOverPanelInstance;
    private GameObject gameWinPanelInstance;

    [Header("ค่าพลังชีวิตสูงสุด")]
    public float maxHealth = 50f;
    private float currentHealth;

    void Start()
    {
        // เซ็ตพลังชีวิตเริ่มต้น
        currentHealth = maxHealth;
        Time.timeScale = 1f;

        // หา Canvas แรกใน Scene
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null)
            Debug.LogError("ไม่พบ Canvas ใน Scene กรุณาใส่ Canvas ไว้ใน Hierarchy");

        // สปอนน์ Game Over Panel จาก Prefab ลงใต้ Canvas
        if (gameOverPanelPrefab != null && mainCanvas != null)
        {
            gameOverPanelInstance = Instantiate(
                gameOverPanelPrefab,
                mainCanvas.transform,  // parent ให้เป็น Canvas
                worldPositionStays: false
            );
            // ขยายให้เต็มหน้าจอ
            var rt = gameOverPanelInstance.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.anchoredPosition = Vector2.zero;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            gameOverPanelInstance.SetActive(false);
        }

        // สปอนน์ Game Win Panel จาก Prefab ลงใต้ Canvas
        if (gameWinPanelPrefab != null && mainCanvas != null)
        {
            gameWinPanelInstance = Instantiate(
                gameWinPanelPrefab,
                mainCanvas.transform,
                worldPositionStays: false
            );
            var rt = gameWinPanelInstance.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.anchoredPosition = Vector2.zero;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            gameWinPanelInstance.SetActive(false);
        }
    }

    /// <summary>
    /// เรียกเมื่อต้องการให้ฐานได้รับความเสียหาย
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
            Die();
    }

    /// <summary>
    /// เรียกเมื่อฐานถูกทำลาย
    /// </summary>
    void Die()
    {
        // หยุดเกม
        Time.timeScale = 0f;

        if (baseType == BaseType.Player)
        {
            if (gameOverPanelInstance != null)
                gameOverPanelInstance.SetActive(true);
            else
                Debug.LogWarning("ยังไม่ได้เซ็ต GameOverPanel Prefab ใน Inspector");
        }
        else // Enemy
        {
            if (gameWinPanelInstance != null)
                gameWinPanelInstance.SetActive(true);
            else
                Debug.LogWarning("ยังไม่ได้เซ็ต GameWinPanel Prefab ใน Inspector");
        }

        Destroy(gameObject);
    }
}
