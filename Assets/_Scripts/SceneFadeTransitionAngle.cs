using UnityEngine;

public class SceneFadeTransitionAngle : MonoBehaviour
{
    [Header("Réglages de la scène")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private float transitionFadeDuration = -1f;

    [Header("Filtrage du déclencheur")]
    [SerializeField] private string allowedTag = "Player";

    [Header("Angle requis")]
    [Tooltip("Angle (0-180°) entre transform.forward de cet objet et la direction vers le joueur.")]
    [SerializeField] private float triggerAngle = 0f;
    [Tooltip("Marge de tolérance autour de l'angle requis.")]
    [SerializeField] private float triggerAngleTolerance = 15f;

    private bool transitionStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        TryStartTransition(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryStartTransition(other.gameObject);
    }

    private void TryStartTransition(GameObject other)
    {
        if (transitionStarted) return;

        if (!string.IsNullOrEmpty(allowedTag) && !other.CompareTag(allowedTag))
            return;

        Vector3 directionToOther = (other.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToOther);

        if (Mathf.Abs(angle - triggerAngle) > triggerAngleTolerance)
            return;

        if (ScreenFader.Instance == null)
        {
            Debug.LogError("[SceneFadeTransitionAngle] Aucun ScreenFader trouvé dans la scène.");
            return;
        }

        transitionStarted = true;
        ScreenFader.Instance.FadeOutThenLoadScene(targetSceneName, transitionFadeDuration);
    }
}
