using UnityEngine;
using System;

public class ColorSwapHandler : MonoBehaviour
{
    public static event Action<ColorID> OnColorSwap;

    [SerializeField] private Color Color1;
    [SerializeField] private Color Color2;
    [SerializeField] private ColorID initialColor;
    private ColorID _activeColor;

    public enum ColorID
    {
        Color1,
        Color2
    }

    private void Start()
    {
        ChangeActiveColor(initialColor);
    }

    private void Update()
    {
        if (InputManager.Instance.SwapWasPressed()) ChangeActiveColor();
    }

    public void ChangeActiveColor()
    {
        _activeColor = _activeColor switch
        {
            ColorID.Color1 => ColorID.Color2,
            ColorID.Color2 => ColorID.Color1,
            _ => ColorID.Color1
        };

        ChangeActiveColor(_activeColor);
    }

    private void ChangeActiveColor(ColorID newColor)
    {
        OnColorSwap?.Invoke(newColor);

        Camera.main.backgroundColor = (newColor == ColorID.Color1) ? Color2 : Color1;
    }
}