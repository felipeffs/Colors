using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance { get; private set; }
    [SerializeField] private GameObject bitPrefab;
    private GameObject _bit;
    private BitController _bitController;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log($"There can be only one instance of {name}");
            Destroy(this);
            return;
        }

        Instance = this;

        //Temp
        if (_bit == null)
        {
            _bit = Instantiate(bitPrefab, Vector3.zero, bitPrefab.transform.rotation);
            _bitController = _bit.GetComponentInChildren<BitController>();
            _bit.SetActive(false);
        }
    }

    public void ResetBit(Vector3 newPosition)
    {
        if (_bit == null)
        {
            _bit = Instantiate(bitPrefab, Vector3.zero, bitPrefab.transform.rotation);
            _bitController = _bit.GetComponentInChildren<BitController>();
            _bit.SetActive(false);
        }

        _bitController.Reset(newPosition);
        _bit.SetActive(true);
    }
}
