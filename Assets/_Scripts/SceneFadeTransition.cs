using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Fait fondre l'écran au noir puis charge une nouvelle scène
/// lorsqu'un objet (ex: le joueur) entre en collision avec ce trigger.
///
/// Détruit également tous les NPC cars et car generators présents
/// dans la scène lorsque le joueur déclenche la transition.
/// </summary>
public class SceneFadeTransition : MonoBehaviour
{
    [Header("Réglages du fondu")]
    [Tooltip("Image UI plein écran (noire, alpha = 0 au départ) utilisée pour le fondu.")]
    [SerializeField] private Image fadeImage;

    [Tooltip("Durée du fondu au noir, en secondes.")]
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Réglages de la scène")]
    [Tooltip("Nom exact de la scène à charger.")]
    [SerializeField] private string sceneToLoad;

    [Header("Filtrage du déclencheur")]
    [Tooltip("Tag requis sur l'objet qui déclenche le trigger.")]
    [SerializeField] private string requiredTag = "Player";

    [Header("Objets à supprimer")]
    [Tooltip("Tag utilisé par les voitures NPC.")]
    [SerializeField] private string npcCarTag = "NPCCar";

    [Tooltip("Tag utilisé par les générateurs de voitures.")]
    [SerializeField] private string carGeneratorTag = "CarGenerator";

    private bool isTransitioning = false;

    private void Awake()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "[SceneFadeTransition] Aucune 'fadeImage' assignée. " +
                "Le fondu ne sera pas visible."
            );
        }
    }

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
        if (isTransitioning)
            return;

        if (!string.IsNullOrEmpty(requiredTag) &&
            !other.CompareTag(requiredTag))
        {
            return;
        }

        isTransitioning = true;

        // Supprime les NPC cars et les générateurs
        DeleteNPCCarsAndGenerators();

        StartCoroutine(FadeAndLoadScene());
    }

    private void DeleteNPCCarsAndGenerators()
    {
        // --- NPC CARS ---
        GameObject[] npcCars = GameObject.FindGameObjectsWithTag(npcCarTag);

        foreach (GameObject npcCar in npcCars)
        {
            if (npcCar != null)
            {
                Destroy(npcCar);
            }
        }

        Debug.Log(
            "[SceneFadeTransition] " +
            npcCars.Length +
            " NPC car(s) supprimé(s)."
        );

        // --- CAR GENERATORS ---
        GameObject[] carGenerators =
            GameObject.FindGameObjectsWithTag(carGeneratorTag);

        foreach (GameObject generator in carGenerators)
        {
            if (generator != null)
            {
                Destroy(generator);
            }
        }

        Debug.Log(
            "[SceneFadeTransition] " +
            carGenerators.Length +
            " car generator(s) supprimé(s)."
        );
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

                c.a = Mathf.Clamp01(
                    elapsed / fadeDuration
                );

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

        // --- CHARGEMENT DU SCORE AVEC TEMPS RESTANT ---

        int currentTotal =
            PlayerPrefs.GetInt(
                "CurrentGameScore",
                0
            );

        int scoreBaseNiveau = 10;

        float tempsRestant = 0f;

        if (GameManager.Instance != null)
        {
            tempsRestant =
                GameManager.Instance.Timer;
        }

        int pointsTemps =
            Mathf.RoundToInt(tempsRestant);

        int scoreGagnePourCeNiveau =
            scoreBaseNiveau + pointsTemps;

        currentTotal +=
            scoreGagnePourCeNiveau;

        PlayerPrefs.SetInt(
            "CurrentGameScore",
            currentTotal
        );

        PlayerPrefs.Save();

        // --- CHARGEMENT DE LA NOUVELLE SCÈNE ---

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError(
                "[SceneFadeTransition] " +
                "'sceneToLoad' n'est pas renseigné."
            );
        }
    }
}
