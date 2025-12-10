using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private float startTime = 30f;  
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject victoryUI;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI fragmentText;
    [SerializeField] private TextMeshProUGUI lightDebugText;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject exitDoor;
    [SerializeField] private int totalFragments = 5;
    private float currentTime;
    private bool isGameOver = false;
    private bool isVictory = false;
    
    void Start()
    {
        currentTime = startTime;
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
        
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(currentTime, 0);
        }
        
        if (timerText != null) 
            timerText.text = Mathf.Ceil(currentTime).ToString();

        if (lightDebugText != null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null && player.playerLight != null)
            {
                float percentage = (player.playerLight.intensity / player.maxLightIntensity) * 100f;
                lightDebugText.text = $"Light: {percentage:F0}%";
            }
        }
        
        if (currentTime <= 0f && !isGameOver)
        {
            TriggerGameOver();
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
    
    private void TriggerGameOver()
    {
        isGameOver = true;
        if (gameOverUI != null) gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void PlayerDied()
    {
        if (!isGameOver)
            TriggerGameOver();
    }
    
    public void TriggerGameOver(string reason = "")
    {
        if (isGameOver) return;
        
        isGameOver = true;
        if (!string.IsNullOrEmpty(reason))
            Debug.Log($"Game Over: {reason}");
        
        if (gameOverUI != null) gameOverUI.SetActive(true);
        Time.timeScale = 0f;
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