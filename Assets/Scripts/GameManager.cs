using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private PlayerInputHandler playerInputHandler;

    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;
    [SerializeField] private ObjectPool meteorPool;
    public ObjectPool MeteorPool => meteorPool;
    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            playerInputHandler.disableInput();
        }
        else
        {
            playerInputHandler.enableInput();
        }
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }
}
