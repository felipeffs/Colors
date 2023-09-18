using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayModeHandler : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (InputManager.Instance.NavigationRight())
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        if (InputManager.Instance.NavigationLeft())
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }

    }
}
