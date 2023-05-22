using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(Attacher))]
public class ActivatorCube : MonoBehaviour, IReceiveDamage
{
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float distanceFromGround = 0.2f;

    // Const Ref
    private Attacher magnetismProp;
    private Collider2D cubeCollider;
    private Rigidbody2D cubeRb;

    // Dinamic Ref
    private Collider2D other;
    private IAction behaviourConnector;

    // Connected Variables
    private bool isConnected = false;
    private Vector3 initialPos;

    public void Awake()
    {
        initialPos = transform.position;
        magnetismProp = GetComponent<Attacher>();
        cubeCollider = GetComponent<Collider2D>();
        cubeRb = GetComponent<Rigidbody2D>();
    }

    public void Start()
    {
        LevelManager.OnRestartLevel += LevelManager_OnRestartLevel;
    }

    private void OnDestroy()
    {
        LevelManager.OnRestartLevel -= LevelManager_OnRestartLevel;
    }

    public void Update()
    {
        if (!isConnected)
        {
            GroundCheck();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (isConnected)
            {
                Unplug();
            }
            else
            {
                Plug();
            }
        }
    }

    private void Unplug()
    {
        isConnected = false;
        behaviourConnector?.Desactive();
        behaviourConnector = null;
        magnetismProp?.DetachAll();
    }

    private void GroundCheck()
    {
        //CollisionCheck
        Vector3 colliderCenterPos = cubeCollider.bounds.center;

        // Checking order: center, left, right
        float[] groundCheckPoints = {colliderCenterPos.x,
             colliderCenterPos.x - cubeCollider.bounds.extents.x,
              colliderCenterPos.x + cubeCollider.bounds.extents.x};

        foreach (var point in groundCheckPoints)
        {
            var checkPoint = new Vector3(point, colliderCenterPos.y - cubeCollider.bounds.extents.y, colliderCenterPos.z);

            RaycastHit2D raycast = Physics2D.Raycast(checkPoint, Vector2.down, distanceFromGround, groundLayers);

            if (raycast.collider != null)
            {
                other = raycast.collider;
                Vector2 groundVelocity = raycast.collider.gameObject.GetComponent<Rigidbody2D>().velocity;

                //Velocity
                if (groundVelocity != Vector2.zero)
                {
                    if (Mathf.Abs(cubeRb.velocity.x) < Mathf.Abs(groundVelocity.x))
                        cubeRb.velocity = Vector2.right * groundVelocity.x + cubeRb.velocity;
                    if (Mathf.Abs(cubeRb.velocity.y) < Mathf.Abs(groundVelocity.y))
                        cubeRb.velocity = Vector2.up * groundVelocity.y + cubeRb.velocity; ;

                    break;
                }
            }
        }
    }

    public void Plug()
    {
        if (isConnected) return;
        if (other == null) return;

        behaviourConnector = other.GetComponent<IAction>();

        if (behaviourConnector is null) return;

        isConnected = true;
        behaviourConnector?.Active();
        magnetismProp?.AttachObject(other.transform, cubeRb);
    }

    private void Reset()
    {
        Unplug();
        transform.position = initialPos;
        cubeRb.velocity = Vector2.zero;
    }

    private void LevelManager_OnRestartLevel()
    {
        Reset();
    }

    void IReceiveDamage.TakeDamage(int damage)
    {
        Reset();
    }
}