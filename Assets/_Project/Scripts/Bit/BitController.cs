using UnityEngine;
using System;

public class BitController : MonoBehaviour, IReceiveDamage
{
    public static event Action OnPlayerDeath;

    [Header("Debug")]
    [SerializeField] private bool debugLines;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D bitCollider;
    [SerializeField] private BitAnimator animator;
    private GameObject _parent;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpMaxHeight = 3f;
    [SerializeField] private float wallJumpMaxHeight = 3f;
    [SerializeField] private float wallJumpDuration = .4f;
    [SerializeField] private float wallJumpPenaltyDuration = .75f;
    [SerializeField] private float fallAcceleration = 3f;
    [SerializeField] private float maxFallSpeed = 20f;
    private bool _isOnWallJumpPenalty;
    private float _wallJumpPenaltyTimer;
    private float _wallJumpTimer;
    private bool _isWallJumpCompleted = true;
    private Direction _lastWallJumpedDirection;
    Rigidbody2D groundRb;
    [ReadOnly][SerializeField] Vector2 groundVelocity;

    [Header("Collision Check")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float distanceFromGround = .45f;
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private float distanceFromWall = .35f;

    //State Machine
    public enum States
    {
        None,
        Dead,
        Idle,
        Walk,
        Jump,
        Falling,
        WallJump
    }

    [Header("State Machine")]
    [ReadOnly][SerializeField] private States _currentState;
    private States _lastState;
    private States _nextState;
    private bool _firstCicle = false;

    [Header("Input Buffer")]
    [SerializeField] private float jumpBufferWindow;
    private bool _jumpBuffered;
    private float _jumpBufferTimer;

    [Header("Coyote Time")]
    [SerializeField] private float _coyoteWindow = .4f;
    private float _coyoteTimer;
    private bool _coyoteJump;

    // Dead
    private bool _isDead;

    // Direction
    private enum Direction
    {
        Right = 1,
        Left = -1
    }

    private Direction _currentDirection;

    private void Awake()
    {
        _parent = gameObject.transform.parent.gameObject;
    }

    private void Update()
    {
        JumpBuffer();
        RunState();
        DebugCollisionCheck();
    }

    private void RunState()
    {
        _nextState = _currentState switch
        {
            States.Dead => Dead(),
            States.Idle => Idle(),
            States.Walk => Walk(),
            States.Jump => Jump(),
            States.Falling => Falling(),
            States.WallJump => WallJump(),
            _ => States.Idle
        };

        _nextState = GlobalTransitions(_nextState);

        // Change State
        if (_nextState != _currentState)
        {
            //animation stop
            animator.Action(false, _currentState, _nextState);

            _firstCicle = true;
            _lastState = _currentState;
            _currentState = _nextState;

            //animation play
            animator.Action(true, _currentState);
            Debug.Log(_currentState);

            //Remove ground ref
            groundVelocity = Vector2.zero;
        }
        else
        {
            _firstCicle = false;
        }
    }

    private States Idle()
    {
        rb.velocity = Vector2.zero;
        _coyoteTimer = _coyoteWindow;
        ApplyInertia();

        // Transitions
        if (!IsGrounded())
        {
            return States.Falling;
        }

        if (InputManager.Instance.WalkRawValue() != 0)
        {
            return States.Walk;
        }

        if (InputManager.Instance.JumpWasPressed() || _jumpBuffered)
        {
            return States.Jump;
        }

        return States.Idle;
    }

    private States Walk()
    {
        MoveHorizontally();

        // Transitions
        if (!IsGrounded())
        {
            return States.Falling;
        }
        if (InputManager.Instance.WalkRawValue() == 0)
        {
            return States.Idle;
        }
        if (InputManager.Instance.JumpWasPressed() || _jumpBuffered)
        {
            return States.Jump;
        }
        return States.Walk;
    }

    private States Jump()
    {
        if (_firstCicle)
        {
            //jumpForce = squareRoot(jumpMaxHeight * gravity * -2) * mass
            var jumpForce = Mathf.Sqrt(jumpMaxHeight * (Physics2D.gravity.y * rb.gravityScale) * -2) *
                            rb.mass;
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            ConsumeCoyoteTime();

            _jumpBuffered = false;
        }
        MoveHorizontally();

        // Transitions
        if (IsTouchingWall() && (InputManager.Instance.JumpWasPressed() || _jumpBuffered))
        {
            return States.WallJump;
        }
        if (rb.velocity.y <= 0)
        {
            return States.Falling;
        }
        return States.Jump;
    }

    private States Falling()
    {
        UpdateCoyoteTimer();

        // Wall Jump Penalty
        if (_firstCicle)
            _wallJumpPenaltyTimer = 0.5f;
        else
        {
            _wallJumpPenaltyTimer -= Time.deltaTime;
        }

        if (_wallJumpPenaltyTimer <= 0) _isOnWallJumpPenalty = false;

        //Fall Aceleration and Speed Limiter
        var newYVelocity = rb.velocity.y - fallAcceleration * Time.deltaTime;
        if (newYVelocity >= -maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, newYVelocity);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, newYVelocity);
        }

