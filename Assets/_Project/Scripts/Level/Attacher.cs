using UnityEngine;

public class Attacher : MonoBehaviour
{
    [SerializeField] private Transform transformAttached;

    public void AttachObject(Transform transformToAttach, Rigidbody2D rb)
    {
        transformAttached = transformToAttach.GetComponent<Connector>()?.GetPointOfConnection() ?? transformToAttach;

        //Setting the new position
        transform.rotation = Quaternion.identity;
        transform.position = transformAttached.position;
        gameObject.transform.SetParent(transformToAttach);

        //Resetting the velocity after setting it to Kinematic to prevent it from moving away
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
    }

    public void DetachAll()
    {
        gameObject.transform.SetParent(null);
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        transformAttached = null;
    }
}