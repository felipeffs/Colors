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
            ChangeDisplayMode(false);
        }
        if (InputManager.Instance.NavigationRight())
        {
            ChangeDisplayMode(true);
        }
    }

    private void HoverFullscreen(bool isFullscreen)
    {
        selectedFullscreen.SetActive(isFullscreen);
        selectedWindowed.SetActive(!isFullscreen);
    }

    public void ChangeDisplayMode(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        HoverFullscreen(isFullscreen);
    }
}
