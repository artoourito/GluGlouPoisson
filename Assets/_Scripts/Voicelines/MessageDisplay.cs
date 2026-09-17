using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton qui gère l'affichage d'un message texte à l'écran (via un UI Text).
/// Utilisé par OneTimeMessageTrigger et RandomMessageOnTouch.
///
/// MISE EN PLACE :
/// 1. Créez un Canvas avec un composant Text (UI > Text) plein écran ou en bas de l'écran.
/// 2. Créez un GameObject vide "MessageDisplay", attachez-y ce script,
///    et assignez le Text du Canvas au champ "messageText".
/// 3. (Optionnel) Cochez "persistAcrossScenes" si vos messages doivent aussi
///    fonctionner après un changement de scène.
/// </summary>
public class MessageDisplay : MonoBehaviour
{
    public static MessageDisplay Instance { get; private set; }

    [Tooltip("Composant UI Text utilisé pour afficher les messages.")]
    [SerializeField] private Text messageText;

    [Tooltip("Durée d'affichage par défaut, en secondes.")]
    [SerializeField] private float defaultDisplayDuration = 3f;

    [Tooltip("Si coché, cet objet persiste entre les scènes (DontDestroyOnLoad).")]
    [SerializeField] private bool persistAcrossScenes = false;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (messageText != null)
        {
            messageText.text = string.Empty;
        }
        else
        {
            Debug.LogWarning("[MessageDisplay] Aucun 'messageText' assigné.");
        }
    }

    /// <summary>
    /// Affiche un message pendant la durée par défaut.
    /// </summary>
    public void ShowMessage(string message)
    {
        ShowMessage(message, defaultDisplayDuration);
    }

    /// <summary>
    /// Affiche un message pendant une durée donnée. Remplace le message en cours s'il y en a un.
    /// </summary>
    public void ShowMessage(string message, float duration)
    {
        if (messageText == null) return;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(ShowMessageRoutine(message, duration));
    }

    private IEnumerator ShowMessageRoutine(string message, float duration)
    {
        messageText.text = message;
        yield return new WaitForSeconds(duration);
        messageText.text = string.Empty;
        currentRoutine = null;
    }
}
