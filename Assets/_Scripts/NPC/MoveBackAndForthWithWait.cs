using UnityEngine;

/// <summary>
/// Slowly moves this object back and forth between two points (A and B),
/// pausing for a random amount of time at each end before continuing.
///
/// SETUP:
/// 1. Attach this script to the object you want to move.
/// 2. Assign "pointA" and "pointB" (empty GameObjects placed in the scene work well).
/// 3. Adjust "speed", "minWaitTime" and "maxWaitTime" to taste.
/// </summary>
public class MoveBackAndForthWithWait : MonoBehaviour
{
    [Tooltip("Starting point.")]
    [SerializeField] private Transform pointA;

    [Tooltip("Ending point.")]
    [SerializeField] private Transform pointB;

    [Tooltip("Movement speed, in units per second.")]
    [SerializeField] private float speed = 2f;

    [Tooltip("Minimum time, in seconds, to wait at each point before heading back.")]
    [SerializeField] private float minWaitTime = 1f;

    [Tooltip("Maximum time, in seconds, to wait at each point before heading back.")]
    [SerializeField] private float maxWaitTime = 4f;

    private Transform currentTarget;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private float currentWaitDuration;

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("[MoveBackAndForthWithWait] 'pointA' and/or 'pointB' are not assigned.");
            enabled = false;
            return;
        }

        // Start by moving toward point B.
        currentTarget = pointB;
    }

    private void Update()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= currentWaitDuration)
            {
                isWaiting = false;
                waitTimer = 0f;
                // Switch to the other point once the wait is over.
                currentTarget = currentTarget == pointA ? pointB : pointA;
            }

            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Once we've reached the current target, start a new random wait instead of switching immediately.
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.01f)
        {
            isWaiting = true;
            currentWaitDuration = Random.Range(minWaitTime, maxWaitTime);
        }
    }
}