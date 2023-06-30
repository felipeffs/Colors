using UnityEngine;
using System;

public class BitController : MonoBehaviour, IReceiveDamage
{
    public static event Action OnPlayerDeath;
    public static event Action<State> OnStateChange;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool debugLines;
#endif

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D bitCollider;
    [SerializeField] private BitAnimator animator;
    private GameObject _parent;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpMaxHeight = 4.5f;
    [SerializeField] private float wallJumpMaxHeight = 4f;
    [SerializeField] private float wallJumpDuration = .3f;
    [SerializeField] private float wallJumpPenaltyDuration = .75f;
    [SerializeField] private float fallAcceleration = 15f;
    [SerializeField] private float maxFallSpeed = 20f;
    private bool _isOnWallJumpPenalty;
    private float _wallJumpPenaltyTimer;
    private float _wallJumpTimer;
    private bool _isWallJumpCompleted = true;
    private Direction _lastWallJumpedDirection;
    private Rigidbody2D _groundRb;
    [ReadOnly][SerializeField] Vector2 groundVelocity;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float distanceFromGround = .04f;

    [Header("Wall Detection")]
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private float distanceFromWall = .02f;

    //State Machine
    public enum State
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
    [ReadOnly][SerializeField] private State _currentState;
    private State _lastState;
    private State _nextState;
    private bool _firstCicle = false;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteWindow = .2f;
    private float _coyoteTimer;
    private bool _coyoteJump;

    // Dead
    private bool _isDead;

    private Direction _currentDirection;

    private void Awake()
    {
        _parent = gameObject.transform.parent.gameObject;
    }

    private void Update()
    {
        RunState();
#if UNITY_EDITOR
        DebugCollisionCheck();
#endif
    }

    private void RunState()
    {
        _nextState = _currentState switch
        {
            State.Dead => Dead(),
            State.Idle => Idle(),
            State.Walk => Walk(),
            State.Jump => Jump(),
            State.Falling => Falling(),
            State.WallJump => WallJump(),
            _ => State.Idle
        };

        _nextState = GlobalTransitions(_nextState);

        // Change State
        if (_nextState != _currentState)
        {
            //animation stop
            animator?.Action(false, _currentState, _nextState);

            _firstCicle = true;
            _lastState = _currentState;
            _currentState = _nextState;

            //animation play
            animator?.Action(true, _currentState);
            Debug.Log(_currentState);

            OnStateChange?.Invoke(_currentState);

            //Reset ground velocity
            groundVelocity = Vector2.zero;
        }
        else
        {
            _firstCicle = false;
        }
    }

    private State Idle()
    {
        rb.velocity = Vector2.zero;
        _coyoteTimer = coyoteWindow;
        ApplyInertia();

        // Transitions
        if (!IsGrounded())
        {
            return State.Falling;
        }

        if (InputManager.Instance.WalkRawValue() != 0)
        {
            return State.Walk;
        }

        if (InputManager.Instance.JumpWasPressed())
        {
            return State.Jump;
        }

        return State.Idle;
    }

    private State Walk()
    {
        MoveHorizontally();

        // Transitions
        if (!IsGrounded())
        {
            return State.Falling;
        }
        if (InputManager.Instance.WalkRawValue() == 0)
        {
            return State.Idle;
        }
        if (InputManager.Instance.JumpWasPressed())
        {
            return State.Jump;
        }
        return State.Walk;
    }

    private State Jump()
    {
        if (_firstCicle)
        {
            //jumpForce = squareRoot(jumpMaxHeight * gravity * -2) * mass
            var jumpForce = Mathf.Sqrt(jumpMaxHeight * (Physics2D.gravity.y * rb.gravityScale) * -2) *
                            rb.mass;
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            ConsumeCoyoteTime();
        }
        MoveHorizontally();

        // Transitions
        if (IsTouchingWall() && (InputManager.Instance.JumpWasPressed()))
        {
            return State.WallJump;
        }
        if (rb.velocity.y <= 0)
        {
            return State.Falling;
        }
        return State.Jump;
    }

    private State Falling()
    {
        UpdateCoyoteTimer();

        // Wall Jump Penalty
        if (_firstCicle)
        {
            _wallJumpPenaltyTimer = 0.5f;
            if (_lastState != State.WallJump) rb.velocity = Vector2.zero;
        }

        else
        {
            _wallJumpPenaltyTimer -= Time.deltaTime;
        }

        if (_wallJumpPenaltyTimer <= 0) _isOnWallJumpPenalty = false;

        // Fall Aceleration and Speed Limiter
        var newYVelocity = rb.velocity.y - fallAcceleration * Time.deltaTime;
        if (newYVelocity >= -maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, newYVelocity);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
        }

