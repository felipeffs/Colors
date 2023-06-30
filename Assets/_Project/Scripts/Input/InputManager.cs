using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private PlayerInputs _controls;

    [Header("Input Buffer")]
    [SerializeField] private float jumpBufferWindow = 0.4f;
    private bool _jumpBuffered;
    private float _jumpBufferTimer;

    [Header("Awake")]
    [SerializeField] private float awakeUpDelay = 0.3f;
    private float _awakeUpTimer;

    protected override void SingletonAwake()
    {
        _controls = new PlayerInputs();
    }

    private void OnEnable()
    {
        GameManager.OnPause += GameManager_OnPause;
        _controls.Bit.Enable();
        _controls.Level.Enable();
        _controls.Menu.Enable();
    }

    private void OnDisable()
    {
        GameManager.OnPause -= GameManager_OnPause;
        _controls.Bit.Disable();
        _controls.Level.Disable();
        _controls.Menu.Disable();
    }

    private void Start()
    {
        _awakeUpTimer = awakeUpDelay;
    }

    private void Update()
    {
        if (_awakeUpTimer > 0)
        {
            _awakeUpTimer -= Time.deltaTime;
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
            _controls.Menu.Enable();
        }
        else
        {
            _controls.Bit.Enable();
            _controls.Level.Restart.Enable();
            _controls.Menu.Disable();
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

    public bool InteractWasPressed()
    {
        return _controls.Bit.Interact.WasPerformedThisFrame();
    }

    public bool GrabWasPressed()
    {
        return _controls.Bit.Grab.WasPerformedThisFrame();
    }

    //UI Menu

    public bool PointerClickHeld()
    {
        return false;
    }

    public bool PointerDelta()
    {
        var delta = _controls.Menu.Delta.ReadValue<Vector2>();
        return delta.y > 0.12f || delta.x > 0.12f;
    }

    public bool PointerClickPressed()
    {
        return _controls.Menu.PointerSelect.WasPressedThisFrame();
    }

    public bool PointerClickReleased()
    {
        return _controls.Menu.PointerSelect.WasReleasedThisFrame();
    }

    public Vector3 PointerPosition()
    {
        return _controls.Menu.Position.ReadValue<Vector2>();
    }

    public bool NavigationSelectPressed()
    {
        return _controls.Menu.NavigationSelect.WasPressedThisFrame();
    }

    public bool NavigationSelectReleased()
    {
        return _controls.Menu.NavigationSelect.WasReleasedThisFrame();
    }

    public bool NavigationUp()
    {
        return _controls.Menu.NavigationUp.WasPressedThisFrame();
    }

    public bool NavigationDown()
    {
        return _controls.Menu.NavigationDown.WasPressedThisFrame();
    }

    public bool NavigationLeft()
    {
        return _controls.Menu.NavigationLeft.WasPressedThisFrame();
    }

    public bool NavigationRight()
    {
        return _controls.Menu.NavigationRight.WasPressedThisFrame();
    }
}