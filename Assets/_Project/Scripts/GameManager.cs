using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    public static event Action<bool> OnPause;
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private SOLevelOrder levelOrder;

    private LevelManager _levelManager;
    private bool isGamePause;

    private void Start()
    {
        _pauseUI.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.Instance.PauseWasPressed())
        {
            if (!isGamePause)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

    }

    private void PauseGame()
    {
        isGamePause = true;
        OnPause?.Invoke(true);
        Time.timeScale = 0;
        _pauseUI.SetActive(true);
    }

    public void ResumeGame()
    {
        OnPause?.Invoke(false);
        Time.timeScale = 1;
        _pauseUI.SetActive(false);
        isGamePause = false;
    }

    public void RestartLevel()
    {
        _levelManager?.RestartLevel();
        ResumeGame();
    }

    public void SetLevelManager(LevelManager levelManager)
    {
        _levelManager = levelManager;
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        ResumeGame();
        levelOrder.LoadMainMenu();
    }
}
