using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private AnimationCurve accelerationCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool _isShiftButtonHeld;

    [SerializeField] private List<Transform> wheels =
        new List<Transform>();

    private Rigidbody _rb;

    private bool _joystickIsUsed = false;
    private bool _powerOn = false;

    private float _transitionStartSpeed = 0f;
    private float _transitionTargetSpeed = 0f;
    private float _transitionDuration = 1f;
    private float _transitionElapsedTime = 0f;

    private bool _isTransitioning = false;

    private AnimationCurve _activeCurve = null;

    private float _bounceTimer = 0f;

    #endregion


    #region Built-in Methods

    private void Start()
    {
        InputManager.Instance.startAcceleratorPedalAction +=
            OnAcceleratorPedalStarted;

        InputManager.Instance.cancelAcceleratorPedalAction +=
            OnAcceleratorPedalCanceled;

        InputManager.Instance.startBrakePedalAction +=
            OnBrakePedalStarted;

        InputManager.Instance.cancelBrakePedalAction +=
            OnBrakePedalCanceled;

        InputManager.Instance.startSwitchPedalAction +=
            OnSwitchPedalStarted;

        InputManager.Instance.cancelSwitchPedalAction +=
            OnSwitchPedalCanceled;

        InputManager.Instance.carSteeringWheelAction +=
            OnCarSteeringWheelCalled;

        InputManager.Instance.automaticTransmissionAction +=
            OnAutomaticTransmissionCalled;

        InputManager.Instance.randomButton1Action +=
            OnRandomButton1Called;

        InputManager.Instance.randomButton2Action +=
            OnRandomButton2Called;

        InputManager.Instance.randomButton3Action +=
            OnRandomButton3Called;

        InputManager.Instance.powerButtonAction +=
            OnPowerButtonCalled;

        _rb = GetComponent<Rigidbody>();
    }


    private void OnDisable()
    {
        InputManager.Instance.startAcceleratorPedalAction -=
            OnAcceleratorPedalStarted;

        InputManager.Instance.cancelAcceleratorPedalAction -=
            OnAcceleratorPedalCanceled;

        InputManager.Instance.startBrakePedalAction -=
            OnBrakePedalStarted;

        InputManager.Instance.cancelBrakePedalAction -=
            OnBrakePedalCanceled;

        InputManager.Instance.startSwitchPedalAction -=
            OnSwitchPedalStarted;

        InputManager.Instance.cancelSwitchPedalAction -=
            OnSwitchPedalCanceled;

        InputManager.Instance.carSteeringWheelAction -=
            OnCarSteeringWheelCalled;

        InputManager.Instance.automaticTransmissionAction -=
            OnAutomaticTransmissionCalled;

        InputManager.Instance.randomButton1Action -=
            OnRandomButton1Called;

        InputManager.Instance.randomButton2Action -=
            OnRandomButton2Called;

        InputManager.Instance.randomButton3Action -=
            OnRandomButton3Called;

        InputManager.Instance.powerButtonAction -=
            OnPowerButtonCalled;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopLoop();
        }
    }


    private void Update()
    {
        if (_bounceTimer > 0f)
        {
            _bounceTimer -= Time.deltaTime;
            return;
        }


        if (_isTransitioning)
        {
            _transitionElapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(
                _transitionElapsedTime / _transitionDuration
            );

            float evaluationFactor = t;

            if (_activeCurve != null)
            {
                evaluationFactor = _activeCurve.Evaluate(t);
            }

            moveSpeed = Mathf.Lerp(
                _transitionStartSpeed,
                _transitionTargetSpeed,
                evaluationFactor
            );

            targetSpeed = _transitionTargetSpeed;

            if (t >= 1f)
            {
                _isTransitioning = false;
            }
        }


        if (_powerOn)
        {
            Vector2 joystickValue =
                InputManager.Instance.AutomaticTransmission;

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


        currentAngle = Mathf.MoveTowards(
            currentAngle,
            targetAngle,
            rotationSpeed * Time.deltaTime
        );


        if (Mathf.Abs(moveSpeed) > 0.1f)
        {
            float directionMultiplier =
                (moveSpeed < 0) ? -1f : 1f;

            float turnAmount =
                currentAngle *
                Mathf.Abs(moveSpeed) *
                turnSensitivity *
                Time.deltaTime *
                directionMultiplier;

            transform.Rotate(0, turnAmount, 0);
        }


        Vector3 targetVelocity =
            transform.forward * moveSpeed;

        targetVelocity.y = _rb.linearVelocity.y;

        _rb.linearVelocity = targetVelocity;


        foreach (Transform wheel in wheels)
        {
            Vector3 wheelRota =
                wheel.localEulerAngles;

            wheelRota.y = currentAngle;

            wheel.localRotation =
                Quaternion.Euler(wheelRota);
        }
    }

    #endregion


    #region Input Methods

    private void OnAcceleratorPedalStarted()
    {
        Debug.Log("Pedal d'acceleration appuyé");

        SoundManager.Instance.Play("AcceleratorPress");
        SoundManager.Instance.PlayLoop("AcceleratorLoop");

        Accelerator();
    }


    private void OnAcceleratorPedalCanceled()
    {
        Debug.Log("Pedal d'acceleration relâché");

        SoundManager.Instance.Play("AcceleratorRelease");
        SoundManager.Instance.StopLoop();

        targetSpeed = 0f;

        StartSpeedTransition(
            0f,
            decelerationDuration,
            null
        );
    }


    private void OnBrakePedalStarted()
    {
        Debug.Log("Pedal de frein appuyé");

        SoundManager.Instance.Play("BrakePress");

        StartSpeedTransition(
            0f,
            brakingDuration,
            null
        );
    }


    private void OnBrakePedalCanceled()
    {
        Debug.Log("Pedal de frein relâché");

        SoundManager.Instance.Play("BrakeRelease");
    }


    private void OnSwitchPedalStarted()
    {
        Debug.Log("Pedal Switch Start");

        _isShiftButtonHeld = true;
    }


    private void OnSwitchPedalCanceled()
    {
        Debug.Log("Pedal Switch Canceled");

        _isShiftButtonHeld = false;
    }


    private void OnCarSteeringWheelCalled()
    {
        Debug.Log("Volant");
    }


    private void OnAutomaticTransmissionCalled()
    {
        Debug.Log("Boite auto");
    }


    private void OnRandomButton1Called()
    {
        Debug.Log("Bouton random 1");

        SoundManager.Instance.Play("RandomButton1");
    }


    private void OnRandomButton2Called()
    {
        Debug.Log("Bouton random 2");

        SoundManager.Instance.Play("RandomButton2");

        TurnRight();
    }


    private void OnRandomButton3Called()
    {
        Debug.Log("Bouton random 3");

        SoundManager.Instance.Play("RandomButton3");

        TurnLeft();
    }


    private void OnPowerButtonCalled()
    {
        Power();
    }

    #endregion


    #region Vehicle Logic Methods

    public void SetSteeringInput(float rawAngle)
    {
        targetAngle = Mathf.Clamp(
            rawAngle,
            -maxAngle,
            maxAngle
        );
    }


    private void StartSpeedTransition(
        float target,
        float duration,
        AnimationCurve curve)
    {
        _transitionStartSpeed = moveSpeed;
        _transitionTargetSpeed = target;
        _transitionDuration = duration;
        _transitionElapsedTime = 0f;
        _activeCurve = curve;

        _isTransitioning = true;
    }


    private void Accelerator()
    {
        if (!_powerOn)
        {
            return;
        }

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

        StartSpeedTransition(
            targetGoal,
            accelerationDuration,
            accelerationCurve
        );
    }


    private void TurnRight()
    {
        targetAngle = Mathf.Clamp(
            targetAngle + angleStep,
            -maxAngle,
            maxAngle
        );
    }


    private void TurnLeft()
    {
        targetAngle = Mathf.Clamp(
            targetAngle - angleStep,
            -maxAngle,
            maxAngle
        );
    }


    private void GearUp()
    {
        int previousGear = currentGear;

        currentGear = Mathf.Min(
            currentGear + 1,
            maxGear
        );

        if (currentGear != previousGear)
        {
            SoundManager.Instance.Play("GearSwitch");
        }

        UpdateSpeedLimit();
    }


    private void GearDown()
    {
        int previousGear = currentGear;

        currentGear = Mathf.Max(
            minGear,
            currentGear - 1
        );

        if (currentGear != previousGear)
        {
            SoundManager.Instance.Play("GearSwitch");
        }

        UpdateSpeedLimit();
    }


    private void UpdateSpeedLimit()
    {
        float maxAllowedSpeed =
            GetMaxSpeedForCurrentGear();

        if (Mathf.Abs(moveSpeed) > maxAllowedSpeed)
        {
            float targetGoal =
                (currentGear == -1)
                ? -maxReverseSpeed
                : maxAllowedSpeed;

            StartSpeedTransition(
                targetGoal,
                1f,
                null
            );
        }
    }


    private float GetMaxSpeedForCurrentGear()
    {
        switch (currentGear)
        {
            case -1:
                return maxReverseSpeed;

            case 0:
                return maxGear0Speed;

            case 1:
                return maxGear1Speed;

            default:
                return maxGear0Speed;
        }
    }


    private void Power()
    {
        _powerOn = !_powerOn;

        if (_powerOn)
        {
            Debug.Log("La voiture est allumée");

            SoundManager.Instance.Play("Contact");
        }
        else
        {
            Debug.Log("La voiture est éteinte");

            SoundManager.Instance.Play("Contact");
        }

        StartSpeedTransition(
            0f,
            decelerationDuration,
            null
        );
    }


    public void OnBounced()
    {
        moveSpeed = 0f;
        targetSpeed = 0f;
        _isTransitioning = false;
        _bounceTimer = 0.3f;
    }

    #endregion
}