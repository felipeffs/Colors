using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSwapHUDHandler : MonoBehaviour
{
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    [SerializeField] private Image colorIndicator;
    [SerializeField] private Image cooldownRing;
    [SerializeField] private Transform availableIcon;

    private float _currentTime;
    [Min(0.1f)]
    private float _duration = 1;

    void OnEnable()
    {
        ColorSwapHandler.OnColorSwap += ColorSwapHandler_OnColorSwap;
    }

    void OnDestroy()
    {
        ColorSwapHandler.OnColorSwap -= ColorSwapHandler_OnColorSwap;

    }

    private void ColorSwapHandler_OnColorSwap(ColorSwapHandler.ColorID colorID)
    {
        newColor = colorID == ColorSwapHandler.ColorID.Color1 ? color1 : color2;
        itsChanged = true;
    }

    void FixedUpdate()
    {
        AnimationMachine();
    }

    private ColorAnimationState _currentFadeState;
    private ColorAnimationState _nextState;
    private bool _isFirstCicle = true;
    private bool itsChanged;
    private Color currentColor;
    private Color newColor;

    [SerializeField] private ColorSwapHandler colorSwapHandler;

    enum ColorAnimationState
    {
        FillingUp,
        Ready,
        Desactived
    }

    private void AnimationMachine()
    {
        _nextState = _currentFadeState switch
        {
            ColorAnimationState.Desactived => Desactived(),
            ColorAnimationState.Ready => Ready(),
            ColorAnimationState.FillingUp => FillingUp(),
            _ => Desactived()
        };

        _nextState = GlobalTransitions();

        // Change State
        if (_nextState != _currentFadeState)
        {
            _isFirstCicle = true;
            _currentFadeState = _nextState;
        }
        else
        {
            _isFirstCicle = false;
        }
    }

    private ColorAnimationState GlobalTransitions()
    {
        return _nextState;
    }

    private ColorAnimationState Ready()
    {
        availableIcon.gameObject.SetActive(true);
        if (itsChanged)
        {
            itsChanged = false;
            if (currentColor != newColor) return ColorAnimationState.FillingUp;
        }
        return ColorAnimationState.Ready;
    }

    private ColorAnimationState FillingUp()
    {
        if (_isFirstCicle)
        {
            availableIcon.gameObject.SetActive(false);
            _currentTime = 0;
            _duration = colorSwapHandler.GetCooldownTimer();
        }

        _currentTime += Time.deltaTime;

        var colorAVector = new Vector3(currentColor.r, currentColor.g, currentColor.b);
        var colorBVector = new Vector3(newColor.r, newColor.g, newColor.b);
        var colortemp = Vector3.Lerp(colorAVector, colorBVector, _currentTime);

        currentColor = new Color(colortemp.x, colortemp.y, colortemp.z);

        colorIndicator.color = currentColor;

        cooldownRing.fillAmount = _currentTime * (1 / _duration);
        if (_currentTime >= _duration) return ColorAnimationState.Ready;
        return ColorAnimationState.FillingUp;
    }

    private ColorAnimationState Desactived()
    {
        availableIcon.parent.gameObject.SetActive(false);
        return ColorAnimationState.Ready;
    }

}
