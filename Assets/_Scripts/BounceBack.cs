using UnityEngine;

/// <summary>
/// Attach this to an object (e.g. a bounce pad, spring, or wall) to send the
/// player bouncing back in the exact direction they came from, rather than
/// reflecting off the surface normal or launching in a fixed direction.
///
/// SETUP:
/// 1. Attach this script to the object you want the player to bounce off of.
///    Its Collider can be solid (not a trigger) for a normal physical bounce,
///    or "Is Trigger" if you want the bounce without a hard physical stop.
/// 2. The player needs a Rigidbody (3D) or Rigidbody2D (2D).
/// 3. Adjust "bounceMultiplier" to control how strong the bounce-back is
///    relative to the player's incoming speed.
/// </summary>
public class BounceBack : MonoBehaviour
{
    [Tooltip("How strongly to bounce the player back, relative to their incoming speed. " +
             "1 = same speed reversed, 1.5 = 50% stronger, etc.")]
    [SerializeField] private float bounceMultiplier = 1f;

    [Tooltip("Minimum bounce speed applied, in case the player barely touches the object.")]
    [SerializeField] private float minBounceSpeed = 2f;

    [Tooltip("Tag required on the object that triggers the bounce.")]
    [SerializeField] private string requiredTag = "Player";

    // ---------- 3D version ----------

    private void OnCollisionEnter(Collision collision)
    {
        if (!string.IsNullOrEmpty(requiredTag) && !collision.gameObject.CompareTag(requiredTag))
            return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        // relativeVelocity points in the direction the player was travelling
        // relative to this object at the moment of impact.
        Vector3 incomingDirection = collision.relativeVelocity;
        if (incomingDirection.sqrMagnitude < 0.0001f) return;

        float incomingSpeed = incomingDirection.magnitude;
        float bounceSpeed = Mathf.Max(incomingSpeed * bounceMultiplier, minBounceSpeed);

        // Send the player back the way they came from.
        Vector3 bounceDirection = -incomingDirection.normalized;
        rb.linearVelocity = bounceDirection * bounceSpeed;
    }

    // ---------- 2D version ----------
    // Delete this section if your project is 3D.

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!string.IsNullOrEmpty(requiredTag) && !collision.gameObject.CompareTag(requiredTag))
            return;

        Rigidbody2D rb = collision.rigidbody;
        if (rb == null) return;

        Vector2 incomingDirection = collision.relativeVelocity;
        if (incomingDirection.sqrMagnitude < 0.0001f) return;

        float incomingSpeed = incomingDirection.magnitude;
        float bounceSpeed = Mathf.Max(incomingSpeed * bounceMultiplier, minBounceSpeed);

        Vector2 bounceDirection = -incomingDirection.normalized;
        rb.linearVelocity = bounceDirection * bounceSpeed;
    }
}
