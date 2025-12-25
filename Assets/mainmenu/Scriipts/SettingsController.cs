using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicSlider.value = savedVolume;

        SetVolume(savedVolume);
    }

    public void OnVolumeChanged()
    {
        float volume = musicSlider.value;
        SetVolume(volume);

        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    private void SetVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }
}

