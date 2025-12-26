using UnityEngine;

public class Fragment : MonoBehaviour
{  
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                collected = true;
                player.CollectFragment(gameObject.GetInstanceID());
                Destroy(gameObject);
            }
        }
    }
}