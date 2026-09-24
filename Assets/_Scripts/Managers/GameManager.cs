using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private float timer;
    [SerializeField] private float maxTimer = 60f;
    [SerializeField] private AudioClip gameoverSound;

    private bool _timerStop;
    private static GameManager _instance;
    private AudioSource _audioSource;
    #endregion

    #region Properties
    public float Timer => timer;
    public static GameManager Instance => _instance;
    #endregion

    #region Built-in Methods
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        timer = maxTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timerStop == false)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                Debug.Log("Timer terminé");
                
                _timerStop = true;
                StartCoroutine(GameOver());
                return;
            }
        }
    }
    #endregion

    IEnumerator GameOver()
    {
        _audioSource.clip = gameoverSound;
        _audioSource.Play();
        yield return new WaitForSeconds(3f);
        SaveCurrentScoreToHighScore();
        SceneManager.LoadScene("Menu");
    }

    private void SaveCurrentScoreToHighScore()
    {
        int currentScore = PlayerPrefs.GetInt("CurrentGameScore", 0);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }
    }
}
