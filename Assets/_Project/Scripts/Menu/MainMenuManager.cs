using System;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SOLevelOrder levelOrder;
    [SerializeField] private MenuManager menuManager;

    void Awake()
    {
        menuManager = GetComponentInParent<MenuManager>();
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        levelOrder.LoadLevelByIndex(0);
    }

    public void SettingsMenu()
    {
        menuManager.changeCurrentMenu();
    }
}
