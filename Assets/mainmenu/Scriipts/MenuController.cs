using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject settingsPanel;
    
    public void Start()
    {
        settingsPanel.SetActive(false);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Level1"); 
    }
    public void Back()
    {
        SceneManager.LoadScene("Main menu");
    }

    public void Settings()
    {
        if (settingsPanel.activeSelf == false)
        {
            settingsPanel.SetActive(true);
        }
        else if (settingsPanel.activeSelf == true)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exit pressed");
        Application.Quit();
    }
}

