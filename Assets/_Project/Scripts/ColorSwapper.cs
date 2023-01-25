using UnityEngine;
using System;

public class ColorSwapper : MonoBehaviour
{
    [SerializeField] private ColorSwapHandler.ColorID tileColor;
    public ColorSwapHandler.ColorID TileColor { get => tileColor; }

    public event Action<bool> BeforeColorSwap;

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
        BeforeColorSwap?.Invoke(newColor == tileColor);
        gameObject.SetActive(newColor == tileColor);
    }
}
