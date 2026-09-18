using UnityEngine;

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

        // --- C'est ici qu'il fallait le mettre pour la 3D ! ---
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.OnBounced();
        }
        // -----------------------------------------------------

        Vector3 incomingDirection = collision.relativeVelocity;
        if (incomingDirection.sqrMagnitude < 0.0001f) return;

        float incomingSpeed = incomingDirection.magnitude;
        float bounceSpeed = Mathf.Max(incomingSpeed * bounceMultiplier, minBounceSpeed);

        Vector3 bounceDirection = -incomingDirection.normalized;
        rb.linearVelocity = bounceDirection * bounceSpeed;
    }

    // ---------- 2D version ----------
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