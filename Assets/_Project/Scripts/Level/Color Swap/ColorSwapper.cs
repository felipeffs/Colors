using UnityEngine;
using System;

public class ColorSwapper : MonoBehaviour
{
    [SerializeField] private ColorSwapHandler.ColorID tileColor;
    public ColorSwapHandler.ColorID TileColor { get => tileColor;}
    private bool _isActive = true;
    public bool IsActive { get => _isActive; set => _isActive = value; }
    public event Action<bool> DoOnColorChange;

    private void Awake()
    {
        ColorSwapHandler.OnColorSwap += ColorSwapHandler_OnColorSwap;
    }

    private void OnDestroy()
    {
        ColorSwapHandler.OnColorSwap -= ColorSwapHandler_OnColorSwap;
    }

    private void ColorSwapHandler_OnColorSwap(ColorSwapHandler.ColorID newColor)
    {
        if (!_isActive) return;
        gameObject.SetActive(newColor == tileColor);
        DoOnColorChange?.Invoke(newColor == tileColor);
    }

}
