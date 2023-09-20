using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ResolutionHandler : MonoBehaviour
{
    private Resolution[] suportedResolutions;
    [SerializeField] TextMeshProUGUI resolutionDisplayer;
    int currentIndex = 0;

    void Awake()
    {
        suportedResolutions = Screen.resolutions;
        var currentRes = Screen.currentResolution;
        currentIndex = Array.FindIndex(suportedResolutions, res =>
        {
            if (res.width != currentRes.width)
                return false;
            if (res.height != currentRes.height)
                return false;
            if (res.refreshRate != currentRes.refreshRate)
                return false;
            return true;
        }
        );
        resolutionDisplayer.text = suportedResolutions[currentIndex].ToString();
    }

    void Update()
    {
        if (InputManager.Instance.NavigationRight())
        {
            ChangeResolution(+1);
        }
        if (InputManager.Instance.NavigationLeft())
        {
            ChangeResolution(-1);
        }
    }

    private void ChangeResolution(int direction)
    {
        currentIndex += 1 * direction;
        if (currentIndex > suportedResolutions.Length - 1) currentIndex = 0;

        if (currentIndex < 0) currentIndex = suportedResolutions.Length - 1;

        Screen.SetResolution(suportedResolutions[currentIndex].width, suportedResolutions[currentIndex].height, Screen.fullScreenMode);
        resolutionDisplayer.text = suportedResolutions[currentIndex].ToString();
    }
}
