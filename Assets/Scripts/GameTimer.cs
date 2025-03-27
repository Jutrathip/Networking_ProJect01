using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 60f; // ⏳ ตั้งเวลาถอยหลัง (60 วินาที)
    public TMP_Text timerText; // ตัวแสดงผลเวลา
    public GameObject gameOverCanvas; // Canvas ที่ใช้ Restart เกม
    public Button restartButton; // ปุ่ม Restart

    private bool isGameOver = false;

    void Start()
    {
        gameOverCanvas.SetActive(false); // ซ่อน Canvas ตอนเริ่มเกม
        restartButton.onClick.AddListener(RestartGame); // ผูกปุ่ม Restart
        UpdateTimerUI(); // อัพเดต UI ตอนเริ่มเกม
    }

    void Update()
    {
        if (isGameOver) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timeRemaining = 0; // ป้องกันค่าติดลบ
            UpdateTimerUI();
            EndGame();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f; // ✅ หยุดเกม
        gameOverCanvas.SetActive(true); // ✅ แสดง Canvas เมื่อหมดเวลา
        Debug.Log("Time's Up! Game Paused.");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // ✅ รีเซ็ตเวลาให้กลับมาปกติ
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // โหลดฉากใหม่
    }
}
