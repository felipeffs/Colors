using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSwapAudioController : MonoBehaviour
{
    void OnEnable()
    {
        ColorSwapHandler.OnColorSwapByPlayer += ColorSwapHandler_OnColorSwap;
    }

    void OnDisable()
    {
        ColorSwapHandler.OnColorSwapByPlayer -= ColorSwapHandler_OnColorSwap;
    }

    private void ColorSwapHandler_OnColorSwap()
    {
        AudioManager.Instance.PlayAudio(AudioManager.Sound.Swap);
    }
}