        //Move Horizontally
        if (_lastState != State.WallJump)
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
            return State.Idle;
        }

        else if (_coyoteJump)
        {
            Debug.Log("coyoteJump");
            return State.Jump;
        }

        if (IsTouchingWall() && (InputManager.Instance.JumpWasPressed()))
        {
            return State.WallJump;
        }

        return State.Falling;
    }

    private State WallJump()
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

            ConsumeCoyoteTime();
            Flip();
        }

        //Transitions
        if (_isWallJumpCompleted)
        {
            return State.Falling;
        }

        return State.WallJump;
    }

    private State Dead()
    {
        if (_firstCicle)
        {
            OnPlayerDeath?.Invoke();
            _parent.SetActive(false);
        }

        //Transitions
        return _isDead ? State.Dead : State.Idle;
    }

    private State GlobalTransitions(State currentState)
    {
        return _isDead ? State.Dead : currentState;
    }

    private void ApplyInertia()
    {
        rb.velocity = groundVelocity + Vector2.up * rb.velocity.y;
    }

    private void Move(Vector2 speed)
    {
        if (!IsGrounded()) groundVelocity = Vector2.zero;

        if (_currentState == State.Walk)
        {
            rb.velocity = new Vector2(speed.x + groundVelocity.x, speed.y - 3f);
        }
        else if (_currentState == State.Jump)
        {
            rb.velocity = speed;
        }
        else
        {
            rb.velocity = groundVelocity + speed;
        }
    }

    private void MoveHorizontally()
    {
        var horintalMovement = InputManager.Instance.WalkRawValue();
        Move(new Vector2(horintalMovement * walkSpeed, rb.velocity.y));

        if (horintalMovement == 0) return;
        Flip();
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
        Vector3 colliderCenterPos = bitCollider.bounds.center;

        // Checking order: center, left, right
        float[] groundCheckPoints = { colliderCenterPos.x,
             colliderCenterPos.x - bitCollider.bounds.extents.x,
              colliderCenterPos.x + bitCollider.bounds.extents.x};

        foreach (var point in groundCheckPoints)
        {
            var checkPoint = new Vector3(point, colliderCenterPos.y - bitCollider.bounds.extents.y, colliderCenterPos.z);

            RaycastHit2D raycastHit = Physics2D.Raycast(checkPoint, Vector2.down, distanceFromGround, groundLayers);
            _groundRb = raycastHit.collider?.gameObject.GetComponent<Rigidbody2D>();

            if (_groundRb != null)
                groundVelocity = _groundRb.velocity;

            if (raycastHit) return true;
        }

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

#if UNITY_EDITOR
    private void DebugCollisionCheck()
    {
        if (!debugLines) return;

        int directionMultiplier = 0;
        Vector3 direction = Vector3.zero;

        Vector3 colliderCenterPos = bitCollider.bounds.center;

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

        var wallPoint = new Vector3(colliderCenterPos.x + (bitCollider.bounds.extents.x * directionMultiplier), colliderCenterPos.y, colliderCenterPos.z);
        Debug.DrawLine(wallPoint, wallPoint + (direction * distanceFromWall));

        //Ground
        // Debug order: center, left, right
        float[] groundCheckPoints = { colliderCenterPos.x ,
             colliderCenterPos.x - bitCollider.bounds.extents.x ,
              colliderCenterPos.x + bitCollider.bounds.extents.x  };

        foreach (var point in groundCheckPoints)
        {
            var checkPos = new Vector3(point, colliderCenterPos.y - bitCollider.bounds.extents.y, colliderCenterPos.z);
            Debug.DrawLine(checkPos, checkPos + (Vector3.down * distanceFromGround));
        }
    }
#endif

    private void Flip()
    {
        if (rb.velocity.x < 0)
        {
            _currentDirection = Direction.Left;
            animator?.VisualFlip(true);
        }
        else if (rb.velocity.x > 0)
        {
            _currentDirection = Direction.Right;
            animator?.VisualFlip(false);
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
        //Reset death flag
        _isDead = false;

        //Change Position
        _parent.transform.position = newPosition;

        //Reset Physics
        rb.velocity = Vector2.zero;

        //Reset CoyoteTime
        ConsumeCoyoteTime();

        //Reset Direction
        _currentDirection = Direction.Right;
        animator?.VisualFlip(false);
    }

    public void TakeDamage(int damage)
    {
        _isDead = true;
    }

    public Direction GetCurrentDirection() => _currentDirection;
}

// Direction
public enum Direction
{
    Right = 1,
    Left = -1
}