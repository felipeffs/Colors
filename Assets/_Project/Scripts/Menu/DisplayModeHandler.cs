using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayModeHandler : MonoBehaviour
{
    [SerializeField] private GameObject selectedWindowed;
    [SerializeField] private GameObject selectedFullscreen;

    void Start()
    {
        HoverFullscreen(Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen);
    }

    void Update()
    {
        if (InputManager.Instance.NavigationLeft())
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            HoverFullscreen(false);
        }
        if (InputManager.Instance.NavigationRight())
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            HoverFullscreen(true);
        }
    }

    private void HoverFullscreen(bool isFullscreen)
    {
        selectedFullscreen.SetActive(isFullscreen);
        selectedWindowed.SetActive(!isFullscreen);
    }
}
