using UnityEngine;

public class Connector : MonoBehaviour, IAction
{
    [SerializeField] private Transform pointOfConnection;
    [SerializeField] private Transform plataformWireless;

    public void Active()
    {
        if (plataformWireless)
            plataformWireless.gameObject.SetActive(false);
    }

    public void Desactive()
    {
        if (plataformWireless)
            plataformWireless.gameObject?.SetActive(true);
    }

    public Transform GetPointOfConnection() => pointOfConnection;
}
