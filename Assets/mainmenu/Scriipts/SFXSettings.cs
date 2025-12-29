using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SFXSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider sfxSlider;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    public void OnSFXVolumeChanged()
    {
        float volume = sfxSlider.value;
        SetVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void SetVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}
