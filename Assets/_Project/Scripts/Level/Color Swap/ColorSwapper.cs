using UnityEngine;
using System;

public class ColorSwapper : MonoBehaviour
{
    [SerializeField] private ColorSwapHandler.ColorID tileColor;


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
        gameObject.SetActive(newColor == tileColor);
    }
}
