using UnityEngine;

/// <summary>
/// Affiche un texte choisi au hasard dans une liste chaque fois que le joueur
/// touche cet objet. Contrairement à OneTimeMessageTrigger, celui-ci se
/// redéclenche à chaque nouveau contact (avec un court délai anti-spam).
///
/// MISE EN PLACE :
/// 1. Attachez ce script à N'IMPORTE QUEL objet ayant un Collider (2D ou 3D)
///    avec "Is Trigger" coché.
/// 2. Assurez-vous qu'un GameObject avec le script MessageDisplay existe dans la scène.
/// 3. Remplissez la liste "messages" avec les textes possibles.
/// </summary>
public class RandomMessageOnTouch : MonoBehaviour
{
    [Tooltip("Liste des messages possibles. Un est choisi au hasard à chaque contact.")]
    [TextArea]
    [SerializeField] private string[] messages =
    {
        "Aïe !",
        "Qu'est-ce que c'est que ça ?",
        "Intéressant...",
        "Continue d'explorer !"
    };

    [Tooltip("Durée d'affichage, en secondes. Laisser à -1 pour utiliser la valeur par défaut de MessageDisplay.")]
    [SerializeField] private float displayDuration = -1f;

    [Tooltip("Tag requis sur l'objet qui déclenche le trigger.")]
    [SerializeField] private string requiredTag = "Player";

    [Tooltip("Délai minimum entre deux déclenchements, pour éviter le spam si le joueur reste dans le collider.")]
    [SerializeField] private float cooldown = 1f;

    private float lastTriggerTime = -Mathf.Infinity;

    private void OnTriggerEnter(Collider other)
    {
        TryTrigger(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryTrigger(other.gameObject);
    }

    private void TryTrigger(GameObject other)
    {
        if (Time.time - lastTriggerTime < cooldown) return;

        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return;

        if (messages == null || messages.Length == 0)
        {
            Debug.LogWarning("[RandomMessageOnTouch] La liste 'messages' est vide.");
            return;
        }

        if (MessageDisplay.Instance == null)
        {
            Debug.LogError("[RandomMessageOnTouch] Aucun MessageDisplay trouvé dans la scène.");
            return;
        }

        lastTriggerTime = Time.time;

        string chosenMessage = messages[Random.Range(0, messages.Length)];

        if (displayDuration > 0f)
        {
            MessageDisplay.Instance.ShowMessage(chosenMessage, displayDuration);
        }
        else
        {
            MessageDisplay.Instance.ShowMessage(chosenMessage);
        }
    }
}