        //Move Horizontally
        if (_lastState != States.WallJump)
        {
            MoveHorizontally();
        }

        //if last state is wall jump only modify velocity if Walk has pressed
        else if (InputManager.Instance.WalkRawValue() != 0)
        {
            MoveWallJumping();
        }

        //Transitions
        if (IsGrounded())
        {
            return States.Idle;
        }

        else if (_coyoteJump)
        {
            Debug.Log("coyoteJump");
            return States.Jump;
        }

        if (IsTouchingWall() && (InputManager.Instance.JumpWasPressed() || _jumpBuffered))
        {
            return States.WallJump;
        }

        return States.Falling;
    }

    private States WallJump()
    {
        //Timer
        _wallJumpTimer -= Time.deltaTime;
        if (_wallJumpTimer < 0)
        {
            _isWallJumpCompleted = true;
        }

        if (_firstCicle)
        {
            //jumpForce = squareRoot(jumpMaxHeight * gravity * -2) * mass
            var jumpForce = Mathf.Sqrt(wallJumpMaxHeight * (Physics2D.gravity.y * rb.gravityScale) * -2) *
                            rb.mass;
            rb.velocity = Vector2.zero;

            var direction = new Vector2(_currentDirection == Direction.Left ? 1f : -1f, 1f).normalized;

            rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);

            //SetTimer
            _isWallJumpCompleted = false;
            _wallJumpTimer = wallJumpDuration;

            _lastWallJumpedDirection = _currentDirection;
            _isOnWallJumpPenalty = true;

            _jumpBuffered = false;
            ConsumeCoyoteTime();
            Flip();
        }

        //Transitions
        if (_isWallJumpCompleted)
        {
            return States.Falling;
        }

        return States.WallJump;
    }

    private States Dead()
    {
        if (_firstCicle)
        {
            OnPlayerDeath?.Invoke();
            _parent.SetActive(false);
        }

        //Transitions
        return _isDead ? States.Dead : States.Idle;
    }

    private States GlobalTransitions(States currentState)
    {
        return _isDead ? States.Dead : currentState;
    }

    private void MoveHorizontally()
    {
        var horintalMovement = InputManager.Instance.WalkRawValue();
        Move(new Vector2(horintalMovement * walkSpeed, rb.velocity.y));

        if (horintalMovement == 0) return;
        Flip();
    }

    private void Move(Vector2 speed)
    {
        if (!IsGrounded()) groundVelocity = Vector2.zero;

        if (_currentState == States.Walk)
        {
            rb.velocity = new Vector2(speed.x + groundVelocity.x, speed.y - 3f);
        }
        else if (_currentState == States.Jump)
        {
            rb.velocity = speed;
        }
        else
        {
            rb.velocity = groundVelocity + speed;
        }
    }

    private void ApplyInertia()
    {
        rb.velocity = groundVelocity + Vector2.up * rb.velocity.y;
    }

    private void MoveWallJumping()
    {
        var horintalMovement = InputManager.Instance.WalkRawValue();

        var airSpeedMultiplier = horintalMovement == (int)_lastWallJumpedDirection && _isOnWallJumpPenalty ? wallJumpPenaltyDuration : 1;

        Move(new Vector2(walkSpeed * airSpeedMultiplier * horintalMovement, rb.velocity.y));

        if (horintalMovement == 0) return;
        Flip();
    }

    private bool IsGrounded()
    {
        var centerBounds = bitCollider.bounds.center;

        var centerPoint = new Vector3(centerBounds.x, centerBounds.y - bitCollider.bounds.extents.y, centerBounds.z);
        RaycastHit2D raycastCenter = Physics2D.Raycast(centerPoint, Vector2.down, distanceFromGround, groundLayers);
        groundRb = raycastCenter.collider?.gameObject.GetComponent<Rigidbody2D>();

        if (groundRb != null)
            groundVelocity = groundRb.velocity;

        if (raycastCenter) return true;

        var leftPoint = new Vector3(centerBounds.x - bitCollider.bounds.extents.x, centerBounds.y - bitCollider.bounds.extents.y, centerBounds.z);
        RaycastHit2D raycastLeft = Physics2D.Raycast(leftPoint, Vector2.down, distanceFromGround, groundLayers);
        groundRb = raycastCenter.collider?.gameObject.GetComponent<Rigidbody2D>();

        if (groundRb != null)
            groundVelocity = groundRb.velocity;
        if (raycastLeft) return true;

        var rightPoint = new Vector3(centerBounds.x + bitCollider.bounds.extents.x, centerBounds.y - bitCollider.bounds.extents.y, centerBounds.z);
        RaycastHit2D raycastRight = Physics2D.Raycast(rightPoint, Vector2.down, distanceFromGround, groundLayers);
        groundRb = raycastCenter.collider?.gameObject.GetComponent<Rigidbody2D>();

        if (groundRb != null)
            groundVelocity = groundRb.velocity;
        if (raycastRight) return true;

        return false;
    }

    private bool IsTouchingWall()
    {
        int directionMultiplier = 0;
        Vector2 direction = Vector2.zero;

        if (_currentDirection == Direction.Left)
        {
            directionMultiplier = -1;
            direction = Vector2.left;
        }
        else
        {
            directionMultiplier = 1;
            direction = Vector2.right;
        }

        var wallPoint = new Vector3(bitCollider.bounds.center.x + (bitCollider.bounds.extents.x * directionMultiplier), bitCollider.bounds.center.y, bitCollider.bounds.center.z);
        RaycastHit2D raycastHit = Physics2D.Raycast(wallPoint, direction, distanceFromWall, wallLayers);
        return raycastHit;
    }

    private void DebugCollisionCheck()
    {
        if (!debugLines) return;

        int directionMultiplier = 0;
        Vector3 direction = Vector3.zero;

        //Wall
        if (_currentDirection == Direction.Left)
        {
            directionMultiplier = -1;
            direction = Vector3.left;
        }
        else
        {
            directionMultiplier = 1;
            direction = Vector3.right;
        }

        var wallPoint = new Vector3(bitCollider.bounds.center.x + (bitCollider.bounds.extents.x * directionMultiplier), bitCollider.bounds.center.y, bitCollider.bounds.center.z);
        Debug.DrawLine(wallPoint, wallPoint + (direction * distanceFromWall));

        //Ground
        var centerBounds = bitCollider.bounds.center;

        var centerPoint = new Vector3(centerBounds.x, centerBounds.y - bitCollider.bounds.extents.y, centerBounds.z);
        Debug.DrawLine(centerPoint, centerPoint + (Vector3.down * distanceFromGround));

        var leftPoint = new Vector3(centerBounds.x - bitCollider.bounds.extents.x, centerBounds.y - bitCollider.bounds.extents.y, centerBounds.z);
        Debug.DrawLine(leftPoint, leftPoint + (Vector3.down * distanceFromGround));

        var rightPoint = new Vector3(centerBounds.x + bitCollider.bounds.extents.x, centerBounds.y - bitCollider.bounds.extents.y, centerBounds.z);
        Debug.DrawLine(rightPoint, rightPoint + (Vector3.down * distanceFromGround));
    }

    private void Flip()
    {
        if (rb.velocity.x < 0)
        {
            _currentDirection = Direction.Left;
            animator.VisualFlip(true);
        }
        else if (rb.velocity.x > 0)
        {
            _currentDirection = Direction.Right;
            animator.VisualFlip(false);
        }
    }

    private void JumpBuffer()
    {
        if (_jumpBufferTimer > 0)
        {
            _jumpBufferTimer -= Time.deltaTime;
        }
        else
        {
            _jumpBuffered = false;
        }

        if (!IsGrounded() && InputManager.Instance.JumpWasPressed())
        {
            _jumpBuffered = true;
            _jumpBufferTimer = jumpBufferWindow;
        }
    }

    private void UpdateCoyoteTimer()
    {
        if (_coyoteTimer > 0)
        {
            _coyoteTimer -= Time.deltaTime;
            if (InputManager.Instance.JumpWasPressed())
            {
                _coyoteJump = true;
            }
        }
    }

    private void ConsumeCoyoteTime()
    {
        _coyoteJump = false;
        _coyoteTimer = 0;
    }

    public void Reset(Vector3 newPosition)
    {
        //
        _isDead = false;

        //Change Position
        _parent.transform.position = newPosition;

        //Reset Physics
        rb.velocity = Vector2.zero;

        //Reset JumpBuffer
        _jumpBuffered = false;
        _jumpBufferTimer = 0;

        //Reset CoyoteTime
        ConsumeCoyoteTime();

        //Reset Flip
        animator.VisualFlip(false);
    }

    public void TakeDamage(int damage)
    {
        _isDead = true;
    }
}
