using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicChangeHandler : MonoBehaviour
{
    float volumeMusic;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI number;

    void Awake()
    {
        volumeMusic = PlayerPrefs.GetFloat("volume_music", 100);
        number.text = volumeMusic.ToString();
        slider.value = volumeMusic / 100;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.NavigationRight())
        {
            volumeMusic += 1;
            if (volumeMusic > 100) volumeMusic = 100;
            number.text = volumeMusic.ToString();
            slider.value = volumeMusic / 100;
            PlayerPrefs.SetFloat("volume_music", volumeMusic);
            PlayerPrefs.Save();
        }
        if (InputManager.Instance.NavigationLeft())
        {
            volumeMusic -= 1;
            if (volumeMusic < 0) volumeMusic = 0;
            print(volumeMusic / 100);
            number.text = volumeMusic.ToString();
            slider.value = volumeMusic / 100;
            PlayerPrefs.SetFloat("volume_music", volumeMusic);
        }
    }
}
