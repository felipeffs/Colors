using UnityEngine;

public class Magnetism : MonoBehaviour
{
    [SerializeField] private Transform attachedTransform;

    public void DoMagneticThing(Transform attachedTransform)
    {
        this.attachedTransform = attachedTransform;
        var c = attachedTransform.GetComponent<Connector>()?.GetPointOfConnection();

        if (c is null)
        {
            c = attachedTransform;
        }

        //Magnetic Behaviour
        transform.rotation = Quaternion.identity;
        transform.position = c.position;
        gameObject.transform.SetParent(attachedTransform);

        // Tem que zerar a velocidade após de por no Kinematic para não ir embora
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void DesdoMagneticThing()
    {
        gameObject.transform.SetParent(null);
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        attachedTransform = null;
    }
}