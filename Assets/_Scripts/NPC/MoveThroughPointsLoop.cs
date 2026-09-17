using UnityEngine;

/// <summary>
/// Moves this object through a list of points in order, looping back to the
/// first point once it reaches the last one (instead of going back and forth).
/// Optionally waits a few seconds at each point before continuing.
///
/// SETUP:
/// 1. Attach this script to the object you want to move.
/// 2. Set the size of "points" to however many stops you want, and assign
///    a Transform (e.g. an empty GameObject placed in the scene) to each slot.
/// 3. Adjust "speed" and "waitTime" to taste.
/// </summary>
public class MoveThroughPointsLoop : MonoBehaviour
{
    [Tooltip("The points to move through, in order. After the last one, it loops back to the first.")]
    [SerializeField] private Transform[] points;

    [Tooltip("Movement speed, in units per second.")]
    [SerializeField] private float speed = 2f;

    [Tooltip("How long to wait, in seconds, at each point before moving to the next.")]
    [SerializeField] private float waitTime = 2f;

    private int currentIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    private void Start()
    {
        if (points == null || points.Length < 2)
        {
            Debug.LogWarning("[MoveThroughPointsLoop] Assign at least 2 points in the 'points' list.");
            enabled = false;
        }
    }

    private void Update()
    {
        Transform target = points[currentIndex];

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                // Move to the next point, looping back to 0 after the last one.
                currentIndex = (currentIndex + 1) % points.Length;
            }

            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            isWaiting = true;
        }
    }
}
