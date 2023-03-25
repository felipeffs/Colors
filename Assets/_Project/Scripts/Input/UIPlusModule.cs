using UnityEngine;
using System.Collections.Generic;

public class UIPlusModule : Singleton<UIPlusModule>
{
    [SerializeField] private List<ButtonPlus> allButtons = new List<ButtonPlus>();

    [SerializeField] private ButtonPlus currentSelectedButton;
    [SerializeField] private ButtonPlus initialSelectedButton;
    [SerializeField] private controlType activeControlType = controlType.Mouse;

    enum controlType
    {
        Mouse,
        GameController
    }

    private void Update()
    {
        //Controll type
        if (activeControlType == controlType.GameController)
        {
            if (InputManager.Instance.PointerDelta())
            {
                currentSelectedButton = null;
                Cursor.lockState = CursorLockMode.None;
                activeControlType = controlType.Mouse;
                return;
            }

            if (InputManager.Instance.NavigationUp())
            {
                Cursor.lockState = CursorLockMode.Locked;

                SetSelected(currentSelectedButton.upLink);
            }
            else if (InputManager.Instance.NavigationDown())
            {
                Cursor.lockState = CursorLockMode.Locked;

                SetSelected(currentSelectedButton.downLink);
            }
            else if (InputManager.Instance.NavigationLeft())
            {
                Cursor.lockState = CursorLockMode.Locked;

                SetSelected(currentSelectedButton.leftLink);
            }
            else if (InputManager.Instance.NavigationRight())
            {
                Cursor.lockState = CursorLockMode.Locked;

                SetSelected(currentSelectedButton.rightLink);
            }
        }
        else if (activeControlType == controlType.Mouse)
        {
            if (InputManager.Instance.NavigationUp() || InputManager.Instance.NavigationDown() || InputManager.Instance.NavigationLeft() || InputManager.Instance.NavigationRight())
            {
                activeControlType = controlType.GameController;
                Cursor.lockState = CursorLockMode.Locked;

                SetSelected(initialSelectedButton);
            }
        }
    }

    public void AddButton(ButtonPlus buttonPlus)
    {
        allButtons.Add(buttonPlus);

        ButtonPlus.OnEnterOver += ButtonPlus_OnEnterOver;
        ButtonPlus.OnExitOver += ButtonPlus_OnExitOver;
    }

    public void SetSelected(ButtonPlus buttonPlus)
    {
        if (buttonPlus == null) return;

        if (currentSelectedButton != null)
        {
            currentSelectedButton.SetCurrentState(ButtonPlus.State.Normal);
        }
        buttonPlus.SetCurrentState(ButtonPlus.State.Highlighted);
        currentSelectedButton = buttonPlus;
    }

    public void ButtonPlus_OnEnterOver(ButtonPlus button)
    {
        if (activeControlType == controlType.Mouse)
            currentSelectedButton = button;
    }

    public void ButtonPlus_OnExitOver(ButtonPlus button)
    {
        if (activeControlType == controlType.Mouse)
            currentSelectedButton = null;
    }
}
