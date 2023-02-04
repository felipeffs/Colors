using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitAudioController : MonoBehaviour
{

    private AudioManager.Sound lastAudioState;
    private BitController.States lastState;

    void OnEnable()
    {
        BitController.OnStateChange += BitController_OnStateChange;
    }

    void OnDisable()
    {
        BitController.OnStateChange -= BitController_OnStateChange;

    }

    private void BitController_OnStateChange(BitController.States newState)
    {
        if (lastAudioState != AudioManager.Sound.Landing)
            AudioManager.Instance.StopAudio(lastAudioState);

        switch (newState)
        {
            case BitController.States.Walk:
                AudioManager.Instance.PlayAudio(AudioManager.Sound.Walk);
                lastAudioState = AudioManager.Sound.Walk;
                break;
            case BitController.States.Jump:
            case BitController.States.WallJump:
                AudioManager.Instance.PlayAudio(AudioManager.Sound.Jump);
                lastAudioState = AudioManager.Sound.Jump;
                break;
            case BitController.States.Dead:
                AudioManager.Instance.PlayAudio(AudioManager.Sound.Die);
                lastAudioState = AudioManager.Sound.Die;
                break;
            case BitController.States.Idle:
                if (lastState == BitController.States.Falling)
                {
                    AudioManager.Instance.PlayAudio(AudioManager.Sound.Landing);
                    lastAudioState = AudioManager.Sound.Landing;
                }
                break;
            default:
                break;
        }

        lastState = newState;
    }
}
