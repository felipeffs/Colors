using UnityEngine;

public class ActivatorCube : MonoBehaviour
{
    [SerializeField] private IAction behaviourConnector;
    [SerializeField] private Magnetism magnetismProp;
    [SerializeField] private bool isConnected = false;
    [SerializeField] private Collider2D cubeCollider;
    [SerializeField] private Rigidbody2D cubeRb;
    [SerializeField] private Rigidbody2D _groundRb;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float distanceFromGround = 0.5f;
    [SerializeField] private Collider2D other = null;
    private Vector3 initialPos;

    public void Awake()
    {
        initialPos = transform.position;
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
        GroundCheck();

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (isConnected)
            {
                Desconectar();
            }
            else
            {
                Conectar();
            }
        }

    }

    private void Desconectar()
    {
        isConnected = false;
        behaviourConnector?.Desactive();
        behaviourConnector = null;
        magnetismProp?.DesdoMagneticThing();
    }

    private void GroundCheck()
    {
        //CollisionCheck
        Vector3 colliderCenterPos = cubeCollider.bounds.center;

        // Checking order: center, left, right
        float[] groundCheckPoints = { colliderCenterPos.x,
             colliderCenterPos.x - cubeCollider.bounds.extents.x,
              colliderCenterPos.x + cubeCollider.bounds.extents.x};

        foreach (var point in groundCheckPoints)
        {
            var checkPoint = new Vector3(point, colliderCenterPos.y - cubeCollider.bounds.extents.y, colliderCenterPos.z);

            RaycastHit2D raycast = Physics2D.Raycast(checkPoint, Vector2.down, distanceFromGround, groundLayers);

            if (raycast.collider != null)
            {
                other = raycast.collider;
                _groundRb = raycast.collider.gameObject.GetComponent<Rigidbody2D>();

                //Velocity
                if (!isConnected && _groundRb)
                {
                    if (Mathf.Abs(cubeRb.velocity.x) < Mathf.Abs(_groundRb.velocity.x))
                        cubeRb.velocity = Vector2.right * _groundRb.velocity.x + cubeRb.velocity;
                    if (Mathf.Abs(cubeRb.velocity.y) < Mathf.Abs(_groundRb.velocity.y))
                        cubeRb.velocity = Vector2.up * _groundRb.velocity.y + cubeRb.velocity; ;

                    break;
                }


            }

        }
    }

    public void Conectar()
    {
        if (isConnected) return;
        if (other == null) return;

        var scripts = other.GetComponents<MonoBehaviour>();

        // IBlah  myListTest= originalObject as IBlah ?? some_code

        foreach (var script in scripts)
        {
            if (script is IAction iscript)
            {
                behaviourConnector = iscript;
                Debug.Log("BBB");
                break;
            }
        }

        if (behaviourConnector is null) return;

        isConnected = true;
        behaviourConnector?.Active();
        magnetismProp?.DoMagneticThing(other.transform);
        Debug.Log("CCC");
    }

    private void Reset()
    {
        Desconectar();
        transform.position = initialPos;
    }

    private void LevelManager_OnRestartLevel()
    {
        Reset();
    }
}