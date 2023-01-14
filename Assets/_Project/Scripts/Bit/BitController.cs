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

    [Header("Collision Check")]
    [SerializeField] private Collider2D wallCheckCollider;
    [SerializeField] private Collider2D groundCheckCollider;
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float distance;


    private States _currentState;


    private bool _firstCicle = false;

    private void Update()
    {
        CheckConditions();
        RunState();
        Flip();
        Debug.Log(_currentState);
    }

    private void CheckConditions()
    {
        if (_firstCicle)
        {
            _firstCicle = false;
        }

        switch (_currentState)
        {
            case States.Idle:

                if (InputManager.Instance.WalkRawValue() != 0)
                {
                    _currentState = States.Walk;
                    _firstCicle = true;
                }

                if (!IsGrounded())
                {
                    _currentState = States.Falling;
                    _firstCicle = true;
                }

                if (InputManager.Instance.JumpWasPressed())
                {
                    _currentState = States.Jump;
                    _firstCicle = true;
                }

                break;
            case States.Walk:

                if (InputManager.Instance.WalkWasReleased())
                {
                    _currentState = States.Idle;
                    _firstCicle = true;

                }
                if (InputManager.Instance.JumpWasPressed())
                {
                    _currentState = States.Jump;
                    _firstCicle = true;

                }
                if (!IsGrounded())
                {
                    _currentState = States.Falling;
                    _firstCicle = true;
                }

                break;
            case States.Jump:

                if (rb.velocity.y <= 0)
                {
                    _currentState = States.Falling;
                    _firstCicle = true;
                }

                break;
            case States.Falling:

                if (IsGrounded())
                {
                    _currentState = States.Idle;
                    _firstCicle = true;
                }

                break;
            case States.WallJump:
                break;
            default:

                _currentState = States.Idle;

                break;
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
        Walk();
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(groundCheckCollider.bounds.center, groundCheckCollider.bounds.size, 0f, Vector2.down, distance, groundLayers);
        return raycastHit.collider != null;
    }

    private bool IsTouchingWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(wallCheckCollider.bounds.center, wallCheckCollider.bounds.size, 0f, Vector2.right, distance, wallLayers);
        return raycastHit.collider != null;
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
