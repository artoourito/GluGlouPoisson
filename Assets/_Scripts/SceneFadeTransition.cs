using System.Collections;
using System.Reflection;
using Unity.Media.Osc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [Tooltip("Tag requis sur l'objet qui déclenche le trigger.")]
    [SerializeField] private string requiredTag = "Player";

    [Header("Destruction des NPC cars")]
    [Tooltip("Tag des objets à détruire au contact du joueur.")]
    [SerializeField] private string npcCarTag = "npcar";

    [Header("OSC Receiver")]
    [Tooltip("Référence au composant OSC Receiver dont il faut changer le port. Peut être sur un autre GameObject.")]
    [SerializeField] private OscReceiver oscReceiver;

    [Tooltip("Nouveau port à assigner au OSC Receiver.")]
    [SerializeField] private int newOscPort = 8001;

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
            Debug.LogWarning("[SceneFadeTransition] Aucune 'fadeImage' assignée.");
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
            yield return new WaitForSeconds(fadeDuration);
        }

        // --- SCORE : géré par le GameManager ---
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddLevelScore();
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