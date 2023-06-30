using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float speed = 3f;
    private int _currentWaypointIndex;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool showTrajectory;
#endif

    private void Start()
    {
        ResetPosition();
        LevelManager.OnRestartLevel += LevelManager_OnRestartLevel;
    }

    private void OnDestroy()
    {
        LevelManager.OnRestartLevel -= LevelManager_OnRestartLevel;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, waypoints[_currentWaypointIndex].position) < 0.1f)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= waypoints.Count)
            {
                _currentWaypointIndex = 0;
            }

#if UNITY_EDITOR
            if (showTrajectory) Debug.DrawLine(transform.position, waypoints[_currentWaypointIndex].position, Color.magenta, Vector2.Distance(transform.position, waypoints[_currentWaypointIndex].position) / speed);
#endif
        }

        //Direction = Destination - Origin
        var direction = (Vector2.MoveTowards(transform.position, waypoints[_currentWaypointIndex].position, 1f * Time.fixedDeltaTime) - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
    }

    private void ResetPosition()
    {
        rb.velocity = rb.velocity = Vector2.zero;
        transform.position = waypoints[0].position;
        _currentWaypointIndex = 1;
    }

    private void LevelManager_OnRestartLevel()
    {
        ResetPosition();
    }
}
