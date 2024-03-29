using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    public event Action<ControlType> OnControlTypeChange;
    private PlayerInputs _controls;

    [Header("Input Buffer")]
    [SerializeField] private float jumpBufferWindow = 0.4f;
    private bool _jumpBuffered;
    private float _jumpBufferTimer;

    [Header("Awake")]
    [SerializeField] private float awakeUpDelay = 0.3f;
    private float _awakeUpTimer;

    public enum ControlType
    {
        KeyboardMouse,
        Gamepad
    }

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
        CheckControlType();
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

    private void CheckControlType()
    {
        var activeLayout = GetLayout(_controls.Bit.Walk);
        if (activeLayout != null) { GetControlType(activeLayout); return; }
        activeLayout = GetLayout(_controls.Bit.Jump);
        if (activeLayout != null) { GetControlType(activeLayout); return; }
        activeLayout = GetLayout(_controls.Bit.SwapColor);
        if (activeLayout != null) { GetControlType(activeLayout); return; }
        activeLayout = GetLayout(_controls.Bit.Grab);
        if (activeLayout != null) { GetControlType(activeLayout); return; }
        activeLayout = GetLayout(_controls.Bit.Interact);
        if (activeLayout != null) { GetControlType(activeLayout); return; }
    }

    private String GetLayout(InputAction action)
    {
        return action.activeControl?.layout;
    }

    private void GetControlType(String layout)
    {
        var currentType = layout == "Button" ? ControlType.Gamepad : ControlType.KeyboardMouse;
        OnControlTypeChange?.Invoke(currentType);
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

    public float GrabWasPressed()
    {
        return _controls.Bit.Grab.ReadValue<float>();
    }

    public bool GrabWasReleased()
    {
        return _controls.Bit.Grab.WasReleasedThisFrame();
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

    public bool NavigationHeldLeft()
    {
        return _controls.Menu.NavigationLeft.ReadValue<bool>();
    }

    public bool NavigationHeldRight()
    {
        return _controls.Menu.NavigationRight.ReadValue<bool>();
    }

    public bool NavigationReturn()
    {
        return _controls.Menu.NavigationReturn.WasPressedThisFrame();
    }
}