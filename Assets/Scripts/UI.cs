using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI fragmentText;
    [SerializeField] private TextMeshProUGUI lightDebugText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private GameObject exitDoor;
    [SerializeField] private int totalFragments = 5;

    private bool isGameOver = false;
    private bool isVictory = false;
    private bool exitDoorUnlocked = false;

    public static UI Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Time.timeScale = 1f;

        if (inGameUI != null) inGameUI.SetActive(true);
        if (winUI != null) winUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (exitDoor != null) exitDoor.SetActive(false);

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartLevel);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(LoadNextLevel);
        }

        UpdateFragmentUI(0);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        if (isGameOver || isVictory) return;

        if (lightDebugText != null)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null && player.playerLight != null)
            {
                float percentage = (player.playerLight.intensity / player.maxLightIntensity) * 100f;
                lightDebugText.text = $"Light: {percentage:F0}%";
            }
        }
    }

    public void UpdateFragmentUI(int collected)
    {
        if (fragmentText != null)
            fragmentText.text = $"Fragments: {collected}/{totalFragments}";

        if (collected >= totalFragments && !exitDoorUnlocked)
            UnlockExitDoor();
    }

    private void UnlockExitDoor()
    {
        exitDoorUnlocked = true;
        if (exitDoor != null)
            exitDoor.SetActive(true);
    }

    public void OnPlayerReachedExit()
    {
        if (exitDoorUnlocked && !isVictory)
            ShowWinUI();
    }

    public void PlayerDied()
    {
        DeathVideoController deathVideo = DeathVideoController.GetInstance();
        if (deathVideo != null)
        {
            deathVideo.PlayDeathVideo();
        }
        else
        {
            ShowGameOverUI("You died!");
        }
    }

    public void ShowGameOverUI(string reason = "")
    {
        if (isGameOver || isVictory) return;

        isGameOver = true;

        if (inGameUI != null) inGameUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ShowWinUI()
    {
        if (isVictory || isGameOver) return;

        isVictory = true;

        if (inGameUI != null) inGameUI.SetActive(false);
        if (winUI != null) winUI.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isVictory = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isVictory = false;
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentIndex < SceneManager.sceneCountInBuildSettings - 1)
            SceneManager.LoadScene(currentIndex + 1);
    }
}
