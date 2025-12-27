using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private UI uiManager;

    void Start()
    {
        uiManager = FindFirstObjectByType<UI>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (uiManager != null)
            {
                uiManager.OnPlayerReachedExit();
            }
        }
    }
}