using UnityEngine;

/// <summary>
/// Assigns a random sprite from a list to this object's SpriteRenderer.
/// Useful for variety among identical prefabs (rocks, coins, decorations, etc.).
///
/// SETUP:
/// 1. Attach this script to any GameObject that has a SpriteRenderer.
/// 2. Fill the "possibleSprites" list with the sprites to choose from.
/// 3. By default, a random sprite is picked once when the object is created (Awake).
///    Enable "randomizeEveryTimeEnabled" if you want it to re-roll each time
///    the object is enabled (e.g. pulled from an object pool).
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class RandomSprite : MonoBehaviour
{
    [Tooltip("The sprite to choose from at random.")]
    [SerializeField] private Sprite[] possibleSprites;

    [Tooltip("If checked, a new random sprite is picked every time this object is enabled, " +
             "instead of just once on creation.")]
    [SerializeField] private bool randomizeEveryTimeEnabled = false;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        PickRandomSprite();
    }

    private void OnEnable()
    {
        // Skip the very first enable right after Awake to avoid picking twice.
        if (randomizeEveryTimeEnabled && spriteRenderer != null && spriteRenderer.sprite != null)
        {
            PickRandomSprite();
        }
    }

    private void PickRandomSprite()
    {
        if (possibleSprites == null || possibleSprites.Length == 0)
        {
            Debug.LogWarning("[RandomSprite] 'possibleSprites' is empty on " + gameObject.name);
            return;
        }

        Sprite chosen = possibleSprites[Random.Range(0, possibleSprites.Length)];
        spriteRenderer.sprite = chosen;
    }
}
