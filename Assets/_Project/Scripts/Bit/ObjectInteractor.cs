using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// here it will check if an object is near it and perform if it is
public class ObjectInteractor : MonoBehaviour
{
    private Dictionary<Transform, IInteractable> _interactableObjects = new Dictionary<Transform, IInteractable>();
    [SerializeField] private Transform bitTransform;
    [SerializeField] private ObjectGrabber objectGrabber;

    public void Update()
    {
        if (InputManager.Instance.InteractWasPressed())
        {
            TryInteractWithNearbyObject();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        StoreObjectRef(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        RemoveObjectRef(other);
    }

    private void StoreObjectRef(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<IInteractable>(out IInteractable interactableObject)) return;

        if (!_interactableObjects.ContainsValue(interactableObject))
            _interactableObjects.Add(other.transform, interactableObject);
    }

    private void RemoveObjectRef(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<IInteractable>(out IInteractable interactableObject)) return;

        _interactableObjects.Remove(other.transform);
    }

    private void TryInteractWithNearbyObject()
    {
        if (objectGrabber.TryGetGrabbedTransform(out Transform grabbedTransform))
        {
            grabbedTransform.gameObject.GetComponent<IInteractable>()?.Interact();
            return;
        }

        if (_interactableObjects.Count == 0) return;

        var shorterDistance = Mathf.Infinity;
        Transform shorterTransform = null;
        foreach (var (transform, iObject) in _interactableObjects)
        {
            var distance = Vector3.Distance(this.transform.position, transform.position);

            if (distance < shorterDistance)
            {
                shorterDistance = distance;
                shorterTransform = transform;
            }
        }
        _interactableObjects[shorterTransform].Interact();
    }
}

