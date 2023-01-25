using System.Collections.Generic;
using UnityEngine;

public class StickyPlataform : MonoBehaviour
{
    [ReadOnly][SerializeField] private List<GameObject> _stickedObjects;
    [SerializeField] private ColorSwapper colorSwapper;

    private void Start()
    {
        colorSwapper = GetComponent<ColorSwapper>();
        colorSwapper.BeforeColorSwap += ColorSwapper_BeforeColorSwap;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != 6)
        {
            other.gameObject.transform.SetParent(transform);
            _stickedObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != 6)
        {
            other.gameObject.transform.SetParent(null);
            _stickedObjects.Remove(other.gameObject);
        }
    }

    private void ColorSwapper_BeforeColorSwap(bool isActive)
    {
        if (isActive) return;

        foreach (var obj in _stickedObjects)
        {
            obj.transform.SetParent(null);
        }
        _stickedObjects.Clear();
    }

}
