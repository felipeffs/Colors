using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorInverter : MonoBehaviour
{
    [ReadOnly][SerializeField] private ColorIDInverter currentColor;
    [SerializeField] private ColorIDInverter initialColor;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private List<ColorInverterPlatform> platforms;

    [ReadOnly][SerializeField] private bool canInverter;

    public enum ColorIDInverter
    {
        Color1,
        Color2
    }
    private void Start()
    {
        ChangeColor(initialColor);
    }

    private void OnEnable()
    {
        ColorSwapHandler.OnColorSwap += ColorSwapHandler_OnColorSwap;
    }

    private void OnDestroy()
    {

        ColorSwapHandler.OnColorSwap -= ColorSwapHandler_OnColorSwap;
    }

    private void ColorSwapHandler_OnColorSwap(ColorSwapHandler.ColorID newColor)
    {
        if (canInverter)
            ChangeColor(currentColor);
    }

    private void ChangeColor(ColorIDInverter newColor)
    {
        if (currentColor == ColorIDInverter.Color1)
        {
            currentColor = ColorIDInverter.Color2;
            sr.color = color1;
        }
        else
        {
            currentColor = ColorIDInverter.Color1;
            sr.color = color2;

        }

        foreach (var platform in platforms)
        {
            platform.InverterColor(currentColor);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 7)
            canInverter = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 7)
            canInverter = false;
    }
}
