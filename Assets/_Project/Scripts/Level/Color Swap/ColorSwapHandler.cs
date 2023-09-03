using UnityEngine;
using System;
using System.Threading;

public class ColorSwapHandler : MonoBehaviour
{
    public static event Action<ColorID> OnColorSwap;
    public static event Action OnColorSwapByPlayer;

    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    [SerializeField] private ColorID initialColor;
    private ColorID _activeColor;
    [SerializeField] private float cooldownTime = .25f;
    private float _timer;

    public enum ColorID
    {
        Color1,
        Color2,
        None
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
        if (InputManager.Instance.SwapWasPressed() && _timer <= 0)
        {
            _timer = cooldownTime;
            OnColorSwapByPlayer?.Invoke();
            ChangeActiveColor();
            return;
        }

        _timer -= Time.deltaTime;
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

        Camera.main.backgroundColor = (_activeColor == ColorID.Color1) ? color2 : color1;
    }

    private void LevelManager_OnRestartLevel()
    {
        ChangeActiveColor(initialColor);
    }

    public float GetCooldownTimer() => cooldownTime;
    public (Color, Color) GetLevelColors() => (color1, color2);

}