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
                //ВОТ ЗДЕСЬ ЗВУК
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlayFragmentSound();
                player.CollectFragment(gameObject.GetInstanceID());
                Destroy(gameObject);
            }
        }
    }
}