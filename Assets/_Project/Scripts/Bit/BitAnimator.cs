using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [Header("Walk")]
    [SerializeField] private ParticleSystem walkTrail;
    public bool isFliped { get => sr.flipX; }

    private readonly Vector3 localFlippedScale = new Vector3(-1, 1, 1);
    private readonly Vector3 localNoFlippedScale = new Vector3(1, 1, 1);

    public void Play(BitController.States state)
    {
        if (state == BitController.States.Walk)
        {
            walkTrail.Play();
        }
        else
        {
            walkTrail.Stop();
        }
    }

    public void VisualFlip(bool flip)
    {
        sr.flipX = flip;
        var newLocalScale = flip ? localFlippedScale : localNoFlippedScale;
        walkTrail.transform.localScale = newLocalScale;
    }
}
