using UnityEngine;

/// <summary>
/// When this object collides with another (matching an optional tag filter),
/// shows the name-input UI panel so the player can enter their name.
///
/// SETUP:
/// 1. Attach this script to one of the two objects that should trigger the name prompt.
/// 2. Assign "namePromptPanel" to the UI panel GameObject containing the
///    TMP_InputField and Submit button (see NameInputPanel.cs). It should
///    start disabled in the scene.
/// 3. Set "requiredTag" to the tag of the other object, or leave empty to
///    accept a collision with anything.
/// </summary>
public class NamePromptTrigger : MonoBehaviour
{
    [Tooltip("The UI panel (with a TMP_InputField and Submit button) to show when triggered.")]
    [SerializeField] private GameObject namePromptPanel;

    [Tooltip("Tag required on the other object for this to count. Leave empty to accept any collision.")]
    [SerializeField] private string requiredTag = "";

    private bool hasTriggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        TryShowPrompt(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryShowPrompt(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryShowPrompt(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryShowPrompt(other.gameObject);
    }

    private void TryShowPrompt(GameObject other)
    {
        if (hasTriggered) return;

        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return;

        if (namePromptPanel == null)
        {
            Debug.LogError("[NamePromptTrigger] 'namePromptPanel' is not assigned.");
            return;
        }

        hasTriggered = true;
        namePromptPanel.SetActive(true);
    }
}