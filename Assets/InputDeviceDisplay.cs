using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputDeviceDisplay : MonoBehaviour
{
    [SerializeField] Image mouseKeyboardHud;
    [SerializeField] Image gamepadHud;

    void Awake()
    {
        mouseKeyboardHud = transform.GetChild(0).GetComponent<Image>();
        gamepadHud = transform.GetChild(1).GetComponent<Image>();
    }

    void OnEnable()
    {
        InputManager.Instance.OnControlTypeChange += InputManager_OnControlTypeChange;
    }

    void OnDisable()
    {
        InputManager.Instance.OnControlTypeChange -= InputManager_OnControlTypeChange;
    }

    private void InputManager_OnControlTypeChange(InputManager.ControlType type)
    {
        if (type == InputManager.ControlType.KeyboardMouse)
        {
            mouseKeyboardHud.enabled = true;
            gamepadHud.enabled = false;
        }
        else
        {
            mouseKeyboardHud.enabled = false;
            gamepadHud.enabled = true;
        }
    }
}
