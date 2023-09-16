using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayModeHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
