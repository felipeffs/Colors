using UnityEngine;

public class ColorSwapper : MonoBehaviour
{
    [SerializeField] private ColorSwapHandler.ColorID TileColor;
    
    private void Start()
    {
        ColorSwapHandler.OnColorSwap += ColorSwapHandler_OnColorSwap;
    }

    private void OnDestroy()
    {
        ColorSwapHandler.OnColorSwap -= ColorSwapHandler_OnColorSwap;
    }

    private void ColorSwapHandler_OnColorSwap(ColorSwapHandler.ColorID newColor)
    {
        gameObject.SetActive(newColor == TileColor);
    }
}
