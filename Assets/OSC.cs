using UnityEngine;

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
}
