using UnityEngine;

public class OSC : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void ReceiveOSC(float value)
    {
        if (playerController != null)
        {
            playerController.SetSteeringInput(value);
            Debug.Log(value);
        }
    }
}
