using UnityEngine;

/// <summary>
/// Spawns a random 3D model (prefab) from a list, at this object's position
/// and rotation. Useful for adding visual variety to identical spawn points
/// (rocks, trees, props, enemy types, etc.).
///
/// SETUP:
/// 1. Attach this script to an empty GameObject placed where you want the model to appear.
/// 2. Fill the "possibleModels" list with the prefabs to choose from.
/// 3. By default, a model is spawned once on Awake. Enable "randomizeEveryTimeEnabled"
///    if this object gets reused (e.g. pooled) and should pick a new model each time.
/// </summary>
public class RandomModel : MonoBehaviour
{
    [Tooltip("The prefabs to choose from at random.")]
    [SerializeField] private GameObject[] possibleModels;

    [Tooltip("If checked, the spawned model becomes a child of this object. " +
             "Recommended so it moves/rotates together with this GameObject.")]
    [SerializeField] private bool parentToThisObject = true;

    [Tooltip("If checked, a new random model is spawned every time this object is enabled, " +
             "replacing the previous one, instead of just once on creation.")]
    [SerializeField] private bool randomizeEveryTimeEnabled = false;

    private GameObject currentModelInstance;

    private void Awake()
    {
        SpawnRandomModel();
    }

    private void OnEnable()
    {
        // Skip the very first enable right after Awake to avoid spawning twice.
        if (randomizeEveryTimeEnabled && currentModelInstance != null)
        {
            SpawnRandomModel();
        }
    }

    private void SpawnRandomModel()
    {
        if (possibleModels == null || possibleModels.Length == 0)
        {
            Debug.LogWarning("[RandomModel] 'possibleModels' is empty on " + gameObject.name);
            return;
        }

        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
        }

        GameObject chosenPrefab = possibleModels[Random.Range(0, possibleModels.Length)];
        Transform parent = parentToThisObject ? transform : null;

        currentModelInstance = Instantiate(chosenPrefab, transform.position, transform.rotation, parent);
    }
}
