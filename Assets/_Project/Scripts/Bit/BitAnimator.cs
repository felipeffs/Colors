using System.Collections;
using UnityEngine;

public class BitAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator anim;

    [Header("Particles")]
    [SerializeField] private ParticleSystem walkTrail;
    [SerializeField] private ParticleSystem jumpDust;

    [Header("Testing")]
    [SerializeField] private bool jumpEffect;
    [SerializeField] private bool walkEffect;
    [SerializeField] private bool fallEffect;

    //Flip
    public bool isFliped { get => sr.flipX; }
    private readonly Vector3 localFlippedScale = new Vector3(-1, 1, 1);
    private readonly Vector3 localNoFlippedScale = new Vector3(1, 1, 1);

    //Animations String Hashs
    private readonly int Idle = Animator.StringToHash("Idle");
    private readonly int Walk = Animator.StringToHash("Walk");
    private readonly int JumpImpulse = Animator.StringToHash("JumpImpulse");
    private readonly int JumpToFall = Animator.StringToHash("JumpToFall");
    private readonly int FallLoop = Animator.StringToHash("FallLoop");

    public void Action(bool play, BitController.States currentState)
    {
        Action(play, currentState, currentState);
    }
    public void Action(bool play, BitController.States currentState, BitController.States nextState)
    {
        switch (currentState)
        {
            case BitController.States.Idle:
                IdleState(play);
                break;
            case BitController.States.Walk:
                WalkState(play);
                break;
            case BitController.States.Jump:
                JumpState(play, nextState);
                break;
            case BitController.States.WallJump:
                JumpState(play, nextState);
                break;
            case BitController.States.Falling:
                FallingState(play, nextState);
                break;
            case BitController.States.Dead:
                break;
            default:
                break;
        }

    }

    private void IdleState(bool play)
    {
        if (play)
        {
            anim.CrossFade(Idle, 0, 0);
        }
    }

    private void WalkState(bool play)
    {
        if (!walkEffect) return;

        if (play)
        {
            walkTrail.Play();
            anim.CrossFade(Walk, 0, 0);
        }
        else
        {
            walkTrail.Stop();
        }
    }

    private void JumpState(bool play, BitController.States nextState)
    {
        if (!jumpEffect) return;

        if (play)
        {
            jumpDust.Play();
            anim.CrossFade(JumpImpulse, 0, 0);
        }
    }

    private void FallingState(bool play, BitController.States nextState)
    {
        if (!fallEffect) return;

        if (play)
        {
            anim.CrossFade(JumpToFall, 0, 0);
            anim.CrossFade(FallLoop, 0, 0);
        }
        else
        {
            if (nextState == BitController.States.Idle)
                jumpDust.Play();
        }
    }

    public void VisualFlip(bool flip)
    {
        sr.flipX = flip;
        var newLocalScale = flip ? localFlippedScale : localNoFlippedScale;
        walkTrail.transform.localScale = newLocalScale;
    }
}
