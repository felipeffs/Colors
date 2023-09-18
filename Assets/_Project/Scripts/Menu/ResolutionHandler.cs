using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ResolutionHandler : MonoBehaviour
{
    private Resolution[] suportedResolutions;
    [SerializeField] TextMeshProUGUI resolutionDisplayer;
    int currentResolution = 0;

    void Start()
    {
        suportedResolutions = Screen.resolutions;
        //suportedResolutions.UnorderedSearchFilter<Resolution> Screen.currentResolution
    }

    void Update()
    {
        if (InputManager.Instance.NavigationRight())
        {
            currentResolution += 1;
            if (currentResolution > suportedResolutions.Length) currentResolution = 0;
            Screen.SetResolution(suportedResolutions[currentResolution].width, suportedResolutions[currentResolution].height, Screen.fullScreenMode);
            resolutionDisplayer.text = suportedResolutions[currentResolution].ToString();
        }
        if (InputManager.Instance.NavigationLeft())
        {
            currentResolution -= 1;
            if (currentResolution < 0) currentResolution = suportedResolutions.Length - 1;
            Screen.SetResolution(suportedResolutions[currentResolution].width, suportedResolutions[currentResolution].height, Screen.fullScreenMode);
            resolutionDisplayer.text = suportedResolutions[currentResolution].ToString();
        }
    }
}
