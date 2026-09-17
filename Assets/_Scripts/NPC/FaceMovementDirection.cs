using UnityEngine;

/// <summary>
/// Rotates this object to always face the direction it's currently moving in.
/// The behavior can be toggled on/off either in the Inspector or at runtime
/// (e.g. from a settings menu, via SetFacingEnabled).
///
/// SETUP:
/// 1. Attach this script to the object that should turn to face its movement.
/// 2. Toggle "enableFacing" in the Inspector, or call SetFacingEnabled(bool)
///    from a UI Toggle/Button in your menu to switch it on/off at runtime.
/// 3. If this is a 2D object (rotating on the Z axis only), check "use2DRotation".
/// </summary>
public class FaceMovementDirection : MonoBehaviour
{
    [Tooltip("Whether the object should rotate to face its movement direction. Can also be changed at runtime via SetFacingEnabled().")]
    [SerializeField] private bool enableFacing = true;

    [Tooltip("How fast the object turns to face its new direction. Higher = snappier, near-instant turning.")]
    [SerializeField] private float rotationSpeed = 10f;

    [Tooltip("Check this if your object only rotates on the Z axis (typical for 2D games).")]
    [SerializeField] private bool use2DRotation = false;

    [Tooltip("Minimum movement speed (units/sec) required before the object bothers rotating. Prevents jittering when nearly stopped.")]
    [SerializeField] private float minSpeedThreshold = 0.05f;

    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 movement = transform.position - lastPosition;
        lastPosition = transform.position;

        if (!enableFacing) return;

        float speed = movement.magnitude / Time.deltaTime;
        if (speed < minSpeedThreshold) return;

        if (use2DRotation)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            Quaternion targetRotation2D = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2D, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Call this from a UI Toggle, Button, or any menu script to turn the
    /// facing behavior on or off at runtime.
    /// </summary>
    public void SetFacingEnabled(bool value)
    {
        enableFacing = value;
    }
}
