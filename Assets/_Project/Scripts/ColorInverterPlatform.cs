using UnityEngine;

public class ColorInverterPlatform : MonoBehaviour
{
    [SerializeField] private ColorInverter.ColorIDInverter colorID;

    public void InverterColor(ColorInverter.ColorIDInverter colorID)
    {
        gameObject.SetActive(colorID == this.colorID);
    }
}
