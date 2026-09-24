using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Timer du niveau")]
    public float levelDuration = 90f;

    [Header("Références UI")]
    public TextMeshProUGUI timerText;

    [Header("Fin du timer")]
    public string gameOverSceneName = "MainMenu";
    public float delayBeforeSceneLoad = 1.5f;

    [Header("Score")]
    [Tooltip("Points de base gagnés en finissant le niveau, avant bonus de temps.")]
    public int scoreBaseNiveau = 10;

    private float _timeRemaining;
    private bool _timerRunning;

    public event System.Action OnTimerFinished;

    public float Timer => _timeRemaining;
    public float MaxTimer => levelDuration;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        StartTimer();
    }

    void Update()
    {
        if (!_timerRunning) return;

        _timeRemaining -= Time.deltaTime;

        if (_timeRemaining <= 0f)
        {
            _timeRemaining = 0f;
            _timerRunning = false;
            UpdateTimerUI();
            OnTimerFinished?.Invoke();
            Debug.Log("⏰ Temps écoulé ! → Retour au menu principal");
            HandleTimeUp();
            return;
        }

        UpdateTimerUI();
    }

    public void StartTimer()
    {
        _timeRemaining = levelDuration;
        _timerRunning = true;
        UpdateTimerUI();
    }

    public void StopTimer() => _timerRunning = false;
    public void SetTimerRunning(bool running) => _timerRunning = running;
    public void SetTimer(float value) { _timeRemaining = value; UpdateTimerUI(); }
    public float GetTimeRemaining() => _timeRemaining;

    // 🔹 NOUVEAU — calcule et ajoute le score du niveau en cours
    /// <summary>
    /// Ajoute au score total les points du niveau (base + temps restant).
    /// À appeler AVANT de charger une nouvelle scène (fin de niveau OU time up).
    /// </summary>
    public void AddLevelScore()
    {
        int currentTotal = PlayerPrefs.GetInt("CurrentGameScore", 0);
        int pointsTemps = Mathf.RoundToInt(_timeRemaining);
        int scoreGagnePourCeNiveau = scoreBaseNiveau + pointsTemps;

        currentTotal += scoreGagnePourCeNiveau;

        PlayerPrefs.SetInt("CurrentGameScore", currentTotal);
        PlayerPrefs.Save();

        Debug.Log($"[Score] +{scoreGagnePourCeNiveau} pts (base {scoreBaseNiveau} + temps {pointsTemps}) → total {currentTotal}");
    }

    // 🔹 NOUVEAU — reset complet du score (à appeler quand on démarre une nouvelle partie)
    public void ResetScore()
    {
        PlayerPrefs.SetInt("CurrentGameScore", 0);
        PlayerPrefs.Save();
        Debug.Log("[Score] Réinitialisé à 0");
    }

    // 🔹 Action déclenchée à la fin du timer
    private void HandleTimeUp()
    {
        // On ajoute le score AVANT de partir au menu
        AddLevelScore();

        if (string.IsNullOrEmpty(gameOverSceneName))
        {
            Debug.LogWarning("Aucune scène de fin renseignée (gameOverSceneName vide).");
            return;
        }

        StartCoroutine(LoadSceneAfterDelay(gameOverSceneName, delayBeforeSceneLoad));
    }

    private System.Collections.IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(_timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(_timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}