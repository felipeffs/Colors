using UnityEngine;
using TMPro;

public class VersionDisplayUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionDisplay;

    private void Start()
    {
        versionDisplay.text = Application.version;
    }
}
