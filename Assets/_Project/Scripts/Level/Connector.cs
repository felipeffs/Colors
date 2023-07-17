using UnityEngine;

public class Connector : MonoBehaviour
{
    [SerializeField] private Transform socket;
    private Collider2D _objectAttached;
    private Collider2D _collider;
    [SerializeField] private ConnectorAction connectorAction;

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public bool AttachObject(Collider2D objectToAttach)
    {
        if (_objectAttached != null) return false;

        _objectAttached = objectToAttach;
        var body = _objectAttached.attachedRigidbody;

        //Resetting the velocity after setting it to Kinematic to prevent it from moving away
        objectToAttach.attachedRigidbody.bodyType = RigidbodyType2D.Kinematic;
        objectToAttach.attachedRigidbody.velocity = Vector2.zero;

        //Setting the new position
        objectToAttach.transform.rotation = Quaternion.identity;
        objectToAttach.transform.position = socket.position;
        objectToAttach.transform.SetParent(transform);

        connectorAction.Execute();

        Physics2D.IgnoreCollision(_collider, _objectAttached, true);
        return true;
    }

    public void DetachAll()
    {
        if (_objectAttached == null) return;

        _objectAttached.transform.SetParent(null);
        _objectAttached.attachedRigidbody.bodyType = RigidbodyType2D.Dynamic;
        Physics2D.IgnoreCollision(_collider, _objectAttached, false);
        _objectAttached = null;
        connectorAction.Execute();
    }
}
