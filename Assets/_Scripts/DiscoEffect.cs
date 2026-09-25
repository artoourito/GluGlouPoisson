using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Halos lumineux colorés dans les quatre coins de l'écran pendant le mode disco.
/// Overlay UI pur (aucune Light 3D, aucun shader) : un Canvas est construit à la
/// volée au premier Play() et vit sous ce GameObject, donc disparaît avec la voiture.
/// </summary>
public class DiscoEffect : MonoBehaviour
{
    [Header("Couleurs")]
    [Tooltip("Palette parcourue en boucle par chaque coin.")]
    [SerializeField]
    private Color[] palette =
    {
        Color.red, Color.green, Color.blue,
        Color.magenta, Color.yellow, Color.cyan
    };

    [Tooltip("Vitesse du cycle, en couleurs par seconde.")]
    [Min(0f)]
    [SerializeField] private float colorCycleSpeed = 1f;

    [Tooltip("Décalage d'index de palette entre deux coins consécutifs (0 = tous synchronisés).")]
    [Min(0f)]
    [SerializeField] private float cornerPhaseOffset = 1.5f;

    [Header("Pulsation")]
    [Tooltip("Fréquence de la pulsation d'intensité, en Hz.")]
    [Min(0f)]
    [SerializeField] private float pulseFrequency = 2f;

    [Range(0f, 1f)]
    [SerializeField] private float minAlpha = 0.15f;

    [Range(0f, 1f)]
    [SerializeField] private float maxAlpha = 0.8f;

    [Header("Forme")]
    [Tooltip("Rayon du halo en fraction de la hauteur de référence (1080 px).")]
    [Range(0.1f, 1.5f)]
    [SerializeField] private float haloSize = 0.6f;

    [Tooltip("Résolution de la texture radiale générée.")]
    [SerializeField] private int textureSize = 256;

    [Header("Durées")]
    [Min(0f)]
    [SerializeField] private float fadeInDuration = 0.3f;

    [Min(0f)]
    [SerializeField] private float fadeOutDuration = 0.6f;

    [Tooltip("Durée utilisée si la durée du clip audio est inconnue (Play(0)).")]
    [Min(0.1f)]
    [SerializeField] private float fallbackDuration = 10f;

    [Header("Rendu")]
    [Tooltip("Ordre de tri du Canvas overlay (au-dessus du HUD).")]
    [SerializeField] private int sortingOrder = 100;

    public bool IsPlaying { get; private set; }

    private GameObject _overlay;
    private Image[] _halos;
    private Texture2D _texture;
    private Sprite _sprite;

    private float _elapsed;
    private float _duration;

    private const float ReferenceHeight = 1080f;

    #region Public API

    /// <summary>Démarre l'effet pour <paramref name="duration"/> secondes (≤ 0 → durée de repli).
    /// Si l'effet tourne déjà, il est prolongé sans relancer le fade-in.</summary>
    public void Play(float duration)
    {
        if (duration <= 0f)
            duration = fallbackDuration;

        EnsureBuilt();

        if (IsPlaying)
        {
            float remaining = _duration - _elapsed;
            if (duration > remaining)
                _duration = _elapsed + duration;
            return;
        }

        _elapsed = 0f;
        _duration = duration;
        IsPlaying = true;
        _overlay.SetActive(true);
        ApplyFrame();
    }

    /// <summary>Déclenche le fade-out immédiatement.</summary>
    public void Stop()
    {
        if (!IsPlaying)
            return;

        _duration = Mathf.Min(_duration, _elapsed + fadeOutDuration);
    }

    #endregion

    #region Built-in Methods

    private void Update()
    {
        if (!IsPlaying)
            return;

        _elapsed += Time.deltaTime;

        if (_elapsed >= _duration)
        {
            Finish();
            return;
        }

        ApplyFrame();
    }

    private void OnDisable()
    {
        Finish();
    }

    private void OnDestroy()
    {
        if (_sprite != null) Destroy(_sprite);
        if (_texture != null) Destroy(_texture);
    }

    #endregion

    #region Animation

    private void ApplyFrame()
    {
        float envelope = 1f;

        if (fadeInDuration > 0f)
            envelope = Mathf.Min(envelope, _elapsed / fadeInDuration);

        if (fadeOutDuration > 0f)
            envelope = Mathf.Min(envelope, (_duration - _elapsed) / fadeOutDuration);

        envelope = Mathf.Clamp01(envelope);

        float pulse01 = (Mathf.Sin(2f * Mathf.PI * pulseFrequency * _elapsed) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, pulse01) * envelope;

        for (int i = 0; i < _halos.Length; i++)
        {
            Color c = SampleColor(_elapsed * colorCycleSpeed + i * cornerPhaseOffset);
            c.a = alpha;
            _halos[i].color = c;
        }
    }

    private Color SampleColor(float index)
    {
        if (palette == null || palette.Length == 0)
            return Color.white;

        int n = palette.Length;
        int floor = Mathf.FloorToInt(index);
        float t = index - floor;

        int a = ((floor % n) + n) % n;
        int b = (a + 1) % n;

        return Color.Lerp(palette[a], palette[b], t);
    }

    private void Finish()
    {
        IsPlaying = false;

        if (_overlay != null)
            _overlay.SetActive(false);
    }

    #endregion

    #region Construction

    private void EnsureBuilt()
    {
        if (_overlay != null)
            return;

        BuildSprite();

        _overlay = new GameObject("DiscoOverlay");
        _overlay.transform.SetParent(transform, false);

        Canvas canvas = _overlay.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = sortingOrder;

        CanvasScaler scaler = _overlay.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, ReferenceHeight);
        scaler.matchWidthOrHeight = 0.5f;

        // Coin : bas-gauche, bas-droite, haut-gauche, haut-droite.
        Vector2[] corners =
        {
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f)
        };
        string[] names = { "Halo_BL", "Halo_BR", "Halo_TL", "Halo_TR" };

        float size = haloSize * ReferenceHeight * 2f;

        _halos = new Image[corners.Length];

        for (int i = 0; i < corners.Length; i++)
        {
            GameObject go = new GameObject(names[i]);
            go.transform.SetParent(_overlay.transform, false);

            Image img = go.AddComponent<Image>();
            img.sprite = _sprite;
            img.raycastTarget = false;
            img.color = new Color(1f, 1f, 1f, 0f);

            // Ancré au coin, pivot au centre : le centre du halo est sur le coin,
            // seul un quart du disque est visible (lumière qui déborde du bord).
            RectTransform rt = img.rectTransform;
            rt.anchorMin = corners[i];
            rt.anchorMax = corners[i];
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(size, size);

            _halos[i] = img;
        }

        _overlay.SetActive(false);
    }

    private void BuildSprite()
    {
        int n = Mathf.Max(16, textureSize);

        _texture = new Texture2D(n, n, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            name = "DiscoHalo"
        };

        Color[] pixels = new Color[n * n];
        float half = (n - 1) * 0.5f;

        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                float dx = (x - half) / half;
                float dy = (y - half) / half;
                float d = Mathf.Sqrt(dx * dx + dy * dy);

                float a = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(1f - d));
                pixels[y * n + x] = new Color(1f, 1f, 1f, a);
            }
        }

        _texture.SetPixels(pixels);
        _texture.Apply(false, true);

        _sprite = Sprite.Create(
            _texture,
            new Rect(0f, 0f, n, n),
            new Vector2(0.5f, 0.5f),
            100f
        );
        _sprite.name = "DiscoHalo";
    }

    #endregion
}
