using UnityEngine;
using UnityEngine.UI;

public class ColorSwapHUDHandler : MonoBehaviour
{
    private Color _color1;
    private Color _color2;

    private float _currentTime;
    [Min(0.1f)]
    private float _duration = 1;

    private void OnEnable()
    {
        ColorSwapHandler.OnColorSwap += ColorSwapHandler_OnColorSwap;
    }

    private void OnDestroy()
    {
        ColorSwapHandler.OnColorSwap -= ColorSwapHandler_OnColorSwap;

    }

    private void Start()
    {
        _colorSwapHandler = FindObjectOfType<ColorSwapHandler>();
        (_color1, _color2) = _colorSwapHandler.GetLevelColors();
    }

    private void ColorSwapHandler_OnColorSwap(ColorSwapHandler.ColorID colorID)
    {
        _newColor = colorID == ColorSwapHandler.ColorID.Color1 ? _color1 : _color2;
    }

    private void FixedUpdate()
    {
        AnimationMachine();
    }

    private ColorAnimationState _currentFadeState;
    private ColorAnimationState _nextState;
    private bool _isFirstCicle = true;
    private Color _currentColor;
    private Color _newColor;
    [SerializeField] private Image colorIndicator;
    [SerializeField] private Image cooldownRing;
    [SerializeField] private Transform availableIcon;
    [SerializeField] private Transform background;
    [SerializeField] private Transform keyIndicator;

    private ColorSwapHandler _colorSwapHandler;

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
        if (_isFirstCicle)
        {
            availableIcon.gameObject.SetActive(true);
            background.gameObject.SetActive(true);
            keyIndicator.gameObject.SetActive(true);
        }

        if (_currentColor != _newColor)
        {
            print(_currentColor != _newColor);
            return ColorAnimationState.FillingUp;
        }

        return ColorAnimationState.Ready;
    }

    private ColorAnimationState FillingUp()
    {
        if (_isFirstCicle)
        {
            availableIcon.gameObject.SetActive(false);
            _currentTime = 0;
            _duration = _colorSwapHandler.GetCooldownTimer();
        }

        _currentTime += Time.deltaTime;

        var colorAVector = new Vector3(_currentColor.r, _currentColor.g, _currentColor.b);
        var colorBVector = new Vector3(_newColor.r, _newColor.g, _newColor.b);
        var colortemp = Vector3.Lerp(colorAVector, colorBVector, _currentTime * (1 / _duration));

        _currentColor = new Color(colortemp.x, colortemp.y, colortemp.z);

        colorIndicator.color = _currentColor;

        cooldownRing.fillAmount = _currentTime * (1 / _duration);

        if (_currentTime >= _duration)
        {
            _currentColor = _newColor;
            return ColorAnimationState.Ready;
        }
        return ColorAnimationState.FillingUp;
    }

    private ColorAnimationState Desactived()
    {
        if (_isFirstCicle)
        {
            availableIcon.parent.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
            keyIndicator.gameObject.SetActive(false);
        }

        return ColorAnimationState.Ready;
    }

}
