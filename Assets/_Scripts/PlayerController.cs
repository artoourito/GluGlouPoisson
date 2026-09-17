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
    [SerializeField] private float decelerationDuration = 4f;
    [SerializeField] private float brakingDuration = 1f;

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
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private bool _isShiftButtonHeld;

    [SerializeField] private List<Transform> wheels = new List<Transform>();

    private Rigidbody _rb;
    private Tween speedTween;
    private bool _joystickIsUsed = false;
    private bool _powerOn = false;
    #endregion

    #region Built-in Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputManager.Instance.startAcceleratorPedalAction += OnAcceleratorPedalStarted;
        InputManager.Instance.cancelAcceleratorPedalAction += OnAcceleratorPedalCanceled;
        InputManager.Instance.startBrakePedalAction += OnBrakePedalStarted;
        InputManager.Instance.cancelBrakePedalAction += OnBrakePedalCanceled;
        InputManager.Instance.startSwitchPedalAction += OnSwitchPedalStarted;
        InputManager.Instance.cancelSwitchPedalAction += OnSwitchPedalCanceled;
        InputManager.Instance.carSteeringWheelAction += OnCarSteeringWheelCalled;
        InputManager.Instance.automaticTransmissionAction += OnAutomaticTransmissionCalled;
        InputManager.Instance.randomButton1Action += OnRandomButton1Called;
        InputManager.Instance.randomButton2Action += OnRandomButton2Called;
        InputManager.Instance.randomButton3Action += OnRandomButton3Called;
        InputManager.Instance.powerButtonAction += OnPowerButtonCalled;

        _rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        InputManager.Instance.startAcceleratorPedalAction -= OnAcceleratorPedalStarted;
        InputManager.Instance.cancelAcceleratorPedalAction -= OnAcceleratorPedalCanceled;
        InputManager.Instance.startBrakePedalAction -= OnBrakePedalStarted;
        InputManager.Instance.cancelBrakePedalAction -= OnBrakePedalCanceled;
        InputManager.Instance.startSwitchPedalAction -= OnSwitchPedalStarted;
        InputManager.Instance.cancelSwitchPedalAction -= OnSwitchPedalCanceled;
        InputManager.Instance.carSteeringWheelAction -= OnCarSteeringWheelCalled;
        InputManager.Instance.automaticTransmissionAction -= OnAutomaticTransmissionCalled;
        InputManager.Instance.randomButton1Action -= OnRandomButton1Called;
        InputManager.Instance.randomButton2Action -= OnRandomButton2Called;
        InputManager.Instance.randomButton3Action -= OnRandomButton3Called;
        InputManager.Instance.powerButtonAction -= OnPowerButtonCalled;
    }

    // Update is called once per frame
    void Update()
    {
        if (_powerOn)
        {
            Vector2 joystickValue = InputManager.Instance.AutomaticTransmission;

            if (_isShiftButtonHeld)
            {
                if (joystickValue.y > 0.8f && !_joystickIsUsed)
                {
                    _joystickIsUsed = true;
                    GearUp();
                }
                else if (joystickValue.y < -0.8f && !_joystickIsUsed)
                {
                    _joystickIsUsed = true;
                    GearDown();
                }
                else if (Mathf.Abs(joystickValue.y) < 0.3f)
                {
                    _joystickIsUsed = false;
                }
            }
        }

        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        if (Mathf.Abs(moveSpeed) > 0.1f)
        {
            float directionMultiplier = (moveSpeed < 0) ? -1f : 1f;

            float turnAmount = currentAngle * Mathf.Abs(moveSpeed) * turnSensitivity * Time.deltaTime * directionMultiplier;
            transform.Rotate(0, turnAmount, 0);
        }

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // rotation des roues
        foreach (Transform wheel in wheels)
        {
            Vector3 wheelRota = wheel.localEulerAngles;

            wheelRota.y = currentAngle;

            wheel.localRotation = Quaternion.Euler(wheelRota);
        }
    }
    #endregion

    #region Input Methods

    void OnAcceleratorPedalStarted()
    {
        Debug.Log("Pedal d acceleration appuyé");
        Accelerator();
    }

    void OnAcceleratorPedalCanceled()
    {
        Debug.Log("Pedal d acceleration relaché");
        targetSpeed = 0f;
        speedTween?.Kill();
        speedTween = DOTween.To(() => moveSpeed, x => moveSpeed = x, 0f, decelerationDuration).SetEase(Ease.OutSine);
    }

    void OnBrakePedalStarted()
    {
        Debug.Log("Pedal break appuyé");
        speedTween?.Kill();
        speedTween = DOTween.To(() => moveSpeed, x => moveSpeed = x, 0f, brakingDuration).SetEase(Ease.OutQuad);
        targetSpeed = 0f;
    }

    void OnBrakePedalCanceled()
    {
        Debug.Log("Pedal break relaché");
    }

    void OnSwitchPedalStarted()
    {
        Debug.Log("Pedal Switch Start");
        _isShiftButtonHeld = true;
    }

    void OnSwitchPedalCanceled()
    {
        Debug.Log("Pedal Switch Canceled");
        _isShiftButtonHeld = false;
    }

    void OnCarSteeringWheelCalled()
    {
        Debug.Log("Volant");
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

    private void OnPowerButtonCalled()
    {
        Power();
    }
    #endregion

    #region Vehicule Logic Methods
    private void Accelerator()
    {
        if (!_powerOn) return;

        float targetGoal = 0f;

        if (currentGear == -1)
        {
            targetGoal = -maxReverseSpeed;
        }
        else if (currentGear == 0)
        {
            targetGoal = maxGear0Speed;
        }
        else if (currentGear == 1)
        {
            targetGoal = maxGear1Speed;
        }

        targetSpeed = targetGoal;

        speedTween?.Kill();

        speedTween = DOTween.To(() => moveSpeed, x => moveSpeed = x, targetSpeed, accelerationDuration).SetEase(accelerationCurve);
    }

    private void TurnRight()
    {
        targetAngle = Mathf.Clamp(targetAngle + angleStep, -maxAngle, maxAngle);
    }

    private void TurnLeft()
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
        currentGear = Mathf.Max(minGear, currentGear - 1);
        UpdateSpeedLimit();
    }

    private void UpdateSpeedLimit()
    {
        float maxAllowedSpeed = GetMaxSpeedForCurrentGear();

        if (Mathf.Abs(moveSpeed) > maxAllowedSpeed)
        {
            speedTween?.Kill();
            float targetGoal = (currentGear == -1) ? -maxReverseSpeed : maxAllowedSpeed;
            speedTween = DOTween.To(() => moveSpeed, x => moveSpeed = x, targetGoal, 1f).SetEase(Ease.OutQuad);
        }
    }

    private float GetMaxSpeedForCurrentGear()
    {
        switch (currentGear)
        {
            case -1: return maxReverseSpeed;
            case 0: return maxGear0Speed;
            case 1: return maxGear1Speed;
            default: return maxGear0Speed;
        }
    }

    private void Power()
    {
        _powerOn = !_powerOn;

        if (_powerOn)
        {
            Debug.Log("La voiture est allumé");
        }
        else
        {
            Debug.Log("la voiture est éteinte");
        }

        speedTween?.Kill();
        speedTween = DOTween.To(() => moveSpeed, x => moveSpeed = x, 0f, decelerationDuration).SetEase(Ease.OutSine);
        targetSpeed = 0f;
    }
    #endregion
}
