using UnityEngine;

public class BitAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer sr;

    [Header("Particles")]
    [SerializeField] private ParticleSystem walkTrail;
    [SerializeField] private ParticleSystem jumpDust;

    [Header("Testing")]
    [SerializeField] private bool jumpEffect;
    [SerializeField] private bool walkEffect;
    [SerializeField] private bool fallEffect;

    public bool isFliped { get => sr.flipX; }

    private readonly Vector3 localFlippedScale = new Vector3(-1, 1, 1);
    private readonly Vector3 localNoFlippedScale = new Vector3(1, 1, 1);

    public void Action(bool play, BitController.States state)
    {
        switch (state)
        {
            case BitController.States.Walk:
                Walk(play);
                break;
            case BitController.States.Jump:
                Jump(play);
                break;
            case BitController.States.Falling:
                Falling(play);
                break;
            case BitController.States.Idle:
                break;
            case BitController.States.WallJump:
                Jump(play);
                break;
            case BitController.States.Dead:
                break;

            default:
                break;
        }

    }

    private void Walk(bool play)
    {
        if (!walkEffect) return;

        if (play)
        {
            walkTrail.Play();
        }
        else
        {
            walkTrail.Stop();
        }
    }

    private void Jump(bool play)
    {
        if (!jumpEffect) return;

        if (play)
        {
            jumpDust.Play();
        }
        else
        {
            jumpDust.Stop();
        }
    }

    private void Falling(bool play)
    {
        if (!fallEffect) return;

        if (!play)
        {
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
