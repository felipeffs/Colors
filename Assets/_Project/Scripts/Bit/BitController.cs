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

    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferWindow;
    private bool _jumpBuffered;
    private float _jumpBufferTimer;

    [Header("Coyote Time")]
    [SerializeField] private float _coyoteWindow = .4f;
    private float _coyoteTimer;
    private bool _coyoteJump;

    private void Update()
    {
        JumpBuffer();
        RunState();
        Flip();
    }

    private void RunState()
    {
        _nextState = _currentState switch
        {
            States.Idle => Idle(),
            States.Walk => Walk(),
            States.Jump => Jump(),
            States.Falling => Falling(),
            States.WallJump => WallJump(),
            _ => States.Idle
        };

        // Change State
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

    private States Idle()
    {
        rb.velocity = Vector2.zero;

        _coyoteTimer = _coyoteWindow;

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

        //Transitions
        if (!IsGrounded())
        {
            return States.Falling;
        }
        if (InputManager.Instance.WalkWasReleased())
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
        }
        MoveHorizontally();

        //Transitions
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

        if (_lastState != States.WallJump)
        {
            MoveHorizontally();
        }

        //if last state is wall jump only modify velocity if Walk has pressed
        else if (InputManager.Instance.WalkRawValue() != 0)
        {
            MoveHorizontally();
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

        if (_firstCicle)
        {
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

            ConsumeCoyoteTime();
        }

        //Timer
        _wallJumpTimer -= Time.deltaTime;
        if (_wallJumpTimer < 0)
        {
            _isWallJumpCompleted = true;
        }

        //Transitions
        if (!_isWallJumpCompleted) return States.WallJump;

        if (rb.velocity.y <= 0)
        {
            return States.Falling;
        }

        if (IsTouchingWall())
        {
            return States.Falling;
        }

        return States.WallJump;
    }

    private void MoveHorizontally()
    {
        var horintalMovement = InputManager.Instance.WalkRawValue();
        rb.velocity = new Vector2(horintalMovement * walkSpeed, rb.velocity.y);
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

    public void ConsumeCoyoteTime()
    {
        _coyoteJump = false;
        _coyoteTimer = 0;
    }
}
