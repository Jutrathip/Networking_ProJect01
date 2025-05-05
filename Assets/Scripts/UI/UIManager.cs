using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("ลาก Panel จาก Scene ใส่ใน Inspector")]
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        gameWinPanel.SetActive(false);
    }

    public void ShowGameOver() => gameOverPanel.SetActive(true);
    public void ShowGameWin()  => gameWinPanel .SetActive(true);
}
