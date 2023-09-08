using System;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static event Action OnRestartLevel;

    [Header("Restart Configuration")]
    [SerializeField] private GameObject restartUIPrefab;
    private GameObject _restartUI;
    [ReadOnly][SerializeField] private TextMeshProUGUI restartText;
    [SerializeField] private Transform startPoint;

    //Player
    [SerializeField] private GameObject bitPrefab;
    private GameObject _bit;
    private BitController _bitController;

    [Header("Level Name UI Options")]
    [SerializeField] private string levelName;
    [SerializeField] private TextMeshProUGUI levelNameText;

    private FadeState _currentFadeState;
    private FadeState _nextState;
    private bool _isFirstCicle = true;
    [Min(1)]
    [Tooltip("Fade In duration in seconds")]
    [SerializeField] private float fadeInDuration = 1;
    [Min(1)]
    [Tooltip("Fade Out duration in seconds")]
    [SerializeField] private float fadeOutDuration = 1;
    [Min(1)]
    [Tooltip("Hold duration in seconds")]
    [SerializeField] private float holdDuration = 1;
    private float holdTimer;
    private bool _resetFade;

    [Header("Abilities Available")]
    [SerializeField] private bool grab = false;
    [SerializeField] private ObjectGrabberHUDHandler hudGrabber;

    enum FadeState
    {
        In,
        Out,
        Hold,
        None
    }

    private void OnEnable()
    {
        if (_restartUI == null)
        {
            _restartUI = Instantiate<GameObject>(restartUIPrefab);
            restartText = _restartUI.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        levelNameText.text = levelName;

        RestartLevel();
        GameManager.Instance?.SetLevelManager(this);
        BitController.OnPlayerDeath += BitController_OnPlayerDeath;
    }

    private void OnDestroy()
    {
        BitController.OnPlayerDeath -= BitController_OnPlayerDeath;
    }

    void Update()
    {
        hudGrabber.ChangeStatus(grab);

        if (InputManager.Instance.RestartWasPressed())
        {
            RestartLevel();
        }

        FadeMachine();
    }

    private void FadeMachine()
    {
        _nextState = _currentFadeState switch
        {
            FadeState.In => FadeIn(),
            FadeState.Hold => Hold(),
            FadeState.Out => FadeOut(),
            _ => None()
        };

        _nextState = GlobalTransitions();

        // Change State
        if (_nextState != _currentFadeState)
        {
            _isFirstCicle = true;
            _currentFadeState = _nextState;
        }
        else
        {
            _isFirstCicle = false;
        }
    }

    private FadeState GlobalTransitions()
    {
        if (_resetFade)
        {
            _resetFade = false;
            return FadeState.In;
        }

        return _nextState;
    }

    private FadeState FadeIn()
    {
        if (_isFirstCicle)
        {
            levelNameText.alpha = 0f;
        }

        levelNameText.alpha += Time.deltaTime / fadeInDuration;

        if (levelNameText.alpha >= 1f)
        {
            return FadeState.Hold;
        }
        return FadeState.In;
    }

    private FadeState Hold()
    {
        if (_isFirstCicle)
        {
            levelNameText.alpha = 1f;
            holdTimer = holdDuration;
        }

        holdTimer -= Time.deltaTime;

        if (holdTimer <= 0)
        {
            return FadeState.Out;
        }

        return FadeState.Hold;
    }

    private FadeState FadeOut()
    {
        if (_isFirstCicle)
        {
            levelNameText.alpha = 1f;
        }

        levelNameText.alpha -= Time.deltaTime / fadeOutDuration;

        if (levelNameText.alpha <= 0f)
        {
            return FadeState.None;
        }
        return FadeState.Out;
    }

    private FadeState None()
    {
        if (_resetFade)
        {
            return FadeState.In;
        }
        return FadeState.None;
    }

    public void RestartLevel()
    {
        restartText.enabled = false;
        ResetBit(startPoint.position);
        OnRestartLevel?.Invoke();
        _resetFade = true;
    }

    private void BitController_OnPlayerDeath()
    {
        restartText.enabled = true;
    }

    private void ResetBit(Vector3 newPosition)
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
}

