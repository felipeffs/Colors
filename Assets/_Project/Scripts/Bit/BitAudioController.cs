using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitAudioController : MonoBehaviour
{
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
        switch (newState)
        {
            case BitController.States.Walk:
                AudioManager.Instance.PlayAudio(AudioManager.Sound.Walk);
                break;
            case BitController.States.Jump:
            case BitController.States.WallJump:
                AudioManager.Instance.PlayAudio(AudioManager.Sound.Jump);
                break;
            case BitController.States.Dead:
                AudioManager.Instance.PlayAudio(AudioManager.Sound.Die);
                break;
            default:
                break;
        }
    }
}
