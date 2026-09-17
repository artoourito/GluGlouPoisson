using UnityEngine;

/// <summary>
/// Fades this object's opacity up as the player gets closer, and back down
/// as they move away. Scales alpha, metallic reflectivity, and emission
/// brightness together, so the object fully disappears rather than leaving
/// visible reflections or a glow behind.
///
/// IMPORTANT: For a Standard shader material, set "Rendering Mode" to "Fade"
/// (not "Transparent") in the material's Inspector. "Transparent" mode does
/// NOT fade specular/metallic reflections, only the base color - "Fade" does.
/// For URP/HDRP, set "Surface Type" to "Transparent".
///
/// SETUP:
/// 1. Attach this script to the object whose opacity should change.
/// 2. Leave "player" empty to auto-find the object tagged "Player".
/// 3. Adjust "minDistance" (opacity = 1) and "maxDistance" (opacity = 0).
/// 4. Uncheck "fadeMetallic" or "fadeEmission" if your material doesn't use them.
/// </summary>
public class FadeWithProximity : MonoBehaviour
{
    [Tooltip("The player's Transform. Leave empty to auto-find the object tagged 'Player'.")]
    [SerializeField] private Transform player;

    [Tooltip("Distance at which the object is fully opaque (opacity = 1).")]
    [SerializeField] private float minDistance = 1f;

    [Tooltip("Distance at which the object is fully transparent (opacity = 0).")]
    [SerializeField] private float maxDistance = 8f;

    [Tooltip("Opacity value used when the player is farther than 'maxDistance'.")]
    [Range(0f, 1f)]
    [SerializeField] private float baseOpacity = 0f;

    [Tooltip("How quickly opacity changes catch up to the target value. Higher = snappier.")]
    [SerializeField] private float fadeSpeed = 5f;

    [Header("Extra properties to fade")]
    [Tooltip("Also scale the material's Metallic value down as it fades, so reflections fade too.")]
    [SerializeField] private bool fadeMetallic = true;

    [Tooltip("Also scale the material's Emission color/brightness down as it fades.")]
    [SerializeField] private bool fadeEmission = true;

    private static readonly int BaseColorURP = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorBuiltIn = Shader.PropertyToID("_Color");
    private static readonly int MetallicId = Shader.PropertyToID("_Metallic");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private Renderer meshRenderer;
    private Material materialInstance;
    private int colorPropertyId;
    private float currentOpacity;

    private float originalMetallic;
    private Color originalEmissionColor;
    private bool hasMetallic;
    private bool hasEmission;

    private void Awake()
    {
        meshRenderer = GetComponent<Renderer>();

        if (meshRenderer != null)
        {
            // .material (not .sharedMaterial) creates a per-instance copy,
            // so fading this object doesn't affect every object sharing the material.
            materialInstance = meshRenderer.material;
            colorPropertyId = materialInstance.HasProperty(BaseColorURP) ? BaseColorURP : ColorBuiltIn;

            hasMetallic = materialInstance.HasProperty(MetallicId);
            if (hasMetallic)
            {
                originalMetallic = materialInstance.GetFloat(MetallicId);
            }

            hasEmission = materialInstance.HasProperty(EmissionColorId);
            if (hasEmission)
            {
                originalEmissionColor = materialInstance.GetColor(EmissionColorId);
                materialInstance.EnableKeyword("_EMISSION");
            }
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        currentOpacity = baseOpacity;
    }

    private void Update()
    {
        if (player == null || materialInstance == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 0 at maxDistance, 1 at minDistance
        float targetOpacity = 1f - Mathf.InverseLerp(minDistance, maxDistance, distance);
        targetOpacity = Mathf.Clamp01(Mathf.Max(targetOpacity, baseOpacity));

        currentOpacity = Mathf.MoveTowards(currentOpacity, targetOpacity, fadeSpeed * Time.deltaTime);

        // Alpha
        Color c = materialInstance.GetColor(colorPropertyId);
        c.a = currentOpacity;
        materialInstance.SetColor(colorPropertyId, c);

        // Metallic (reflections fade with it, since a Metallic of 0 has no reflectivity)
        if (fadeMetallic && hasMetallic)
        {
            materialInstance.SetFloat(MetallicId, originalMetallic * currentOpacity);
        }

        // Emission brightness
        if (fadeEmission && hasEmission)
        {
            materialInstance.SetColor(EmissionColorId, originalEmissionColor * currentOpacity);
        }
    }

    private void OnDestroy()
    {
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
}
