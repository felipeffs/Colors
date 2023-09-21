using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeChangeHandler : MonoBehaviour
{
    private float volumeSound;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI number;

    void Awake()
    {
        volumeSound = PlayerPrefs.GetFloat("volume_sound", 100);
        number.text = volumeSound.ToString();
        slider.value = volumeSound;
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
        volumeSound += value;
        if (volumeSound > 100) volumeSound = 100;
        if (volumeSound < 0) volumeSound = 0;
        number.text = volumeSound.ToString();
        slider.value = volumeSound;
        PlayerPrefs.SetFloat("volume_sound", volumeSound);
        PlayerPrefs.Save();
    }

    private void Slider_OnValueChange(float value)
    {
        volumeSound = value;
        number.text = volumeSound.ToString();
        PlayerPrefs.SetFloat("volume_sound", volumeSound);
        PlayerPrefs.Save();
    }
}
