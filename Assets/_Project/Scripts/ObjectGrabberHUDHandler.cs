using System;
using UnityEngine;
using UnityEngine.UI;

public class ObjectGrabberHUDHandler : MonoBehaviour
{
    private ColorAnimationState _currentFadeState;
    private ColorAnimationState _nextState;
    private bool _isFirstCicle = true;
    [SerializeField] private Image cooldownRing;
    [SerializeField] private Transform availableIcon;
    [SerializeField] private Transform unavailableIcon;
    [SerializeField] private Transform background;
    [SerializeField] private Transform keyIndicator;

    private float _currentTime;
    [Min(0.1f)]
    private float _duration = 1;
    [SerializeField] private ObjectGrabber _grabber;
    private bool isAvailable;

    enum ColorAnimationState
    {
        FillingUp,
        Ready,
        Desactived
    }

    void OnEnable()
    {
        ObjectGrabber.OnStart += ObjectGrabber_onStart;
    }

    void OnDestroy()
    {
        ObjectGrabber.OnStart -= ObjectGrabber_onStart;
    }

    private void ObjectGrabber_onStart(ObjectGrabber grabber)
    {
        _grabber = grabber;
    }

    void FixedUpdate()
    {
        AnimationMachine();
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
        if (_grabber == null || isAvailable == false)
        {
            print("GLOBAL");
            return ColorAnimationState.Desactived;
        }

        return _nextState;
    }

    private ColorAnimationState Ready()
    {
        if (_isFirstCicle)
        {
            cooldownRing.gameObject.SetActive(true);
            availableIcon.gameObject.SetActive(true);
            unavailableIcon.gameObject.SetActive(false);
            background.gameObject.SetActive(true);
            keyIndicator.gameObject.SetActive(true);
        }

        // Condition to change state here
        if (_grabber.isRecharging()) return ColorAnimationState.FillingUp;

        return ColorAnimationState.Ready;
    }

    private ColorAnimationState FillingUp()
    {
        if (_isFirstCicle)
        {
            unavailableIcon.gameObject.SetActive(true);
            _currentTime = 0;
            _duration = _grabber?.GetCooldownTimer() ?? 0f;
        }

        _currentTime += Time.deltaTime;

        cooldownRing.fillAmount = _currentTime * (1 / _duration);
        if (_currentTime >= _duration) return ColorAnimationState.Ready;
        return ColorAnimationState.FillingUp;
    }

    private ColorAnimationState Desactived()
    {
        if (_isFirstCicle)
        {
            cooldownRing.gameObject.SetActive(false);
            availableIcon.gameObject.SetActive(false);
            unavailableIcon.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
            keyIndicator.gameObject.SetActive(false);
        }

        if (isAvailable == true && _grabber != null) return ColorAnimationState.Ready;

        return ColorAnimationState.Ready;
    }

    public void ChangeStatus(bool isActive) => isAvailable = isActive;

}
