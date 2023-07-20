using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class ActivatorCube : GrabbableObject, IReceiveDamage, IInteractable
{
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float distanceFromGround = 0.2f;

    // Connection
    private Collider2D _other;
    private Connector _connector;
    private bool _isConnected = false;

    // Starter Configuration
    private Vector3 _initialPos;
    private ColorSwapper colorSwapper;

    protected override void Awake()
    {
        base.Awake();
        _initialPos = transform.position;
    }

    private void Start()
    {

        LevelManager.OnRestartLevel += LevelManager_OnRestartLevel;
        if (TryGetComponent<ColorSwapper>(out colorSwapper))
        {
            colorSwapper.DoOnColorChange += ColorSwapper_DoOnColorChange;
        }
    }

    private void OnDestroy()
    {
        LevelManager.OnRestartLevel -= LevelManager_OnRestartLevel;
        if (colorSwapper != null)
        {
            colorSwapper.DoOnColorChange -= ColorSwapper_DoOnColorChange;
        }
    }

    private void Update()
    {
        if (_isConnected) return;

        GroundCheck();
    }

    private void Unplug()
    {
        _isConnected = false;
        _connector?.DetachAll();
    }

    private void GroundCheck()
    {
        // CollisionCheck
        Bounds colliderBounds = _grabPerformerCollider != null ? _grabPerformerCollider.bounds : _collider.bounds;

        // Checking order: center, left, right
        float[] groundCheckPoints = {colliderBounds.center.x,
             colliderBounds.center.x - colliderBounds.extents.x,
              colliderBounds.center.x + colliderBounds.extents.x};

        foreach (var point in groundCheckPoints)
        {
            var checkPoint = new Vector3(point, colliderBounds.center.y - colliderBounds.extents.y, colliderBounds.center.z);

            RaycastHit2D raycastHit = Physics2D.Raycast(checkPoint, Vector2.down, distanceFromGround, groundLayers);

            if (raycastHit.collider != null)
            {
                _other = raycastHit.collider;
                Vector2 groundVelocity = _other.gameObject.GetComponent<Rigidbody2D>().velocity;

                //Velocity
                if (groundVelocity != Vector2.zero)
                {
                    if (Mathf.Abs(_rb.velocity.x) < Mathf.Abs(groundVelocity.x))
                        _rb.velocity = Vector2.right * groundVelocity.x + _rb.velocity;
                    if (Mathf.Abs(_rb.velocity.y) < Mathf.Abs(groundVelocity.y))
                        _rb.velocity = Vector2.up * groundVelocity.y + _rb.velocity; ;

                    return;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (_grabPerformerCollider == null) return;

        Bounds colliderBounds = _grabPerformerCollider != null ? _grabPerformerCollider.bounds : _collider.bounds;
        float[] groundCheckPoints = {colliderBounds.center.x,
             colliderBounds.center.x - colliderBounds.extents.x,
              colliderBounds.center.x + colliderBounds.extents.x};
        foreach (var point in groundCheckPoints)
        {
            var checkPoint = new Vector3(point, colliderBounds.center.y - colliderBounds.extents.y, colliderBounds.center.z);
            Debug.DrawLine(checkPoint, (Vector2)checkPoint + (distanceFromGround * Vector2.down), Color.blue);
        }

    }

    private void Plug()
    {
        if (_other == null) return;

        Plug(_other);
    }

    private void Plug(Collider2D toConnect)
    {
        if (!_other.TryGetComponent<Connector>(out _connector)) return;

        if (_connector.AttachObject(_collider))
        {
            _isConnected = true;
            Drop();
        }
    }

    private void Reset()
    {
        Unplug();
        Drop();
        transform.position = _initialPos;
        _rb.velocity = Vector2.zero;
    }

    private void LevelManager_OnRestartLevel()
    {
        Reset();
    }

    // When fall off the level reset the cube
    void IReceiveDamage.TakeDamage(int damage)
    {
        Reset();
    }

    public override void Grab(Transform grabPoint, Collider2D performerCollider, Action performerDrop)
    {
        base.Grab(grabPoint, performerCollider, performerDrop);
        Unplug();
    }

    // Action to perform on interact
    void IInteractable.Interact()
    {
        if (_isConnected)
        {
            Unplug();
        }
        else
        {
            Plug();
        }
    }

    private void ColorSwapper_DoOnColorChange(bool isActive)
    {
        if (isActive) return;
        Drop();
        Unplug();
    }

    public ColorSwapHandler.ColorID GetColor()
    {
        return colorSwapper?.TileColor ?? ColorSwapHandler.ColorID.None;
    }
}