using UnityEngine;
using UnityEngine.EventSystems;

public class ResetSelectButton : MonoBehaviour
{
    [ReadOnly][SerializeField] private EventSystem _currentEventSystem;

    private void OnEnable()
    {
        if (_currentEventSystem == null)
            _currentEventSystem = EventSystem.current;
    }

    private void OnDisable()
    {
        _currentEventSystem.SetSelectedGameObject(_currentEventSystem.firstSelectedGameObject);
    }
}
