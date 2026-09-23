using TMPro;
using UnityEngine;

/// <summary>
/// When this object collides with another (matching an optional tag filter),
/// reads a number from a TMP_Text and adds it to the ScoreManager's score.
///
/// SETUP:
/// 1. Attach this script to one of the two objects that should trigger the score add.
/// 2. Make sure a GameObject with ScoreManager exists in the scene (or persists from a previous one).
/// 3. Assign "scoreValueText" to the TMP_Text (TextMeshPro - Text) component
///    whose displayed number should be added to the score.
/// 4. Set "requiredTag" to the tag of the other object, or leave empty to
///    accept a collision with anything.
/// </summary>
public class ScoreOnCollision : MonoBehaviour
{
    [Tooltip("The TMP_Text whose numeric value gets added to the score on collision.")]
    [SerializeField] private TMP_Text scoreValueText;

    [Tooltip("Tag required on the other object for this to count. Leave empty to accept any collision.")]
    [SerializeField] private string requiredTag = "";

    private void OnCollisionEnter(Collision collision)
    {
        TryAddScore(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryAddScore(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryAddScore(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryAddScore(other.gameObject);
    }

    private void TryAddScore(GameObject other)
    {
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return;

        if (ScoreManager.Instance == null)
        {
            Debug.LogError("[ScoreOnCollision] No ScoreManager found in the scene.");
            return;
        }

        if (scoreValueText == null)
        {
            Debug.LogWarning("[ScoreOnCollision] 'scoreValueText' is not assigned.");
            return;
        }

        if (float.TryParse(scoreValueText.text, out float value))
        {
            ScoreManager.Instance.AddScore(value);
        }
        else
        {
            Debug.LogWarning("[ScoreOnCollision] Could not parse a number from: " + scoreValueText.text);
        }
    }
}