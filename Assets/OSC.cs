using UnityEngine;
using UnityEngine.SceneManagement;

public class OSC : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    public static OSC instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // On s'abonne à l'événement quand l'objet se réveille/s'active
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // On se désabonne (très important pour éviter les bugs de mémoire)
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ReceiveOSC(float value)
    {
        if (playerController != null)
        {
            playerController.SetSteeringInput(value);
            Debug.Log(value);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }
}
