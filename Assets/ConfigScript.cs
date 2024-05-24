using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ConfigScript : MonoBehaviour
{
    private bool isPaused = false;
    public AudioMixer masterVolume;
    public AudioMixer effectsVolume;
    public Slider mainVolumeSlider;
    public Slider effectsSlider;
    public GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        mainVolumeSlider.value = PlayerPrefs.GetFloat("MainVolume", 100f);
        effectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume", 100f);
        SetMainVolume(mainVolumeSlider.value);
        SetEffectsVolume(effectsSlider.value);

        mainVolumeSlider.onValueChanged.AddListener(SetMainVolume);
        effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
    }

    void OnMouseDown()
    {
        TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        pauseMenu.SetActive(isPaused);
    }

    public void SetMainVolume(float volume)
    {
        float dB = (volume > 0 ? Mathf.Log10(volume) * 20 - 20 : -80f);
        masterVolume.SetFloat("MasterVol", dB);

        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetEffectsVolume(float volume)
    {
        float dB = (volume > 0 ? Mathf.Log10(volume) * 20 : -80f);
        effectsVolume.SetFloat("EffectsVol", dB);

        PlayerPrefs.SetFloat("EffectsVolume", volume);
    }
}
