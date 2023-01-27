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
    private readonly int JumpImpulse = Animator.StringToHash("TakeOff");
    private readonly int Jump = Animator.StringToHash("Jump");
    private readonly int JumpToFall = Animator.StringToHash("JumpToFall");
    private readonly int FallLoop = Animator.StringToHash("FallLoop");

    //Animations
    [SerializeField] private float jumpToFallDuration = 15f;
    [SerializeField] private float takeOffDuration = 10f;

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

    IEnumerator CO_Fall()
    {
        anim.CrossFade(JumpToFall, 0, 0);
        yield return new WaitForSeconds(jumpToFallDuration / 60f);
        anim.CrossFade(FallLoop, 0, 0);
    }

    IEnumerator CO_Jump()
    {
        anim.CrossFade(JumpImpulse, 0, 0);
        yield return new WaitForSeconds(takeOffDuration / 60f);
        anim.CrossFade(Jump, 0, 0);
    }

    private void IdleState(bool play)
    {
        if (play)
        {
            StopAllCoroutines();
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
            StartCoroutine(CO_Jump());
        }
    }

    private void FallingState(bool play, BitController.States nextState)
    {
        if (!fallEffect) return;

        if (play)
        {
            StartCoroutine(CO_Fall());
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
