using UnityEngine;

public class Fragment : MonoBehaviour
{  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                player.CollectFragment();
                Destroy(gameObject);
            }
        }
    }
}