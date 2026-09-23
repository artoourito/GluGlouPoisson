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

    [SerializeField] private List<Transform> wheels = new List<Transform>();

    [Header("Audio - Sons par action")]
    [SerializeField] private AudioSource keyPressAudioSource;

    [SerializeField] private AudioClip contactSound;
    [SerializeField] private AudioClip acceleratorPressSound;
    [SerializeField] private AudioClip brakeSound;
    [SerializeField] private AudioClip gearSwitchSound;
    [SerializeField] private AudioClip randomButton2Sound;
    [SerializeField] private AudioClip randomButton3Sound;

    [Header("Audio - Acceleration")]
    [SerializeField] private AudioSource acceleratorAudioSource;
    [SerializeField] private AudioClip acceleratorLoopSound;

    [Header("Audio - Moteur")]
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private AudioClip engineSound;

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

        // Engine sound
        if (engineAudioSource != null)
        {
            engineAudioSource.clip = engineSound;
            engineAudioSource.loop = true;
        }

        // Accelerator loop sound
        if (acceleratorAudioSource != null)
        {
            acceleratorAudioSource.clip = acceleratorLoopSound;
            acceleratorAudioSource.loop = true;
        }
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

        StopAcceleratorSound();
    }


    void Update()
    {
        if (_bounceTimer > 0f)
        {
            _bounceTimer -= Time.deltaTime;
            return;
        }

        // Speed transition
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

        // Gear shifting
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

        // Rotation
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

        // Movement
        Vector3 targetVelocity =
            transform.forward * moveSpeed;

        targetVelocity.y = _rb.linearVelocity.y;

        _rb.linearVelocity = targetVelocity;

        // Wheel rotation
        foreach (Transform wheel in wheels)
        {
            Vector3 wheelRota = wheel.localEulerAngles;

            wheelRota.y = currentAngle;

            wheel.localRotation =
                Quaternion.Euler(wheelRota);
        }
    }

    #endregion


    #region Input Methods

    void OnAcceleratorPedalStarted()
    {
        Debug.Log("Pedal d acceleration appuyé");

        // One click sound
        PlayActionSound(acceleratorPressSound);

        // Start looping accelerator sound
        StartAcceleratorSound();

        Accelerator();
    }


    void OnAcceleratorPedalCanceled()
    {
        Debug.Log("Pedal d acceleration relaché");

        // Stop looping accelerator sound
        StopAcceleratorSound();

        targetSpeed = 0f;

        StartSpeedTransition(
            0f,
            decelerationDuration,
            null
        );
    }


    void OnBrakePedalStarted()
    {
        Debug.Log("Pedal brake appuyé");

        // One sound on press
        PlayActionSound(brakeSound);

        StartSpeedTransition(
            0f,
            brakingDuration,
            null
        );
    }


    void OnBrakePedalCanceled()
    {
        Debug.Log("Pedal brake relâché");
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

        // No sound for turning
    }


    void OnAutomaticTransmissionCalled()
    {
        Debug.Log("Boite auto");

        // No sound
    }


    private void OnRandomButton1Called()
    {
        Debug.Log("Bouton random 1");

        // No sound requested for button 1
    }


    private void OnRandomButton2Called()
    {
        Debug.Log("Bouton random 2");

        PlayActionSound(randomButton2Sound);

        TurnRight();
    }


    private void OnRandomButton3Called()
    {
        Debug.Log("Bouton random 3");

        PlayActionSound(randomButton3Sound);

        TurnLeft();
    }


    private void OnPowerButtonCalled()
    {
        // Contact sound
        PlayActionSound(contactSound);

        Power();
    }

    #endregion


    #region Vehicule Logic Methods

    public void SetSteeringInput(float rawAngle)
    {
        // Limit the received angle
        targetAngle = Mathf.Clamp(
            rawAngle,
            -maxAngle,
            maxAngle
        );
    }


    private void StartSpeedTransition(
        float target,
        float duration,
        AnimationCurve curve
    )
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
            return;

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

        // Only play if the gear actually changed
        if (currentGear != previousGear)
        {
            PlayActionSound(gearSwitchSound);
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

        // Only play if the gear actually changed
        if (currentGear != previousGear)
        {
            PlayActionSound(gearSwitchSound);
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
            Debug.Log("La voiture est allumé");

            if (engineAudioSource != null &&
                !engineAudioSource.isPlaying)
            {
                engineAudioSource.Play();
            }
        }
        else
        {
            Debug.Log("La voiture est éteinte");

            if (engineAudioSource != null)
            {
                engineAudioSource.Stop();
            }

            // Make sure acceleration loop also stops
            StopAcceleratorSound();
        }

        StartSpeedTransition(
            0f,
            decelerationDuration,
            null
        );
    }


    private void PlayActionSound(AudioClip clip)
    {
        if (keyPressAudioSource != null &&
            clip != null)
        {
            keyPressAudioSource.PlayOneShot(clip);
        }
    }


    private void StartAcceleratorSound()
    {
        if (acceleratorAudioSource != null &&
            !acceleratorAudioSource.isPlaying)
        {
            acceleratorAudioSource.Play();
        }
    }


    private void StopAcceleratorSound()
    {
        if (acceleratorAudioSource != null &&
            acceleratorAudioSource.isPlaying)
        {
            acceleratorAudioSource.Stop();
        }
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