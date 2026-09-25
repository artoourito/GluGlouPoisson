using UnityEngine;

/// <summary>
/// Anime en continu un léger squash & stretch sur le scale local de l'objet.
/// À poser sur l'objet qui porte le SpriteRenderer (ex : "Corps" des PNJ).
/// L'échelle de base est lue au démarrage, donc compatible avec n'importe
/// quelle taille de prefab.
/// </summary>
public class SquashAndStretch : MonoBehaviour
{
    [Header("Amplitude")]
    [Tooltip("Étirement vertical max, en fraction de l'échelle de base (0.05 = ±5 %).")]
    [Range(0f, 0.5f)]
    [SerializeField] private float amplitude = 0.05f;

    [Tooltip("Si activé, l'objet s'élargit quand il s'écrase et s'affine quand il s'étire (conservation du volume).")]
    [SerializeField] private bool preserveVolume = true;

    [Tooltip("Poids de la compensation horizontale (1 = compensation complète, 0 = aucune).")]
    [Range(0f, 1f)]
    [SerializeField] private float horizontalWeight = 0.5f;

    [Header("Rythme")]
    [Tooltip("Nombre de cycles par seconde.")]
    [Min(0f)]
    [SerializeField] private float frequency = 1.5f;

    [Tooltip("Décale aléatoirement la phase au démarrage pour que les PNJ ne soient pas synchronisés.")]
    [SerializeField] private bool randomizePhase = true;

    [Tooltip("Forme de l'oscillation sur un cycle (0..1 → -1..1). Laisser vide pour une sinusoïde.")]
    [SerializeField] private AnimationCurve shape = null;

    [Header("Ancrage")]
    [Tooltip("Compense la position verticale pour que la base du sprite reste au sol (pivot centré).")]
    [SerializeField] private bool anchorBottom = false;

    [Tooltip("Demi-hauteur du sprite en unités locales (utilisé seulement si anchorBottom est activé). 0 = auto via SpriteRenderer.")]
    [Min(0f)]
    [SerializeField] private float halfHeight = 0f;

    private Vector3 _baseScale;
    private float _appliedOffsetY;
    private float _phase;
    private float _autoHalfHeight;

    private void Awake()
    {
        _baseScale = transform.localScale;

        if (randomizePhase)
            _phase = Random.value;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
            _autoHalfHeight = sr.sprite.bounds.extents.y;
    }

    private void OnDisable()
    {
        transform.localScale = _baseScale;
        ApplyOffset(0f);
    }

    // Décalage vertical additif : compatible avec les scripts qui déplacent l'objet (MoveBackAndForth…).
    private void ApplyOffset(float offsetY)
    {
        if (Mathf.Approximately(offsetY, _appliedOffsetY))
            return;

        Vector3 p = transform.localPosition;
        p.y += offsetY - _appliedOffsetY;
        transform.localPosition = p;
        _appliedOffsetY = offsetY;
    }

    private void Update()
    {
        if (amplitude <= 0f || frequency <= 0f)
            return;

        float t = (Time.time * frequency + _phase) % 1f;

        float wave;
        if (shape != null && shape.length > 1)
            wave = Mathf.Clamp(shape.Evaluate(t), -1f, 1f);
        else
            wave = Mathf.Sin(t * Mathf.PI * 2f);

        float stretch = 1f + wave * amplitude;
        float squash = 1f;

        if (preserveVolume)
        {
            // 1/sqrt(stretch) conserve le volume ; on pondère pour rester subtil.
            float full = 1f / Mathf.Sqrt(stretch);
            squash = Mathf.Lerp(1f, full, horizontalWeight);
        }

        transform.localScale = new Vector3(
            _baseScale.x * squash,
            _baseScale.y * stretch,
            _baseScale.z * squash
        );

        if (anchorBottom)
        {
            float h = halfHeight > 0f ? halfHeight : _autoHalfHeight;
            ApplyOffset(h * _baseScale.y * (stretch - 1f));
        }
        else
        {
            ApplyOffset(0f);
        }
    }
}
