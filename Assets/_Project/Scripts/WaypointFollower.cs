using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float speed = 3f;
    private int _currentWaypointIndex = 0;

    [Header("Debug")]
    [SerializeField] private bool showTrajectory;

    private void Update()
    {
        if (Vector2.Distance(transform.position, waypoints[_currentWaypointIndex].position) < 0.1f)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= waypoints.Count)
            {
                _currentWaypointIndex = 0;
            }

            if (showTrajectory) Debug.DrawLine(transform.position, waypoints[_currentWaypointIndex].position, Color.magenta, Vector2.Distance(transform.position, waypoints[_currentWaypointIndex].position) / speed);
        }

        //Direction = Destination - Origin
        var direction = (Vector2.MoveTowards(transform.position, waypoints[_currentWaypointIndex].position, 1f * Time.fixedDeltaTime) - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
    }
}
