using UnityEngine;

public class Connector : MonoBehaviour, IAction
{
    [SerializeField] private Transform pointOfConnection;

    public void Active()
    {
        Debug.Log("USEI");
    }

    public void Desactive()
    {
        Debug.Log("Não to usando mais");
    }

    public Transform GetPointOfConnection() => pointOfConnection;
}
