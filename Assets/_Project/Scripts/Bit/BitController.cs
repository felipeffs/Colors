using UnityEngine;

public class BitController : MonoBehaviour
{
    private enum States
    {
        None,
        Idle,
        Walk,
        Jump,
        Falling,
        WallJump
    }

    [Header("Visual")]
    [SerializeField] private SpriteRenderer sr;

    [Header("Physics")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpMaxHeight = 3f;
    [SerializeField] private float wallJumpMaxHeight = 3f;
    [SerializeField] private float wallJumpDuration = .4f;
    private float _wallJumpTimer;
    private bool _isWallJumpCompleted;

    [Header("Collision Check")]
    [SerializeField] private Collider2D wallCheckCollider;
    [SerializeField] private Collider2D groundCheckCollider;
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private LayerMask groundLayers;

    //State Machine
    private States _currentState;
    private States _lastState;
    private States _nextState;
    private bool _firstCicle = false;

    private void Update()
    {
        CheckConditions();
        RunState();
        Flip();
    }

    private void CheckConditions()
    {
        switch (_currentState)
        {
            case States.Idle:

                if (InputManager.Instance.WalkRawValue() != 0)
                {
                    _nextState = States.Walk;
                }

                if (!IsGrounded())
                {
                    _nextState = States.Falling;
                }

                if (InputManager.Instance.JumpWasPressed())
                {
                    _nextState = States.Jump;
                }

                break;
            case States.Walk:

                if (InputManager.Instance.WalkWasReleased())
                {
                    _nextState = States.Idle;
                }
                if (InputManager.Instance.JumpWasPressed())
                {
                    _nextState = States.Jump;
                }
                if (!IsGrounded())
                {
                    _nextState = States.Falling;
                }

                break;
            case States.Jump:

                if (rb.velocity.y <= 0)
                {
                    _nextState = States.Falling;
                }

                if (IsTouchingWall() && InputManager.Instance.JumpWasPressed())
                {
                    _nextState = States.WallJump;
                }

                break;
            case States.Falling:

                if (IsGrounded())
                {
                    _nextState = States.Idle;
                }
                if (IsTouchingWall() && InputManager.Instance.JumpWasPressed())
                {
                    _nextState = States.WallJump;
                }

                break;
            case States.WallJump:

                if (!_isWallJumpCompleted) break;

                if (rb.velocity.y <= 0)
                {
                    _nextState = States.Falling;
                }

                if (IsTouchingWall())
                {
                    _nextState = States.Falling;
                }

                break;
            case States.None:

                _nextState = States.Idle;

                break;
        }
        if (_nextState != _currentState)
        {
            _firstCicle = true;
            _lastState = _currentState;
            _currentState = _nextState;
            Debug.Log(_currentState);
        }
        else
        {
            _firstCicle = false;
        }
    }

    private void RunState()
    {
        switch (_currentState)
        {
            case States.Idle:
                Idle();
                break;
            case States.Walk:
                Walk();
                break;
            case States.Jump:
                Jump();
                break;
            case States.Falling:
                Falling();
                break;
            case States.WallJump:
                WallJump();
                break;
        }
    }

    private void Idle()
    {
        rb.velocity = Vector2.zero;
    }

    private void Walk()
    {
        var horintalMovement = InputManager.Instance.WalkRawValue();
        rb.velocity = new Vector2(horintalMovement * walkSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        if (_firstCicle)
        {
            //jumpForce = squareRoot(jumpMaxHeight * gravity * -2) * mass
            var jumpForce = Mathf.Sqrt(jumpMaxHeight * (Physics2D.gravity.y * rb.gravityScale) * -2) *
                            rb.mass;
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        Walk();
    }

    private void Falling()
    {
        if (_lastState != States.WallJump)
        {
            Walk();
        }

        //if last state is wall jump only modify velocity if Walk has pressed
        else if (!InputManager.Instance.WalkWasPressed())
        {
            Walk();
        }
    }

    private void WallJump()
    {
        //Timer
        if (!_firstCicle)
        {
            _wallJumpTimer -= Time.deltaTime;
            if (_wallJumpTimer < 0)
            {
                _isWallJumpCompleted = true;
            }
            return;
        }

        //jumpForce = squareRoot(jumpMaxHeight * gravity * -2) * mass
        var jumpForce = Mathf.Sqrt(wallJumpMaxHeight * (Physics2D.gravity.y * rb.gravityScale) * -2) *
                        rb.mass;
        rb.velocity = Vector2.zero;

        var direction = sr.flipX == true
            ? new Vector2(1f, 1f).normalized
            : new Vector2(-1f, 1f).normalized;
        rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);

        //SetTimer
        _isWallJumpCompleted = false;
        _wallJumpTimer = wallJumpDuration;
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(groundCheckCollider.bounds.center, groundCheckCollider.bounds.size, 0f, Vector2.down, 0f, groundLayers);
        return raycastHit.collider != null;
    }

    private bool IsTouchingWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(wallCheckCollider.bounds.center, wallCheckCollider.bounds.size, 0f, Vector2.right, 0f, wallLayers);
        var isTouching = raycastHit.collider != null;
        return isTouching;
    }

    private void Flip()
    {
        if (rb.velocity.x < 0)
        {
            sr.flipX = true;
            wallCheckCollider.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rb.velocity.x > 0)
        {
            sr.flipX = false;
            wallCheckCollider.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
