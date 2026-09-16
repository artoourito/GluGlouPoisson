using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private float timer;
    [SerializeField] private float maxTimer = 60f;

    private bool _timerStop;
    private static GameManager _instance;
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
                return;
            }
        }
    }
    #endregion
}
