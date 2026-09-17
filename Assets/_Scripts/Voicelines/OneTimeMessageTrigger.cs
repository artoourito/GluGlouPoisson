using UnityEngine;

/// <summary>
/// Affiche un message texte une seule fois lorsque le joueur touche cet objet
/// (ex: une boîte spécifique). Ne se redéclenche plus ensuite, même si le
/// joueur touche à nouveau la boîte.
///
/// MISE EN PLACE :
/// 1. Attachez ce script à votre boîte, qui doit avoir un Collider (2D ou 3D)
///    avec "Is Trigger" coché.
/// 2. Assurez-vous qu'un GameObject avec le script MessageDisplay existe dans la scène.
/// 3. Renseignez le texte à afficher dans "message".
/// 4. Le joueur doit avoir le Tag renseigné dans "requiredTag" (par défaut "Player").
/// </summary>
public class OneTimeMessageTrigger : MonoBehaviour
{
    [Tooltip("Message affiché lors du premier contact avec le joueur.")]
    [TextArea]
    [SerializeField] private string message = "Vous avez trouvé la boîte !";

    [Tooltip("Durée d'affichage, en secondes. Laisser à -1 pour utiliser la valeur par défaut de MessageDisplay.")]
    [SerializeField] private float displayDuration = -1f;

    [Tooltip("Tag requis sur l'objet qui déclenche le trigger.")]
    [SerializeField] private string requiredTag = "Player";

    private bool hasTriggered = false;

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
        if (hasTriggered) return;

        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return;

        if (MessageDisplay.Instance == null)
        {
            Debug.LogError("[OneTimeMessageTrigger] Aucun MessageDisplay trouvé dans la scène.");
            return;
        }

        hasTriggered = true;

        if (displayDuration > 0f)
        {
            MessageDisplay.Instance.ShowMessage(message, displayDuration);
        }
        else
        {
            MessageDisplay.Instance.ShowMessage(message);
        }
    }
}
