using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DisableObjectAction : ConnectorAction
{
    private ColorSwapper _colorSwapper;
    private bool _doColorsChange;
    [SerializeField] private bool initialState;
    private bool _currentState;

    private void Awake()
    {
        _colorSwapper = GetComponent<ColorSwapper>();
    }

    private void Start()
    {
        _doColorsChange = _colorSwapper ?? false;
        ChangeCurrentState(initialState);
    }

    public override void Execute(bool state)
    {
        ChangeCurrentState(state ? !initialState : initialState);
    }

    private void ChangeCurrentState(bool newState)
    {
        gameObject.SetActive(newState);
        if (_doColorsChange)
            _colorSwapper.IsActive = newState;
    }
}


