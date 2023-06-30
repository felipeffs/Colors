using UnityEngine;
using System.Collections.Generic;

public class UIPlusModule : Singleton<UIPlusModule>
{
    private List<ButtonPlus> _allButtons = new List<ButtonPlus>();

    private ButtonPlus _currentSelectedButton;
    [SerializeField] private ButtonPlus initialSelectedButton;
    private ControlType _activeControlType = ControlType.Mouse;

    private enum ControlType
    {
        Mouse,
        GameController
    }

    private void Update()
    {
        //Controll type
        if (_activeControlType == ControlType.GameController)
        {
            if (InputManager.Instance.PointerDelta())
            {
                _currentSelectedButton = null;
                Cursor.lockState = CursorLockMode.None;
                _activeControlType = ControlType.Mouse;
                return;
            }

            if (InputManager.Instance.NavigationUp())
            {
                Cursor.lockState = CursorLockMode.Locked;

                SetSelected(_currentSelectedButton.upLink);
            }
            else if (InputManager.Instance.NavigationDown())
            {
                Cursor.lockState = CursorLockMode.Locked;

                SetSelected(_currentSelectedButton.downLink);
            }
            else if (InputManager.Instance.NavigationLeft())
            {
                Cursor.lockState = CursorLockMode.Locked;

                SetSelected(_currentSelectedButton.leftLink);
            }
            else if (InputManager.Instance.NavigationRight())
            {
                Cursor.lockState = CursorLockMode.Locked;

                SetSelected(_currentSelectedButton.rightLink);
            }
        }
        else if (_activeControlType == ControlType.Mouse)
        {
            if (InputManager.Instance.NavigationUp() || InputManager.Instance.NavigationDown() || InputManager.Instance.NavigationLeft() || InputManager.Instance.NavigationRight())
            {
                _activeControlType = ControlType.GameController;
                Cursor.lockState = CursorLockMode.Locked;

                SetSelected(initialSelectedButton);
            }
        }
    }

    public void AddButton(ButtonPlus buttonPlus)
    {
        _allButtons.Add(buttonPlus);

        ButtonPlus.OnEnterOver += ButtonPlus_OnEnterOver;
        ButtonPlus.OnExitOver += ButtonPlus_OnExitOver;
    }

    public void SetSelected(ButtonPlus buttonPlus)
    {
        if (buttonPlus == null) return;

        if (_currentSelectedButton != null)
        {
            _currentSelectedButton.SetCurrentState(ButtonPlus.State.Normal);
        }
        buttonPlus.SetCurrentState(ButtonPlus.State.Highlighted);
        _currentSelectedButton = buttonPlus;
    }

    public void ButtonPlus_OnEnterOver(ButtonPlus button)
    {
        if (_activeControlType == ControlType.Mouse)
            _currentSelectedButton = button;
    }

    public void ButtonPlus_OnExitOver(ButtonPlus button)
    {
        if (_activeControlType == ControlType.Mouse)
            _currentSelectedButton = null;
    }
}
