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
    [SerializeField] private GameObject exitDoor;
    [SerializeField] private int totalFragments = 5;

    private bool isGameOver = false;
    private bool isVictory = false;

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

        UpdateFragmentUI(0);
    }

    void Update()
    {
        if (isGameOver || isVictory) return;

        // Show light percentage only
        if (lightDebugText != null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null && player.playerLight != null)
            {
                float percentage =
                    (player.playerLight.intensity / player.maxLightIntensity) * 100f;
                lightDebugText.text = $"Light: {percentage:F0}%";
            }
        }
    }

    public void UpdateFragmentUI(int collected)
    {
        if (fragmentText != null)
            fragmentText.text = $"Fragments: {collected}/{totalFragments}";

        if (collected >= totalFragments && !isVictory)
        {
            TriggerVictory();
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

        if (exitDoor != null)
            exitDoor.SetActive(true);

        if (victoryUI != null)
            victoryUI.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
