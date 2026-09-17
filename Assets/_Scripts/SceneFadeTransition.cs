using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Fait fondre l'écran au noir puis charge une nouvelle scène
/// lorsqu'un objet (ex: le joueur) entre en collision avec ce trigger.
///
/// MISE EN PLACE :
/// 1. Attachez ce script à un GameObject possédant un Collider (2D ou 3D)
///    avec "Is Trigger" coché.
/// 2. Créez un Canvas avec une Image plein écran noire, opacité initiale à 0,
///    et assignez-la au champ "fadeImage" ci-dessous.
/// 3. Renseignez le nom de la scène cible dans "sceneToLoad".
/// 4. Assurez-vous que l'objet qui déclenche le trigger a le bon Tag
///    (par défaut "Player") ou ajustez la vérification dans le code.
/// </summary>
public class SceneFadeTransition : MonoBehaviour
{
    [Header("Réglages du fondu")]
    [Tooltip("Image UI plein écran (noire, alpha = 0 au départ) utilisée pour le fondu.")]
    [SerializeField] private Image fadeImage;

    [Tooltip("Durée du fondu au noir, en secondes.")]
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Réglages de la scène")]
    [Tooltip("Nom exact de la scène à charger (doit être ajoutée aux Build Settings).")]
    [SerializeField] private string sceneToLoad;

    [Header("Filtrage du déclencheur")]
    [Tooltip("Tag requis sur l'objet qui déclenche le trigger. Laisser vide pour accepter n'importe quel objet.")]
    [SerializeField] private string requiredTag = "Player";

    private bool isTransitioning = false;

    private void Awake()
    {
        // Sécurité : s'assurer que l'image de fondu est bien invisible au démarrage
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[SceneFadeTransition] Aucune 'fadeImage' assignée. Le fondu ne sera pas visible.");
        }
    }

    // Utilisez celle qui correspond à votre projet (2D ou 3D) et supprimez l'autre.

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
        if (isTransitioning) return;

        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return;

        isTransitioning = true;
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        // --- Fondu vers le noir ---
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
            // Pas d'image de fondu : on attend quand même la durée pour garder le timing
            yield return new WaitForSeconds(fadeDuration);
        }

        // --- Chargement de la nouvelle scène ---
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("[SceneFadeTransition] 'sceneToLoad' n'est pas renseigné.");
        }
    }
}
