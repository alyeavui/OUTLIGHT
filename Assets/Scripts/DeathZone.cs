using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UI uiManager = FindFirstObjectByType<UI>();
            if (uiManager != null)
            {
                uiManager.PlayerDied();
            }
            else
            {
                Debug.LogError("DeathZone: UI Manager not found!");
            }
            if (collision.TryGetComponent<PlayerController>(out var player))
            {
                player.enabled = false;
            }
        }
    }
}