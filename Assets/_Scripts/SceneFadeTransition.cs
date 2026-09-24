using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Fait fondre l'écran au noir puis charge une nouvelle scène
/// lorsqu'un objet (ex: le joueur) entre en collision avec ce trigger.
///
/// Lorsque le joueur déclenche la transition, tous les NPC cars
/// et car generators de la scène sont supprimés.
/// </summary>
public class SceneFadeTransition : MonoBehaviour
{
    [Header("Réglages du fondu")]
    [Tooltip("Image UI plein écran noire utilisée pour le fondu.")]
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
        // S'assurer que l'image de fondu est invisible au démarrage.
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

        // Vérification du tag du joueur.
        if (!string.IsNullOrEmpty(requiredTag) &&
            !other.CompareTag(requiredTag))
        {
            return;
        }

        isTransitioning = true;

        // Supprimer les voitures et générateurs AVANT le fade.
        DeleteNPCCarsAndGenerators();

        StartCoroutine(FadeAndLoadScene());
    }

    private void DeleteNPCCarsAndGenerators()
    {
        // =========================================================
        // NPC CARS
        // =========================================================

        GameObject[] npcCars =
            GameObject.FindGameObjectsWithTag(npcCarTag);

        int deletedCars = 0;

        foreach (GameObject npcCar in npcCars)
        {
            if (npcCar == null)
                continue;

            // Ne jamais supprimer le GameObject qui contient
            // SceneFadeTransition.
            if (IsPartOfThisObject(npcCar))
                continue;

            Destroy(npcCar);
            deletedCars++;
        }

        Debug.Log(
            "[SceneFadeTransition] " +
            deletedCars +
            " NPC car(s) supprimé(s)."
        );


        // =========================================================
        // CAR GENERATORS
        // =========================================================

        GameObject[] carGenerators =
            GameObject.FindGameObjectsWithTag(carGeneratorTag);

        int deletedGenerators = 0;

        foreach (GameObject generator in carGenerators)
        {
            if (generator == null)
                continue;

            // Très important :
            // si le generator est le parent du GameObject
            // contenant ce script, on ne le détruit pas.
            if (IsPartOfThisObject(generator))
                continue;

            Destroy(generator);
            deletedGenerators++;
        }

        Debug.Log(
            "[SceneFadeTransition] " +
            deletedGenerators +
            " car generator(s) supprimé(s)."
        );
    }

    private bool IsPartOfThisObject(GameObject target)
    {
        // Le GameObject lui-même.
        if (target == gameObject)
            return true;

        // Vérifie si target est un parent de ce script.
        Transform current = transform;

        while (current != null)
        {
            if (current.gameObject == target)
                return true;

            current = current.parent;
        }

        // Vérifie également si target est un enfant
        // du GameObject contenant ce script.
        Transform targetTransform = target.transform;

        if (targetTransform.IsChildOf(transform))
            return true;

        return false;
    }

    private IEnumerator FadeAndLoadScene()
    {
        // =========================================================
        // FONDU VERS LE NOIR
        // =========================================================

        if (fadeImage != null)
        {
            float elapsed = 0f;

            Color c = fadeImage.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;

                c.a =
                    Mathf.Clamp01(
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
            // Même sans image, on garde le timing.
            yield return new WaitForSeconds(
                fadeDuration
            );
        }


        // =========================================================
        // CHARGEMENT DU SCORE AVEC TEMPS RESTANT
        // =========================================================

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
            Mathf.RoundToInt(
                tempsRestant
            );

        int scoreGagnePourCeNiveau =
            scoreBaseNiveau +
            pointsTemps;

        currentTotal +=
            scoreGagnePourCeNiveau;

        PlayerPrefs.SetInt(
            "CurrentGameScore",
            currentTotal
        );

        PlayerPrefs.Save();


        // =========================================================
        // CHARGEMENT DE LA NOUVELLE SCÈNE
        // =========================================================

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(
                sceneToLoad
            );
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

