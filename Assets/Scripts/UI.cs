using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private float startTime = 30f;  
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI timerText; 
    [SerializeField] private Button restartButton; 

    private float currentTime;
    private bool isGameOver = false;

    void Start()
    {
        currentTime = startTime;
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartLevel);
        }
    }

    void Update()
    {
        if (isGameOver) return;
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(currentTime, 0);
        }
        if (timerText != null) timerText.text = Mathf.Ceil(currentTime).ToString();
        if (currentTime <= 0f && !isGameOver)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        if (gameOverUI != null) gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
