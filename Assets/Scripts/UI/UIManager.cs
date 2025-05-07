// Scripts/UI/UIManager.cs
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("ลาก Panel จาก Scene หรือ Prefab ลง Inspector")]
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;

    private Canvas _mainCanvas;
    private GameObject _gameOverInstance;
    private GameObject _gameWinInstance;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        // หา Canvas ทีละเครื่อง (Host+Client)
        _mainCanvas = FindObjectOfType<Canvas>();
        if (_mainCanvas == null)
            Debug.LogError("UIManager: ไม่พบ Canvas ใน Scene");

        // สปอนน์ Panel ลงใต้ Canvas แล้วซ่อนไว้
        if (gameOverPanel != null && _mainCanvas != null)
        {
            _gameOverInstance = Instantiate(gameOverPanel, _mainCanvas.transform, false);
            _gameOverInstance.SetActive(false);
        }
        if (gameWinPanel != null && _mainCanvas != null)
        {
            _gameWinInstance = Instantiate(gameWinPanel, _mainCanvas.transform, false);
            _gameWinInstance.SetActive(false);
        }
    }

    /// <summary>
    /// เรียกบน Server เพื่อสั่งทุกคนให้โชว์ Game Over
    /// </summary>
    public void ShowGameOver()
    {
        if (!IsServer) return;
        ShowGameOverClientRpc();
    }

    /// <summary>
    /// เรียกบน Server เพื่อสั่งทุกคนให้โชว์ Game Win
    /// </summary>
    public void ShowGameWin()
    {
        if (!IsServer) return;
        ShowGameWinClientRpc();
    }

    [ClientRpc]
    private void ShowGameOverClientRpc()
    {
        Time.timeScale = 0f;
        if (_gameOverInstance != null)
            _gameOverInstance.SetActive(true);
    }

    [ClientRpc]
    private void ShowGameWinClientRpc()
    {
        Time.timeScale = 0f;
        if (_gameWinInstance != null)
            _gameWinInstance.SetActive(true);
    }
}
