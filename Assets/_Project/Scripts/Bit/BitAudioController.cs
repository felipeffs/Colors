using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitAudioController : MonoBehaviour
{

    private AudioManager.Sound _lastAudioState;
    private BitController.State _lastState;

    void OnEnable()
    {
        BitController.OnStateChange += BitController_OnStateChange;
    }

    void OnDisable()
    {
        BitController.OnStateChange -= BitController_OnStateChange;

    }

    private void BitController_OnStateChange(BitController.State newState)
    {
        if (_lastAudioState != AudioManager.Sound.Landing)
            AudioManager.Instance.StopAudio(_lastAudioState);

        switch (newState)
        {
            case BitController.State.Walk:
                AudioManager.Instance.PlayAudio(AudioManager.Sound.Walk);
                _lastAudioState = AudioManager.Sound.Walk;
                break;
            case BitController.State.Jump:
            case BitController.State.WallJump:
                AudioManager.Instance.PlayAudio(AudioManager.Sound.Jump);
                _lastAudioState = AudioManager.Sound.Jump;
                break;
            case BitController.State.Dead:
                AudioManager.Instance.PlayAudio(AudioManager.Sound.Die);
                _lastAudioState = AudioManager.Sound.Die;
                break;
            case BitController.State.Idle:
                if (_lastState == BitController.State.Falling)
                {
                    AudioManager.Instance.PlayAudio(AudioManager.Sound.Landing);
                    _lastAudioState = AudioManager.Sound.Landing;
                }
                break;
            default:
                break;
        }

        _lastState = newState;
    }
}
