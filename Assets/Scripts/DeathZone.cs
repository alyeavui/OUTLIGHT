using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public GameObject gameOverUI;
    public float freezeDelay = 0.1f; 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(TriggerGameOver());
        }
    }

    private System.Collections.IEnumerator TriggerGameOver()
    {
        yield return new WaitForSeconds(freezeDelay);
        if (gameOverUI != null)
            gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }
}
