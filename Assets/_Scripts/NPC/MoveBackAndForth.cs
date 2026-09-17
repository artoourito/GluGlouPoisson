using UnityEngine;

/// <summary>
/// Slowly moves this object back and forth between two points (A and B),
/// looping forever. Useful for moving platforms, patrolling enemies, etc.
///
/// SETUP:
/// 1. Attach this script to the object you want to move.
/// 2. Assign "pointA" and "pointB" — these can be empty GameObjects placed
///    in the scene, or left as manual coordinates (see "useLocalTransforms").
/// 3. Adjust "speed" to control how fast it moves.
/// </summary>
public class MoveBackAndForth : MonoBehaviour
{
    [Tooltip("Starting point.")]
    [SerializeField] private Transform pointA;

    [Tooltip("Ending point.")]
    [SerializeField] private Transform pointB;

    [Tooltip("Movement speed, in units per second.")]
    [SerializeField] private float speed = 2f;

    private Transform currentTarget;

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("[MoveBackAndForth] 'pointA' and/or 'pointB' are not assigned.");
            enabled = false;
            return;
        }

        // Start by moving toward point B.
        currentTarget = pointB;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Switch target once we've reached the current one.
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.01f)
        {
            currentTarget = currentTarget == pointA ? pointB : pointA;
        }
    }
}
