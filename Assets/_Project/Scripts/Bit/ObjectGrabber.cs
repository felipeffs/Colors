using UnityEngine;
public class ObjectGrabber : MonoBehaviour
{
    [SerializeField] private Collider2D grabArea;
    [SerializeField] private LayerMask objectsMask;
    [SerializeField] private Transform positionHoldObject;
    private BitController _controller;
    private Collider2D _collider;
    private GrabbableObject _grabbableObject;
    private Transform _transformCaptured;

    private void Awake()
    {
        _controller = transform.parent.GetComponentInChildren<BitController>();
        _collider = transform.parent.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (InputManager.Instance.GrabWasPressed())
        {
            GrabNearestObject();
        }

        ChangeGrabAreaDirection();
    }

    private void ChangeGrabAreaDirection()
    {
        var performerCurrentDirection = _controller.GetCurrentDirection();
        var xOffsetHoldPosition = Mathf.Abs(positionHoldObject.localPosition.x);
        var xOffsetGrabArea = Mathf.Abs(grabArea.transform.localPosition.x);
        var multi = performerCurrentDirection == Direction.Left ? -1 : 1;

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
        _grabbableObject.Grab(positionHoldObject, _collider, () => { _grabbableObject = null; _transformCaptured = null; });
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
}