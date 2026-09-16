using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float targetSpeed = 0f;
    [SerializeField] private float accelerationDuration = 1.5f;
    [SerializeField] private float decelerationRate = 3f;

    [Header("Rotation")]
    [SerializeField] private float currentAngle = 0f;
    [SerializeField] private float targetAngle = 0f;
    [SerializeField] private float angleStep = 10f;
    [SerializeField] private float maxAngle = 40f;
    [SerializeField] private float turnSensitivity = 2f;
    [SerializeField] private float rotationSpeed = 45f;

    [Header("Gear")]
    [SerializeField] private int currentGear = 0;
    [SerializeField] private int minGear = -1;
    [SerializeField] private int maxGear = 1;
    [SerializeField] private float maxReverseSpeed = 10f;
    [SerializeField] private float maxGear0Speed = 10f;
    [SerializeField] private float maxGear1Speed = 25f;

    [SerializeField] private List<Transform> wheels = new List<Transform>();

    private Rigidbody _rb;
    private Tween speedTween;
    #endregion

    #region Built-in Methods
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
        Vector2 joystickValue = InputManager.Instance.AutomaticTransmission;

        if (joystickValue.y > 0.8f)
        {
            GearUp();
        }
        else if (joystickValue.y < -0.8f)
        {
            GearDown();
        }

        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        if (moveSpeed > 0.1f)
        {
            float turnAmount = currentAngle * moveSpeed * turnSensitivity * Time.deltaTime;
            transform.Rotate(0, turnAmount, 0);
        }

        float currentRate = (targetSpeed > moveSpeed) ? accelerationDuration : decelerationRate;
        moveSpeed = Mathf.MoveTowards(moveSpeed, targetSpeed, currentRate * Time.deltaTime);
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        foreach (Transform wheel in wheels)
        {
            Vector3 wheelRota = wheel.localEulerAngles;

            wheelRota.y = currentAngle;

            wheel.localRotation = Quaternion.Euler(wheelRota);
        }
    }
    #endregion

    #region Input Methods
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
        GearUp();
    }

    void OnAcceleratorPedalCalled()
    {
        Debug.Log("Pedal d acceleration");
        Accelerator();
    }

    void OnAutomaticTransmissionCalled()
    {
        Debug.Log("Boite auto");
        GearDown();
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
    #endregion

    #region Vehicule Logic Methods
    private void Accelerator()
    {
        float currentMaxSpeed = GetMaxSpeedForCurrentGear();
        targetSpeed = Mathf.Min(currentMaxSpeed, targetSpeed + 2f);
    }

    private void TurnLeft()
    {
        targetAngle = Mathf.Clamp(targetAngle + angleStep, -maxAngle, maxAngle);
    }

    private void TurnRight()
    {
        targetAngle = Mathf.Clamp(targetAngle - angleStep, -maxAngle, maxAngle);
    }

    private void GearUp()
    {
        currentGear = Mathf.Min(currentGear + 1, maxGear);
        UpdateSpeedLimit();
    }

    private void GearDown()
    {
        currentGear = Mathf.Max(0, currentGear - 1);
        UpdateSpeedLimit();
    }

    private void UpdateSpeedLimit()
    {
        float maxAllowedSpeed = 0f;

        switch (currentGear)
        {
            case 0:
                maxAllowedSpeed = maxGear0Speed;
                break;
            case 1:
                maxAllowedSpeed = maxGear1Speed;
                break;
        }

        if (targetSpeed > maxAllowedSpeed)
        {
            targetSpeed = maxAllowedSpeed;
        }
    }

    private float GetMaxSpeedForCurrentGear()
    {
        switch (currentGear)
        {
            case 0: return maxGear0Speed;
            case 1: return maxGear1Speed;
            default: return maxGear0Speed;
        }
    }
    #endregion
}
