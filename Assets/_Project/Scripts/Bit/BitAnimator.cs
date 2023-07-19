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

#if UNITY_EDITOR
    [Header("Testing")]
    [SerializeField] private bool jumpEffect;
    [SerializeField] private bool walkEffect;
    [SerializeField] private bool fallEffect;
#endif

    //Flip
    private readonly Vector3 LocalFlippedScale = new Vector3(-1, 1, 1);
    private readonly Vector3 LocalNoFlippedScale = Vector3.one;

    //Animations String Hashs
    private readonly int Idle = Animator.StringToHash("Idle");
    private readonly int Walk = Animator.StringToHash("Walk");
    private readonly int JumpImpulse = Animator.StringToHash("TakeOff");
    private readonly int Jump = Animator.StringToHash("Jump");
    private readonly int JumpToFall = Animator.StringToHash("JumpToFall");
    private readonly int FallLoop = Animator.StringToHash("FallLoop");

    [Header("Animations")]
    [SerializeField] private float jumpToFallDuration = 15f;
    [SerializeField] private float takeOffDuration = 10f;

    public void Action(bool play, BitController.State currentState)
    {
        Action(play, currentState, currentState);
    }

    public void Action(bool play, BitController.State currentState, BitController.State nextState)
    {
        StopAllCoroutines();
        switch (currentState)
        {
            case BitController.State.Idle:
                IdleState(play);
                break;
            case BitController.State.Walk:
                WalkState(play);
                break;
            case BitController.State.Jump:
                JumpState(play, nextState);
                break;
            case BitController.State.WallJump:
                JumpState(play, nextState);
                break;
            case BitController.State.Falling:
                FallingState(play, nextState);
                break;
            case BitController.State.Dead:
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
#if UNITY_EDITOR
        if (!walkEffect) return;
#endif

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

    private void JumpState(bool play, BitController.State nextState)
    {
#if UNITY_EDITOR
        if (!jumpEffect) return;
#endif
        if (play)
        {
            jumpDust.Play();
            StartCoroutine(CO_Jump());
        }
    }

    private void FallingState(bool play, BitController.State nextState)
    {
#if UNITY_EDITOR
        if (!fallEffect) return;
#endif

        if (play)
        {
            StartCoroutine(CO_Fall());
        }
        else
        {
            if (nextState == BitController.State.Idle)
                jumpDust.Play();
        }
    }

    public void VisualFlip(bool flip)
    {
        sr.flipX = flip;
        var newLocalScale = flip ? LocalFlippedScale : LocalNoFlippedScale;
        walkTrail.transform.localScale = newLocalScale;
    }
}
