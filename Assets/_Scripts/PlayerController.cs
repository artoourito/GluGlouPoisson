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

    public int CurrentGear
    {
        get { return currentGear; }
    }

    [SerializeField] private float maxReverseSpeed = 10f;
    [SerializeField] private float maxGear0Speed = 10f;
    [SerializeField] private float maxGear1Speed = 25f;

    [SerializeField]
    private AnimationCurve accelerationCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Sounds")]
    [SerializeField] private string discoSoundId;
    [SerializeField] private string honkSoundId;
    [SerializeField] private string warningsSoundId;

    [SerializeField]
    private List<Transform> wheels =
        new List<Transform>();

    private Rigidbody _rb;

    private float _transitionStartSpeed = 0f;
    private float _transitionTargetSpeed = 0f;
    private float _transitionDuration = 1f;
    private float _transitionElapsedTime = 0f;

    private bool _isTransitioning = false;

    private AnimationCurve _activeCurve = null;

    private float _bounceTimer = 0f;

    private bool _powerOn = false;

    // Vrai tant que la pédale d'embrayage (Switch Pedal) est maintenue enfoncée.
    private bool _switchPedalHeld = false;

    #endregion

    #region Built-in Methods

    private void Start()
    {
        InputManager.Instance.startAcceleratorPedalAction += OnAcceleratorPedalStarted;
        InputManager.Instance.cancelAcceleratorPedalAction += OnAcceleratorPedalCanceled;

        InputManager.Instance.startBrakePedalAction += OnBrakePedalStarted;
        InputManager.Instance.cancelBrakePedalAction += OnBrakePedalCanceled;

        InputManager.Instance.startSwitchPedalAction += OnSwitchPedalStarted;
        InputManager.Instance.cancelSwitchPedalAction += OnSwitchPedalCanceled;

        InputManager.Instance.carSteeringWheelAction += OnCarSteeringWheelCalled;
        InputManager.Instance.automaticTransmissionAction += OnAutomaticTransmissionCalled;

        // RANDOM BUTTON 1 = KLAXON
        InputManager.Instance.randomButton1Action += OnHonkStarted;
        InputManager.Instance.randomButton1CanceledAction += OnHonkCanceled;

        // RANDOM BUTTON 2 = WARNINGS
        InputManager.Instance.randomButton2Action += OnWarningsCalled;

        // DISCO
        InputManager.Instance.discoButtonAction += OnDiscoCalled;

        InputManager.Instance.powerButtonAction += OnPowerButtonCalled;

        InputManager.Instance.gearUpAction += GearUp;
        InputManager.Instance.gearDownAction += GearDown;

        _rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null)
            return;

        InputManager.Instance.startAcceleratorPedalAction -= OnAcceleratorPedalStarted;
        InputManager.Instance.cancelAcceleratorPedalAction -= OnAcceleratorPedalCanceled;

        InputManager.Instance.startBrakePedalAction -= OnBrakePedalStarted;
        InputManager.Instance.cancelBrakePedalAction -= OnBrakePedalCanceled;

        InputManager.Instance.startSwitchPedalAction -= OnSwitchPedalStarted;
        InputManager.Instance.cancelSwitchPedalAction -= OnSwitchPedalCanceled;

        InputManager.Instance.carSteeringWheelAction -= OnCarSteeringWheelCalled;
        InputManager.Instance.automaticTransmissionAction -= OnAutomaticTransmissionCalled;

        InputManager.Instance.randomButton1Action -= OnHonkStarted;
        InputManager.Instance.randomButton1CanceledAction -= OnHonkCanceled;

        InputManager.Instance.randomButton2Action -= OnWarningsCalled;

        InputManager.Instance.discoButtonAction -= OnDiscoCalled;

        InputManager.Instance.powerButtonAction -= OnPowerButtonCalled;

        InputManager.Instance.gearUpAction -= GearUp;
        InputManager.Instance.gearDownAction -= GearDown;
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
                evaluationFactor = _activeCurve.Evaluate(t);

            moveSpeed = Mathf.Lerp(
                _transitionStartSpeed,
                _transitionTargetSpeed,
                evaluationFactor
            );

            targetSpeed = _transitionTargetSpeed;

            if (t >= 1f)
                _isTransitioning = false;
        }

        currentAngle = Mathf.MoveTowards(
            currentAngle,
            targetAngle,
            rotationSpeed * Time.deltaTime
        );

        if (Mathf.Abs(moveSpeed) > 0.1f)
        {
            float directionMultiplier =
                moveSpeed < 0 ? -1f : 1f;

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

        targetVelocity.y =
            _rb.linearVelocity.y;

        _rb.linearVelocity =
            targetVelocity;

        foreach (Transform wheel in wheels)
        {
            Vector3 wheelRotation =
                wheel.localEulerAngles;

            wheelRotation.y =
                currentAngle;

            wheel.localRotation =
                Quaternion.Euler(wheelRotation);
        }
    }

    #endregion

    #region Input Methods

    private void OnAcceleratorPedalStarted()
    {
        Debug.Log("Pedal d'acceleration appuyé");
        Accelerator();
    }

    private void OnAcceleratorPedalCanceled()
    {
        Debug.Log("Pedal d'acceleration relâché");

        targetSpeed = 0f;

        StartSpeedTransition(
            0f,
            decelerationDuration,
            null
        );
    }

    private void OnBrakePedalStarted()
    {
        Debug.Log("Pedal brake appuyé");

        StartSpeedTransition(
            0f,
            brakingDuration,
            null
        );
    }

    private void OnBrakePedalCanceled()
    {
        Debug.Log("Pedal brake relâché");
    }

    private void OnSwitchPedalStarted()
    {
        Debug.Log("Pedal Switch Start");
        _switchPedalHeld = true;
    }

    private void OnSwitchPedalCanceled()
    {
        Debug.Log("Pedal Switch Canceled");
        _switchPedalHeld = false;
    }

    private void OnCarSteeringWheelCalled()
    {
        Debug.Log("Volant");
    }

    private void OnAutomaticTransmissionCalled()
    {
        Debug.Log("Boite auto");
    }

    // RANDOM BUTTON 1 = KLAXON (joue tant que le bouton est maintenu)
    private void OnHonkStarted()
    {
        Debug.Log("Klaxon");
        TryPlaySound(honkSoundId, loop: true);
    }

    private void OnHonkCanceled()
    {
        Debug.Log("Klaxon relâché");
        TryStopLoop();
    }

    // RANDOM BUTTON 2 = WARNINGS
    private void OnWarningsCalled()
    {
        Debug.Log("Warnings");
        TryPlaySound(warningsSoundId, loop: false);
    }

    // DISCO
    private void OnDiscoCalled()
    {
        Debug.Log("Disco");
        TryPlaySound(discoSoundId, loop: false);
    }

    private void OnPowerButtonCalled()
    {
        Power();
    }

    #endregion

    #region Sound Helpers

    // Point d'entrée unique utilisé par klaxon, warnings et disco pour jouer un son,
    // avec les mêmes vérifications de sécurité.
    private void TryPlaySound(string soundId, bool loop)
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("[PlayerController] SoundManager.Instance est null, le son ne peut pas jouer.");
            return;
        }

        if (string.IsNullOrEmpty(soundId))
        {
            Debug.LogWarning("[PlayerController] L'identifiant de son n'est pas renseigné.");
            return;
        }

        if (loop)
            SoundManager.Instance.PlayLoop(soundId);
        else
            SoundManager.Instance.Play(soundId);
    }

    private void TryStopLoop()
    {
        if (SoundManager.Instance == null)
            return;

        SoundManager.Instance.StopLoop();
    }

    #endregion

    #region Vehicle Logic

    public void SetSteeringInput(float rawAngle)
    {
        if (InputManager.Instance != null &&
            InputManager.Instance.SteeringInverted)
        {
            rawAngle = -rawAngle;
        }

        targetAngle =
            Mathf.Clamp(
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
            return;

        float targetGoal = 0f;

        switch (currentGear)
        {
            case -1:
                targetGoal = -maxReverseSpeed;
                break;

            case 0:
                targetGoal = maxGear0Speed;
                break;

            case 1:
                targetGoal = maxGear1Speed;
                break;
        }

        targetSpeed = targetGoal;

        StartSpeedTransition(
            targetGoal,
            accelerationDuration,
            accelerationCurve
        );
    }

    private void GearUp()
    {
        if (!_switchPedalHeld)
        {
            Debug.Log("[PlayerController] Appuyez sur la pédale d'embrayage avant de changer de vitesse.");
            return;
        }

        int previousGear = currentGear;

        currentGear =
            Mathf.Min(
                currentGear + 1,
                maxGear
            );

        Debug.Log(
            "GEAR UP : " +
            previousGear +
            " -> " +
            currentGear
        );

        if (currentGear != previousGear)
            UpdateSpeedLimit();
    }

    private void GearDown()
    {
        if (!_switchPedalHeld)
        {
            Debug.Log("[PlayerController] Appuyez sur la pédale d'embrayage avant de changer de vitesse.");
            return;
        }

        int previousGear = currentGear;

        currentGear =
            Mathf.Max(
                minGear,
                currentGear - 1
            );

        Debug.Log(
            "GEAR DOWN : " +
            previousGear +
            " -> " +
            currentGear
        );

        if (currentGear != previousGear)
            UpdateSpeedLimit();
    }

    private void UpdateSpeedLimit()
    {
        float maxAllowedSpeed =
            GetMaxSpeedForCurrentGear();

        if (Mathf.Abs(moveSpeed) >
            maxAllowedSpeed)
        {
            float targetGoal =
                currentGear == -1
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
            Debug.Log("La voiture est allumée");
        else
            Debug.Log("La voiture est éteinte");

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