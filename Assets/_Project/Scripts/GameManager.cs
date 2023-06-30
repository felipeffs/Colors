using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    public static event Action<bool> OnPause;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private SOLevelOrder levelOrder;

    private LevelManager _levelManager;
    private bool _isPaused;

    private void Start()
    {
        pauseUI.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.Instance.PauseWasPressed())
        {
            if (!_isPaused)
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
        _isPaused = true;
        OnPause?.Invoke(true);
        Time.timeScale = 0;
        pauseUI.SetActive(true);
    }

    public void ResumeGame()
    {
        OnPause?.Invoke(false);
        Time.timeScale = 1;
        pauseUI.SetActive(false);
        _isPaused = false;
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
