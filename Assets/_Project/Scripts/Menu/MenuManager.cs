using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private ButtonPlus settingsInitialButton;
    [SerializeField] private ButtonPlus mainInitialButton;
    [SerializeField] private GameObject settingsHudGroup;
    [SerializeField] private GameObject mainMenuHudGroup;
    private MenuState _nextMenu;
    private MenuState _currentMenu;
    private bool _firstCicle = true;
    private bool changeToSettings;

    private void Update()
    {
        RunState();
    }

    private void RunState()
    {
        _nextMenu = _currentMenu switch
        {
            MenuState.Main => MainMenu(),
            MenuState.Settings => SettingsMenu(),
            _ => MenuState.Main
        };

        _nextMenu = GlobalTransitions(_nextMenu);

        // Change State
        if (_nextMenu != _currentMenu)
        {
            Debug.Log(_currentMenu);
            _firstCicle = true;
            _currentMenu = _nextMenu;
            Debug.Log(_currentMenu);

        }
        else
        {
            _firstCicle = false;
        }
    }

    private MenuState GlobalTransitions(MenuState nextMenu)
    {
        return _nextMenu;
    }

    private MenuState SettingsMenu()
    {
        if (_firstCicle)
        {
            mainMenuHudGroup.SetActive(false);
            settingsHudGroup.SetActive(true);
            UIPlusModule.Instance.ChangeInitialSelectedButton(settingsInitialButton);
            UIPlusModule.Instance.SetSelected(settingsInitialButton);
        }

        if (InputManager.Instance.NavigationReturn())
        {
            return MenuState.Main;
        }
        return MenuState.Settings;
    }

    private MenuState MainMenu()
    {
        if (_firstCicle)
        {
            mainMenuHudGroup.SetActive(true);
            settingsHudGroup.SetActive(false);
            UIPlusModule.Instance.ChangeInitialSelectedButton(mainInitialButton);
            UIPlusModule.Instance.SetSelected(mainInitialButton);
        }

        if (changeToSettings)
        {
            changeToSettings = false;
            return MenuState.Settings;
        }
        return MenuState.Main;
    }

    public enum MenuState
    {
        Main,
        Settings
    }

    public void changeCurrentMenu()
    {
        changeToSettings = true;
    }
}
