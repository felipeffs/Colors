using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeHandler : MonoBehaviour
{
    int volumeMusic;

    void Awake()
    {
        volumeMusic = PlayerPrefs.GetInt("volume_music", 100);
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
            PlayerPrefs.SetInt("volume_music", volumeMusic);
            PlayerPrefs.Save();
            print(volumeMusic);
        }
        if (InputManager.Instance.NavigationLeft())
        {
            volumeMusic -= 1;
            if (volumeMusic < 0) volumeMusic = 0;
            PlayerPrefs.SetInt("volume_music", volumeMusic);
            print(volumeMusic);
        }
    }
}
