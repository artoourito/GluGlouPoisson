using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFadeTransitionAngle : MonoBehaviour
{
    [Header("Fondu")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Scène")]
    [SerializeField] private string targetSceneName;

    [Header("Filtrage du déclencheur")]
    [SerializeField] private string allowedTag = "Player";

    [Header("Angle requis")]
    [Tooltip("Angle (0-180°) entre transform.forward de cet objet et la direction vers le joueur.")]
    [SerializeField] private float triggerAngle = 0f;
    [Tooltip("Marge de tolérance autour de l'angle requis.")]
    [SerializeField] private float triggerAngleTolerance = 15f;

    private bool transitionStarted = false;

    private void Awake()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
        else
        {
            Debug.LogWarning("[SceneFadeTransitionAngle] 'fadeImage' non assignée.");
        }
    }

    // OnTriggerStay est appelé à CHAQUE frame tant que l'objet reste dans le trigger,
    // contrairement à OnTriggerEnter qui n'est appelé qu'une seule fois à l'entrée.
    private void OnTriggerStay(Collider other)
    {
        CheckAngleAndTransition(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckAngleAndTransition(other.gameObject);
    }

    private void CheckAngleAndTransition(GameObject other)
    {
        if (transitionStarted) return;

        if (!string.IsNullOrEmpty(allowedTag) && !other.CompareTag(allowedTag))
            return;

        Vector3 directionToOther = (other.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToOther);

        Debug.Log("[SceneFadeTransitionAngle] Angle mesuré: " + angle + " (requis: " + triggerAngle + " ± " + triggerAngleTolerance + ")");

        if (Mathf.Abs(angle - triggerAngle) > triggerAngleTolerance)
            return;

        Debug.Log("[SceneFadeTransitionAngle] Angle correct, lancement de la transition.");
        transitionStarted = true;
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        Debug.Log("[SceneFadeTransitionAngle] Début du fondu.");

        if (fadeImage != null)
        {
            float elapsed = 0f;
            Color c = fadeImage.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Clamp01(elapsed / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }

            c.a = 1f;
            fadeImage.color = c;
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        Debug.Log("[SceneFadeTransitionAngle] Fondu terminé, chargement de la scène: " + targetSceneName);

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Destroy(fadeImage.gameObject);
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("[SceneFadeTransitionAngle] 'targetSceneName' n'est pas renseigné.");
        }
    }
}