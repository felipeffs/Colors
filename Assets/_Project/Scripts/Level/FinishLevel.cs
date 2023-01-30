using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    [SerializeField] private SOLevelOrder levelOrder;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 7)
        {
            levelOrder.LoadNextLevel();
        }
    }
}
