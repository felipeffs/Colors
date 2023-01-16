using System;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static event Action OnRestartLevel;

    [SerializeField] private TextMeshProUGUI restartText;
    [SerializeField] private Transform startPoint;

    private void Start()
    {
        RestartLevel();
        BitController.OnPlayerDeath += BitController_OnPlayerDeath;
    }

    private void OnDestroy()
    {
        BitController.OnPlayerDeath -= BitController_OnPlayerDeath;
    }

    void Update()
    {
        if (InputManager.Instance.RestartWasPressed())
        {
            RestartLevel();
        }
    }

    private void RestartLevel()
    {
        restartText.enabled = false;
        GameHandler.Instance.ResetBit(startPoint.position);
        OnRestartLevel?.Invoke();
    }

    private void BitController_OnPlayerDeath()
    {
        restartText.enabled = true;
    }
}
