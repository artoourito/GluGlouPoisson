using UnityEngine;

/// <summary>
/// Rotates this object to always face the direction it's currently moving in.
/// The behavior can be toggled on/off either in the Inspector or at runtime
/// (e.g. from a settings menu, via SetFacingEnabled).
/// </summary>
public class FaceMovementDirection : MonoBehaviour
{
    [Tooltip("Whether the object should rotate to face its movement direction.")]
    [SerializeField] private bool enableFacing = true;

    [Tooltip("How fast the object turns to face its new direction.")]
    [SerializeField] private float rotationSpeed = 10f;

    [Tooltip("Check this if your object only rotates on the Z axis (typical for 2D games).")]
    [SerializeField] private bool use2DRotation = false;

    [Tooltip("Minimum movement speed required before the object rotates.")]
    [SerializeField] private float minSpeedThreshold = 0.05f;

    [Header("Correction d'orientation")]
    [Tooltip("Décalage en degrés à ajouter si le modèle de la voiture pointe de base vers le haut, l'arrière, etc.")]
    float rotationOffset = 180f;

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
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg + rotationOffset;
            Quaternion targetRotation2D = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2D, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement.normalized);
            // Applique un décalage en rotation 3D si nécessaire
            targetRotation *= Quaternion.Euler(0f, rotationOffset, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void SetFacingEnabled(bool value)
    {
        enableFacing = value;
    }
}