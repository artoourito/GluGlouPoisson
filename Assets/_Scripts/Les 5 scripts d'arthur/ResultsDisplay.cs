using TMPro;
using UnityEngine;

/// <summary>
/// Displays the player's name and final score. Attach this in the results
/// scene, which should load AFTER the ScoreManager has been set up (since
/// ScoreManager persists across scenes via DontDestroyOnLoad).
///
/// SETUP:
/// 1. In your results scene, create two TMP_Text elements: one for the name,
///    one for the score.
/// 2. Attach this script to any GameObject in that scene.
/// 3. Assign "nameDisplayText" and "scoreDisplayText" to those TMP_Text elements.
/// </summary>
public class ResultsDisplay : MonoBehaviour
{
    [Tooltip("TMP_Text used to display the player's entered name.")]
    [SerializeField] private TMP_Text nameDisplayText;

    [Tooltip("TMP_Text used to display the final score.")]
    [SerializeField] private TMP_Text scoreDisplayText;

    [Tooltip("Optional format string for the score, e.g. \"Score: {0}\".")]
    [SerializeField] private string scoreFormat = "Score: {0}";

    [Tooltip("Optional format string for the name, e.g. \"Player: {0}\".")]
    [SerializeField] private string nameFormat = "{0}";

    private void Start()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogError("[ResultsDisplay] No ScoreManager found. Make sure it persisted from the previous scene.");
            return;
        }

        if (nameDisplayText != null)
        {
            nameDisplayText.text = string.Format(nameFormat, ScoreManager.Instance.PlayerName);
        }

        if (scoreDisplayText != null)
        {
            scoreDisplayText.text = string.Format(scoreFormat, ScoreManager.Instance.CurrentScore);
        }
    }
}