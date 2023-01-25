using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlataform : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float velocity;
    private int _currentWaypointIndex = 0;

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Vector2.Distance(transform.position, waypoints[_currentWaypointIndex].position) < 0.1f)
            {
                _currentWaypointIndex++;
                if (_currentWaypointIndex >= waypoints.Count)
                {
                    _currentWaypointIndex = 0;
                }

            }
            transform.position = Vector2.MoveTowards(transform.position, waypoints[_currentWaypointIndex].position, velocity * Time.deltaTime);
        }
    }
}
