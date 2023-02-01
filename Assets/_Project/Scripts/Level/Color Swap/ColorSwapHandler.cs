using UnityEngine;
using System;

public class ColorSwapHandler : MonoBehaviour
{
    public static event Action<ColorID> OnColorSwap;
    public static event Action OnColorSwapByPlayer;

    [SerializeField] private Color Color1;
    [SerializeField] private Color Color2;
    [SerializeField] private ColorID initialColor;
    private ColorID _activeColor;

    public enum ColorID
    {
        Color1,
        Color2
    }

    private void Awake()
    {
        LevelManager.OnRestartLevel += LevelManager_OnRestartLevel;
    }

    private void OnDisable()
    {
        LevelManager.OnRestartLevel -= LevelManager_OnRestartLevel;
    }

    private void Update()
    {
        if (InputManager.Instance.SwapWasPressed())
        {
            OnColorSwapByPlayer?.Invoke();
            ChangeActiveColor();
        }
    }

    public void ChangeActiveColor()
    {
        var newColor = _activeColor switch
        {
            ColorID.Color1 => ColorID.Color2,
            ColorID.Color2 => ColorID.Color1,
            _ => ColorID.Color1
        };


        ChangeActiveColor(newColor);
    }

    private void ChangeActiveColor(ColorID newColor)
    {
        _activeColor = newColor;

        OnColorSwap?.Invoke(_activeColor);

        Camera.main.backgroundColor = (_activeColor == ColorID.Color1) ? Color2 : Color1;
    }

    private void LevelManager_OnRestartLevel()
    {
        ChangeActiveColor(initialColor);
    }
}