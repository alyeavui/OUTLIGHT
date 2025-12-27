using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject victoryUI;
    [SerializeField] private TextMeshProUGUI fragmentText;
    [SerializeField] private TextMeshProUGUI lightDebugText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private GameObject exitDoor;
    [SerializeField] private int totalFragments = 5;

    private bool isGameOver = false;
    private bool isVictory = false;
    private bool exitDoorUnlocked = false;

    void Start()
    {
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (victoryUI != null) victoryUI.SetActive(false);
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

    void Update()
    {
        if (isGameOver || isVictory) return;

        if (lightDebugText != null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
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
        {
            UnlockExitDoor();
        }
    }

    private void UnlockExitDoor()
    {
        exitDoorUnlocked = true;

        if (exitDoor != null)
        {
            exitDoor.SetActive(true);
            Debug.Log("Exit door unlocked! Find the exit to complete the level.");
        }
    }

    public void OnPlayerReachedExit()
    {
        if (exitDoorUnlocked && !isVictory)
        {
            TriggerVictory();
        }
        else if (!exitDoorUnlocked)
        {
            Debug.Log("Exit is locked! Collect all fragments first.");
        }
    }

    public void PlayerDied()
    {
        TriggerGameOver("You died!");
    }

    public void TriggerGameOver(string reason = "")
    {
        if (isGameOver || isVictory) return;

        isGameOver = true;

        if (!string.IsNullOrEmpty(reason))
            Debug.Log($"Game Over: {reason}");

        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }

    private void TriggerVictory()
    {
        isVictory = true;

        if (victoryUI != null)
            victoryUI.SetActive(true);

        Time.timeScale = 0f;
        
        Debug.Log("Victory! Level completed!");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Level1")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (currentSceneName == "Level2")
        {
            SceneManager.LoadScene("Level3");
        }
    }
}