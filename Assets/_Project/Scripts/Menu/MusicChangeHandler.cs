using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicChangeHandler : MonoBehaviour
{
    private float volumeMusic;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI number;

    void Awake()
    {
        volumeMusic = PlayerPrefs.GetFloat("volume_music", 100);
        number.text = volumeMusic.ToString();
        slider.value = volumeMusic;
        slider.onValueChanged.AddListener(Slider_OnValueChange);
    }

    void OnEnable()
    {
        slider.enabled = true;
    }

    void OnDisable()
    {
        slider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.NavigationRight())
        {
            ChangeVolume(1);
        }
        if (InputManager.Instance.NavigationLeft())
        {
            ChangeVolume(-1);
        }
    }

    private void ChangeVolume(int value)
    {
        volumeMusic += value;
        if (volumeMusic > 100) volumeMusic = 100;
        if (volumeMusic < 0) volumeMusic = 0;
        number.text = volumeMusic.ToString();
        slider.value = volumeMusic;
        PlayerPrefs.SetFloat("volume_music", volumeMusic);
        PlayerPrefs.Save();
    }

    private void Slider_OnValueChange(float value)
    {
        volumeMusic = value;
        number.text = volumeMusic.ToString();
        PlayerPrefs.SetFloat("volume_sound", volumeMusic);
        PlayerPrefs.Save();
    }
}
