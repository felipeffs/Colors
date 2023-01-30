using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private PlayerInputs _controls;

    [Header("Input Buffer")]
    [SerializeField] private float jumpBufferWindow = 0.4f;
    private bool _jumpBuffered;
    private float _jumpBufferTimer;

    [Header("Awake")]
    [SerializeField] private float _delayToAwake = 0.3f;
    private float _timerToAwake;

    protected override void SingletonAwake()
    {
        _controls = new PlayerInputs();
    }

    private void OnEnable()
    {
        GameManager.OnPause += GameManager_OnPause;
        _controls.Bit.Enable();
        _controls.Level.Enable();
    }

    private void OnDisable()
    {
        GameManager.OnPause -= GameManager_OnPause;
        _controls.Bit.Disable();
        _controls.Level.Disable();
    }

    private void Start()
    {
        _timerToAwake = _delayToAwake;
    }

    private void Update()
    {
        if (_timerToAwake > 0)
        {
            _timerToAwake -= Time.deltaTime;
            return;
        }

        if (_jumpBufferTimer > 0)
        {
            _jumpBufferTimer -= Time.deltaTime;
        }
        else
        {
            _jumpBuffered = false;
        }

        if (_controls.Bit.Jump.WasPressedThisFrame())
        {
            _jumpBuffered = true;
            _jumpBufferTimer = jumpBufferWindow;
        }
    }

    private void GameManager_OnPause(bool isPaused)
    {
        if (isPaused)
        {
            _controls.Bit.Disable();
            _controls.Level.Restart.Disable();
        }
        else
        {
            _controls.Bit.Enable();
            _controls.Level.Restart.Enable();
        }
    }

    public float WalkRawValue()
    {
        if (_controls.Bit.Walk.ReadValue<float>() > 0)
            return 1f;
        else if (_controls.Bit.Walk.ReadValue<float>() < 0)
            return -1f;
        return 0f;
    }

    public bool WalkWasPressed()
    {
        return _controls.Bit.Walk.WasPressedThisFrame();
    }

    public bool WalkWasPerformed()
    {
        return _controls.Bit.Walk.WasPerformedThisFrame();
    }

    public bool WalkWasReleased()
    {
        return _controls.Bit.Walk.WasReleasedThisFrame();
    }

    public bool JumpWasPressed()
    {
        var wasPressed = _jumpBuffered;
        _jumpBuffered = false;
        return wasPressed;
    }

    public bool SwapWasPressed()
    {
        return _controls.Bit.SwapColor.WasPressedThisFrame();
    }

    public bool RestartWasPressed()
    {
        return _controls.Level.Restart.WasPressedThisFrame();
    }

    public bool PauseWasPressed()
    {
        return _controls.Level.Pause.WasPerformedThisFrame();
    }
}