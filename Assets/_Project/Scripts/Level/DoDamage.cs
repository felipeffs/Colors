using UnityEngine;

public class DoDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.GetComponentInChildren<IReceiveDamage>()?.TakeDamage(1);
    }
}
