using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeChangeHandler : MonoBehaviour
{
    int volumeSound;

    void Awake()
    {
        volumeSound = PlayerPrefs.GetInt("volume_sound", 100);
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
            volumeSound += 1;
            if (volumeSound > 100) volumeSound = 100;
            PlayerPrefs.SetInt("volume_sound", volumeSound);
            PlayerPrefs.Save();
            print(volumeSound);
        }
        if (InputManager.Instance.NavigationLeft())
        {
            volumeSound -= 1;
            if (volumeSound < 0) volumeSound = 0;
            PlayerPrefs.SetInt("volume_sound", volumeSound);
            print(volumeSound);
        }
    }
}
