using UnityEngine;
using System;

public class ObjectGrabber : MonoBehaviour
{
    public static event Action<ObjectGrabber> OnStart;
    [SerializeField] private Collider2D grabArea;
    [SerializeField] private LayerMask objectsMask;
    [SerializeField] private Transform positionHoldObject;
    [SerializeField] private float maxGrabDistance = 3.5f;
    private BitController _controller;
    private Collider2D _collider;
    [SerializeField] private GrabbableObject _grabbableObject;
    private Transform _transformCaptured;

    private float xOffsetHoldPosition;
    private float xOffsetGrabArea;
    private float _timer;
    [SerializeField] private float cooldownTime = .25f;


    private void Awake()
    {
        _controller = transform.parent.GetComponentInChildren<BitController>();
        _collider = transform.parent.GetComponent<Collider2D>();
    }

    private void Start()
    {
        OnStart?.Invoke(this);
        xOffsetHoldPosition = Mathf.Abs(positionHoldObject.localPosition.x);
        xOffsetGrabArea = Mathf.Abs(grabArea.transform.localPosition.x);
    }

    private void OnEnable()
    {
        BitController.OnPlayerDeath += BitController_OnPlayerDeath;
    }

    private void OnDisable()
    {
        BitController.OnPlayerDeath -= BitController_OnPlayerDeath;
    }

    private void BitController_OnPlayerDeath()
    {
        if (_grabbableObject == null) return;
        _grabbableObject.Drop();
    }

    private void Update()
    {
        ChangeGrabAreaDirection();

        if (InputManager.Instance.GrabWasPressed() && _timer <= 0)
        {
            _timer = cooldownTime;
            print(_timer);
            GrabNearestObject();
            return;
        }

        _timer -= Time.deltaTime;
    }

    private void ChangeGrabAreaDirection()
    {
        var multi = _controller.GetCurrentDirection() == Direction.Left ? -1 : 1;

        positionHoldObject.localPosition = new Vector2(xOffsetHoldPosition * multi, positionHoldObject.localPosition.y);
        grabArea.transform.localPosition = new Vector2(xOffsetGrabArea * multi, grabArea.transform.localPosition.y);
    }

    private void GrabNearestObject()
    {
        if (_grabbableObject != null)
        {
            _grabbableObject.Drop();
            return;
        }

        Collider2D colliderCaptured = Physics2D.OverlapBox(new Vector2(grabArea.bounds.center.x, grabArea.bounds.center.y), new Vector2(grabArea.bounds.size.x, grabArea.bounds.size.y), 0f, objectsMask);

        if (colliderCaptured == null) return;

        // Guard clause: checks if it has captured a grabbable object
        if (!colliderCaptured.TryGetComponent<GrabbableObject>(out _grabbableObject)) return;
        _grabbableObject.Grab(positionHoldObject, _collider, () => { _grabbableObject = null; _transformCaptured = null; }, maxGrabDistance);
        _transformCaptured = colliderCaptured.transform;
    }

    public bool TryGetGrabbedTransform(out Transform grabbedTransform)
    {
        grabbedTransform = _transformCaptured;
        return grabbedTransform == null ? false : true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(grabArea.bounds.center, new Vector3(grabArea.bounds.size.x, grabArea.bounds.size.y, 0));
    }
#endif

    public float GetCooldownTimer() => cooldownTime;
    public bool isRecharging() => _timer > 0;
}