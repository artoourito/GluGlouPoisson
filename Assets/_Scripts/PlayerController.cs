using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float targetSpeed = 0f;
    [SerializeField] private float accelerationRate = 2f;
    [SerializeField] private float decelerationRate = 3f;

    [SerializeField] private float currentAngle = 0f;
    [SerializeField] private float targetAngle = 0f;
    [SerializeField] private float angleStep = 10f;
    [SerializeField] private float maxAngle = 40f;
    [SerializeField] private float turnSensitivity = 2f;
    [SerializeField] private float rotationSpeed = 45f;

    [SerializeField] private List<Transform> wheels = new List<Transform>();

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
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        if (moveSpeed > 0.1f)
        {
            float turnAmount = currentAngle * moveSpeed * turnSensitivity * Time.deltaTime;
            transform.Rotate(0, turnAmount, 0);
        }

        float currentRate = (targetSpeed > moveSpeed) ? accelerationRate : decelerationRate;
        moveSpeed = Mathf.MoveTowards(moveSpeed, targetSpeed, currentRate * Time.deltaTime);
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        foreach (Transform wheel in wheels) 
        {
            Vector3 wheelRota = wheel.localEulerAngles;

            wheelRota.y = currentAngle;

            wheel.localRotation = Quaternion.Euler(wheelRota);
        }
    }

    void OnCarSteeringWheelCalled()
    {
        Debug.Log("Volant");
    }

    void OnBrakePedalCalled()
    {
        Debug.Log("Pedal break");
        targetSpeed = 0f;
    }

    void OnSwitchPedalCalled()
    {
        Debug.Log("Pedal Switch");
    }

    void OnAcceleratorPedalCalled()
    {
        Debug.Log("Pedal d acceleration");
        targetSpeed = 2f;
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
        TurnRight();
    }

    private void OnRandomButton3Called()
    {
        Debug.Log("Bouton random 3");
        TurnLeft();
    }

    private void TurnLeft()
    {
        targetAngle = Mathf.Clamp(targetAngle + angleStep, -maxAngle, maxAngle);
    }

    private void TurnRight()
    {
        targetAngle = Mathf.Clamp(targetAngle - angleStep, -maxAngle, maxAngle);
    }
}
