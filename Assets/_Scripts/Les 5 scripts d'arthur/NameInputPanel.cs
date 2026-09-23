using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the name-input UI panel: reads the name typed by the player,
/// stores it in ScoreManager, and loads the results scene.
///
/// SETUP:
/// 1. Create a UI Panel with a TMP_InputField (for the name) and a Button ("Submit").
/// 2. Attach this script to the Panel (or any GameObject on it).
/// 3. Assign "nameInputField" to the TMP_InputField.
/// 4. Set "resultsSceneName" to the name of the scene that displays the results
///    (must be added to Build Settings).
/// 5. On the Submit Button's OnClick() list, drag this GameObject in and
///    choose NameInputPanel > OnSubmitPressed().
/// </summary>
public class NameInputPanel : MonoBehaviour
{
    [Tooltip("The input field where the player types their name.")]
    [SerializeField] private TMP_InputField nameInputField;

    [Tooltip("Name of the scene to load after the name is submitted.")]
    [SerializeField] private string resultsSceneName;

    [Tooltip("Name used if the player submits without typing anything.")]
    [SerializeField] private string defaultNameIfEmpty = "Player";

    /// <summary>
    /// Call this from the Submit button's OnClick() event.
    /// </summary>
    public void OnSubmitPressed()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogError("[NameInputPanel] No ScoreManager found in the scene.");
            return;
        }

        string enteredName = nameInputField != null ? nameInputField.text.Trim() : "";

        if (string.IsNullOrEmpty(enteredName))
        {
            enteredName = defaultNameIfEmpty;
        }

        ScoreManager.Instance.SetPlayerName(enteredName);

        if (!string.IsNullOrEmpty(resultsSceneName))
        {
            SceneManager.LoadScene(resultsSceneName);
        }
        else
        {
            Debug.LogError("[NameInputPanel] 'resultsSceneName' is not set.");
        }
    }
}