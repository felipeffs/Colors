using UnityEngine;
using System;

public class Connector : MonoBehaviour
{
    [SerializeField] private Transform socket;
    private Collider2D _objectAttached;
    private Collider2D _collider;
    [SerializeField] private ConnectorAction[] connectorActions;
    private ColorSwapper _colorSwapper;

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _colorSwapper = GetComponent<ColorSwapper>();
    }

    void Start()
    {
        if (_colorSwapper)
            _colorSwapper.DoOnColorChange += ColorSwapper_DoOnColorChange;
    }

    void OnDestroy()
    {
        if (_colorSwapper)
        _colorSwapper.DoOnColorChange -= ColorSwapper_DoOnColorChange;
    }

    void ColorSwapper_DoOnColorChange(bool state)
    {
        if (state || !_objectAttached) return;
        if (_colorSwapper.TileColor != _objectAttached.GetComponent<ActivatorCube>().GetColor())
        {
            DetachAll();
        }
    }

    public bool AttachObject(Collider2D objectToAttach)
    {
        if (_objectAttached != null) return false;

        _objectAttached = objectToAttach;
        var _rb = _objectAttached.attachedRigidbody;

        //Resetting the velocity after setting it to Kinematic to prevent it from moving away
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.velocity = Vector2.zero;

        //Setting the new position
        objectToAttach.transform.rotation = Quaternion.identity;
        objectToAttach.transform.position = socket.position;
        objectToAttach.transform.SetParent(transform);

        Array.ForEach(connectorActions, a => a?.Execute(true));

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

        Array.ForEach(connectorActions, a => a?.Execute(false));
    }
}
