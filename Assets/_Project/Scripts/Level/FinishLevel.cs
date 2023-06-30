using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    [SerializeField] private SOLevelOrder levelOrder;
    private bool _activated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_activated) return;
        if (other.gameObject.layer == 7)
        {
            _activated = true;
            levelOrder.LoadNextLevel();
        }
    }
}
