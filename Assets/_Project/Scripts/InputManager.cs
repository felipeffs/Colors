using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private PlayerInputs _controls;

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
        return _controls.Bit.Jump.WasPressedThisFrame();
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