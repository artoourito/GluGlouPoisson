using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

/// <summary>
/// Pilote la scène de crédits : lit la vidéo depuis StreamingAssets en plein écran,
/// permet d'accélérer / ralentir la lecture (pédales du cockpit ou clavier),
/// garde le mode disco actif, puis revient au menu à la fin de la vidéo.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CreditsController : MonoBehaviour
{
    [Header("Vidéo")]
    [Tooltip("Nom du fichier vidéo dans Assets/StreamingAssets.")]
    [SerializeField] private string videoFileName = "Credits.mp4";

    [Tooltip("Scène chargée à la fin de la vidéo (ou en cas d'erreur).")]
    [SerializeField] private string returnSceneName = "Menu";

    [Tooltip("Volume de la piste audio de la vidéo.")]
    [Range(0f, 1f)]
    [SerializeField] private float videoVolume = 1f;

    [Header("Vitesse de lecture")]
    [Tooltip("Vitesse au démarrage.")]
    [Min(0.1f)]
    [SerializeField] private float defaultSpeed = 1f;

    [Tooltip("Incrément appliqué à chaque appui (accélérer ajoute, ralentir retire).")]
    [Min(0.05f)]
    [SerializeField] private float speedStep = 0.25f;

    [Min(0.1f)]
    [SerializeField] private float minSpeed = 0.25f;

    [Min(0.1f)]
    [SerializeField] private float maxSpeed = 4f;

    [Header("Contrôles cockpit (via InputManager)")]
    [Tooltip("Pédale d'accélérateur = accélérer les crédits.")]
    [SerializeField] private bool acceleratorPedalSpeedsUp = true;

    [Tooltip("Pédale de frein = ralentir les crédits.")]
    [SerializeField] private bool brakePedalSlowsDown = true;

    [Header("Contrôles clavier (secours / éditeur)")]
    [SerializeField] private Key speedUpKey = Key.UpArrow;
    [SerializeField] private Key slowDownKey = Key.DownArrow;
    [SerializeField] private Key skipKey = Key.Escape;

    [Header("Disco")]
    [Tooltip("Effet visuel disco. Si vide, cherché sur le même GameObject.")]
    [SerializeField] private DiscoEffect discoEffect;

    [Tooltip("Identifiant du son disco dans le SoundManager.")]
    [SerializeField] private string discoSoundId = "MusiquePourModeDisco";

    [Header("Ambiance")]
    [Tooltip("Volume de l'ambiance du SoundManager pendant les crédits.")]
    [Range(0f, 1f)]
    [SerializeField] private float ambientVolumeDuringCredits = 0f;

    [Tooltip("Volume d'ambiance restauré en quittant les crédits.")]
    [Range(0f, 1f)]
    [SerializeField] private float ambientVolumeAfterCredits = 0.2f;

    private VideoPlayer _videoPlayer;
    private float _currentSpeed;
    private bool _leaving;

    #region Built-in Methods

    private void Awake()
    {
        Camera cam = GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        if (discoEffect == null)
            discoEffect = GetComponent<DiscoEffect>();

        _currentSpeed = Mathf.Clamp(defaultSpeed, minSpeed, maxSpeed);

        SetupVideoPlayer(cam);
    }

    private void Start()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetAmbientVolume(ambientVolumeDuringCredits);

        if (InputManager.Instance == null)
        {
            Debug.LogWarning("[CreditsController] InputManager.Instance est null : seuls les contrôles clavier sont actifs.");
            return;
        }

        if (acceleratorPedalSpeedsUp)
            InputManager.Instance.startAcceleratorPedalAction += SpeedUp;

        if (brakePedalSlowsDown)
            InputManager.Instance.startBrakePedalAction += SlowDown;

        InputManager.Instance.discoButtonAction += OnDiscoCalled;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.startAcceleratorPedalAction -= SpeedUp;
            InputManager.Instance.startBrakePedalAction -= SlowDown;
            InputManager.Instance.discoButtonAction -= OnDiscoCalled;
        }

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetAmbientVolume(ambientVolumeAfterCredits);

        if (_videoPlayer != null)
        {
            _videoPlayer.loopPointReached -= OnVideoFinished;
            _videoPlayer.errorReceived -= OnVideoError;
            _videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null)
            return;

        if (speedUpKey != Key.None && kb[speedUpKey].wasPressedThisFrame)
            SpeedUp();

        if (slowDownKey != Key.None && kb[slowDownKey].wasPressedThisFrame)
            SlowDown();

        if (skipKey != Key.None && kb[skipKey].wasPressedThisFrame)
            ReturnToMenu();
    }

    #endregion

    #region Video

    private void SetupVideoPlayer(Camera cam)
    {
        _videoPlayer = gameObject.GetComponent<VideoPlayer>();
        if (_videoPlayer == null)
            _videoPlayer = gameObject.AddComponent<VideoPlayer>();

        _videoPlayer.playOnAwake = false;
        _videoPlayer.waitForFirstFrame = true;
        _videoPlayer.isLooping = false;
        _videoPlayer.skipOnDrop = true;

        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.url = Path.Combine(Application.streamingAssetsPath, videoFileName);

        _videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        _videoPlayer.targetCamera = cam;
        _videoPlayer.aspectRatio = VideoAspectRatio.FitInside;

        _videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        _videoPlayer.SetDirectAudioVolume(0, videoVolume);

        _videoPlayer.loopPointReached += OnVideoFinished;
        _videoPlayer.errorReceived += OnVideoError;
        _videoPlayer.prepareCompleted += OnVideoPrepared;

        _videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        ApplySpeed();
        vp.Play();
        Debug.Log($"[CreditsController] Lecture de {videoFileName} ({vp.length:0.#} s)");
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        ReturnToMenu();
    }

    private void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.LogError($"[CreditsController] Erreur vidéo : {message}");
        ReturnToMenu();
    }

    private void ReturnToMenu()
    {
        if (_leaving)
            return;

        _leaving = true;

        if (_videoPlayer != null && _videoPlayer.isPlaying)
            _videoPlayer.Stop();

        if (string.IsNullOrEmpty(returnSceneName))
        {
            Debug.LogWarning("[CreditsController] 'returnSceneName' n'est pas renseigné.");
            return;
        }

        SceneManager.LoadScene(returnSceneName);
    }

    #endregion

    #region Speed

    public void SpeedUp() => SetSpeed(_currentSpeed + speedStep);

    public void SlowDown() => SetSpeed(_currentSpeed - speedStep);

    public void SetSpeed(float speed)
    {
        _currentSpeed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        ApplySpeed();
        Debug.Log($"[CreditsController] Vitesse des crédits : x{_currentSpeed:0.##}");
    }

    private void ApplySpeed()
    {
        if (_videoPlayer == null)
            return;

        if (!_videoPlayer.canSetPlaybackSpeed)
        {
            Debug.LogWarning("[CreditsController] Cette plateforme ne permet pas de changer la vitesse de lecture.");
            return;
        }

        _videoPlayer.playbackSpeed = _currentSpeed;
    }

    #endregion

    #region Disco

    private void OnDiscoCalled()
    {
        float duration = 0f;

        if (SoundManager.Instance != null && !string.IsNullOrEmpty(discoSoundId))
        {
            SoundManager.Instance.Play(discoSoundId);
            duration = SoundManager.Instance.GetClipLength(discoSoundId);
        }

        if (discoEffect != null)
            discoEffect.Play(duration);
    }

    #endregion
}
