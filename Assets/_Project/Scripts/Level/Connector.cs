using UnityEngine;

public class Connector : MonoBehaviour, IAction
{
    [SerializeField] private Transform pointOfConnection;
    [SerializeField] private Transform platformToDisableTransform;
    //deactivatingPlatformTransform

    public void Active()
    {
        if (platformToDisableTransform)
            platformToDisableTransform.gameObject.SetActive(false);
    }

    public void Desactive()
    {
        if (platformToDisableTransform)
            platformToDisableTransform.gameObject.SetActive(true);
    }

    public Transform GetPointOfConnection() => pointOfConnection;
}
