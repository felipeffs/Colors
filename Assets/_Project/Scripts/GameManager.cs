using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    public static event Action<bool> OnPause;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private SOLevelOrder levelOrder;
    [SerializeField] private int frameRate = 60;

    private LevelManager _levelManager;
    private bool _isPaused;

    private void Start()
    {
        MusicChange(true);

        Application.targetFrameRate = frameRate;
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
        MusicChange(false);
    }

    public void ResumeGame()
    {
        OnPause?.Invoke(false);
        Time.timeScale = 1;
        pauseUI.SetActive(false);
        _isPaused = false;
        MusicChange(true);
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
        MusicChange(false);
    }

    //MUSIC
    [Header("Music")]
    [SerializeField] AudioClip gameplayMusic;
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioSource channel;
    [SerializeField] float lastTrackPosition;

    private void MusicChange(bool isGameplay)
    {
        var volumeMusic = (float)PlayerPrefs.GetInt("volume_music");
        var currentTrackPosition = channel.time;
        channel.volume = volumeMusic / 100f;
        channel.clip = isGameplay ? gameplayMusic : menuMusic;
        channel.loop = true;
        channel.time = lastTrackPosition;
        lastTrackPosition = currentTrackPosition;
        channel.Play();
    }
}
