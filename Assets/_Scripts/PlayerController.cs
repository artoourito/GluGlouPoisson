using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] private float forceMulti = 1f;

    private Rigidbody _rb;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputManager.Instance.carSteeringWheelAction += OnCarSteeringWheelCalled;
        InputManager.Instance.brakePedalAction += OnBrakePedalCalled;
        InputManager.Instance.switchPedalAction += OnSwitchPedalCalled;
        InputManager.Instance.acceleratorPedalAction += OnAcceleratorPedalCalled;
        InputManager.Instance.automaticTransmissionAction += OnAutomaticTransmissionCalled;
        InputManager.Instance.randomButton1Action += OnRandomButton1Called;
        InputManager.Instance.randomButton2Action += OnRandomButton2Called;
        InputManager.Instance.randomButton3Action += OnRandomButton3Called;

        _rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        InputManager.Instance.carSteeringWheelAction -= OnCarSteeringWheelCalled;
        InputManager.Instance.brakePedalAction -= OnBrakePedalCalled;
        InputManager.Instance.switchPedalAction -= OnSwitchPedalCalled;
        InputManager.Instance.acceleratorPedalAction -= OnAcceleratorPedalCalled;
        InputManager.Instance.automaticTransmissionAction -= OnAutomaticTransmissionCalled;
        InputManager.Instance.randomButton1Action -= OnRandomButton1Called;
        InputManager.Instance.randomButton2Action -= OnRandomButton2Called;
        InputManager.Instance.randomButton3Action -= OnRandomButton3Called;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCarSteeringWheelCalled()
    {
        Debug.Log("Volant");
    }

    void OnBrakePedalCalled()
    {
        Debug.Log("Pedal break");
        _rb.AddForce(Vector3.forward * forceMulti);
    }

    void OnSwitchPedalCalled()
    {
        Debug.Log("Pedal Switch");
    }

    void OnAcceleratorPedalCalled()
    {
        Debug.Log("Pedal d acceleration");
    }

    void OnAutomaticTransmissionCalled()
    {
        Debug.Log("Boite auto");
    }

    private void OnRandomButton1Called()
    {
        Debug.Log("Bouton random 1");
    }

    private void OnRandomButton2Called()
    {
        Debug.Log("Bouton random 2");
    }

    private void OnRandomButton3Called()
    {
        Debug.Log("Bouton random 3");
    }
}
