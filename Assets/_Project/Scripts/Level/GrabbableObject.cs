using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrabbableObject : MonoBehaviour
{
    [SerializeField] protected float grabSpeed;
    protected Action _performerDrop;
    protected Transform _grabPoint;
    protected Collider2D _collider;
    protected Collider2D _grabPerformerCollider;
    protected float _gravityScale;
    protected Rigidbody2D _rb;

    protected virtual void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rb = _collider.attachedRigidbody;
    }

    protected virtual void FixedUpdate()
    {
        MoveToGrabPoint();
    }

    private void MoveToGrabPoint()
    {
        if (_grabPoint == null) return;

        //var distance = Vector3.Distance(transform.position, _grabPoint.position);
        var blend = 1 - Mathf.Pow(0.5f, Time.deltaTime * grabSpeed);
        var newPosition = Vector2.Lerp(transform.position, _grabPoint.position, blend);
        _rb.MovePosition(newPosition);
    }

    public virtual void Grab(Transform grabPoint, Collider2D performerCollider, Action performerDrop)
    {
        _performerDrop = performerDrop;

        // Ignore Collider while grabbed
        Physics2D.IgnoreCollision(performerCollider, _collider, true);

        _grabPerformerCollider = performerCollider;
        _gravityScale = _rb.gravityScale;
        _rb.gravityScale = 0;
        _grabPoint = grabPoint;
        return;
    }

    public void Drop()
    {
        if (_grabPerformerCollider == null) return;

        _performerDrop?.Invoke();
        Physics2D.IgnoreCollision(_grabPerformerCollider, _collider, false);
        _rb.gravityScale = _gravityScale;
        _grabPerformerCollider = null;
        _grabPoint = null;
        _rb.velocity = Vector2.zero;
    }
}
