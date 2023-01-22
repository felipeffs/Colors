using System;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static event Action OnRestartLevel;

    //
    [SerializeField] private TextMeshProUGUI restartText;
    [SerializeField] private Transform startPoint;

    //Player
    [SerializeField] private GameObject bitPrefab;
    private GameObject _bit;
    private BitController _bitController;

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
        if (InputManager.Instance.ExitWasPressed())
        {
            CloseGame();
        }
    }

    private void RestartLevel()
    {
        restartText.enabled = false;
        ResetBit(startPoint.position);
        OnRestartLevel?.Invoke();
    }

    private void BitController_OnPlayerDeath()
    {
        restartText.enabled = true;
    }

    public void ResetBit(Vector3 newPosition)
    {
        if (_bit == null)
        {
            _bit = Instantiate(bitPrefab, Vector3.zero, bitPrefab.transform.rotation);
            _bitController = _bit.GetComponentInChildren<BitController>();
            _bit.SetActive(false);
        }

        _bitController.Reset(newPosition);
        _bit.SetActive(true);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}

