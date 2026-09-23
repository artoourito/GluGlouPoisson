using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Switches to another scene when this object is clicked with the mouse.
/// Works for both world objects (via a Collider) and UI Buttons.
///
/// SETUP FOR A WORLD OBJECT (3D/2D model, sprite, etc.):
/// 1. Attach this script directly to the object.
/// 2. Make sure it has a Collider (2D or 3D) — OnMouseDown requires one.
/// 3. Make sure a Camera in the scene has "Physics Raycaster" (3D) or
///    "Physics 2D Raycaster" (2D) if you're also using UI elements, though
///    OnMouseDown works out of the box for plain world objects without one.
/// 4. Set "sceneToLoad" to the name of the scene to switch to.
///
/// SETUP FOR A UI BUTTON:
/// 1. Attach this script to any GameObject in the scene (e.g. an empty "SceneSwitcher").
/// 2. On your Button component, add an OnClick() entry and drag this
///    GameObject in, then choose SceneSwitchOnClick > LoadScene().
///    (You can skip the "sceneToLoad" field for this method and instead
///    call LoadSceneByName with a parameter directly from the Button's OnClick.)
/// </summary>
public class SceneSwitchOnClick : MonoBehaviour
{
    [Tooltip("Name of the scene to load (must be added to Build Settings).")]
    [SerializeField] private string sceneToLoad;

    // Used for world objects with a Collider.
    private void OnMouseDown()
    {
        LoadScene();
    }

    /// <summary>
    /// Loads the scene set in "sceneToLoad". Hook this up to a UI Button's
    /// OnClick() event, or call it from other scripts.
    /// </summary>
    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("[SceneSwitchOnClick] 'sceneToLoad' is not set.");
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    /// <summary>
    /// Loads a scene by name passed directly as a parameter — useful if you
    /// want a single script to serve several buttons that each load a
    /// different scene (drag this method into a Button's OnClick and type
    /// the scene name in the field that appears).
    /// </summary>
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneSwitchOnClick] No scene name provided.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
