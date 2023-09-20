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
        slider.value = volumeSound / 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.NavigationRight())
        {
            volumeSound += 1;
            if (volumeSound > 100) volumeSound = 100;
            number.text = volumeSound.ToString();
            slider.value = volumeSound / 100;
            PlayerPrefs.SetFloat("volume_sound", volumeSound);
            PlayerPrefs.Save();
        }
        if (InputManager.Instance.NavigationLeft())
        {
            volumeSound -= 1;
            if (volumeSound < 0) volumeSound = 0;
            print(volumeSound / 100);
            number.text = volumeSound.ToString();
            slider.value = volumeSound / 100;
            PlayerPrefs.SetFloat("volume_sound", volumeSound);
        }
    }
}
