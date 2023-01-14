using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputs _controls;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log($"There can be only one instance of {name}");
            Destroy(this);
            return;
        }

        Instance = this;
        _controls = new PlayerInputs();
    }

    private void OnEnable()
    {
        _controls.Bit.Enable();
    }

    private void OnDisable()
    {
        _controls.Bit.Disable();
    }

    public float WalkRawValue()
    {
        return _controls.Bit.Walk.ReadValue<float>();
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
        return _controls.Bit.SwapColor.WasPressedThisFrame();
    }

    public bool CloseGameWasPressed()
    {
        return _controls.Bit.SwapColor.WasPerformedThisFrame();
    }
}