using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip fragmentPickupSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayFragmentSound()
    {
        if (fragmentPickupSound != null)
        {
            sfxSource.PlayOneShot(fragmentPickupSound);
        }
    }
}

