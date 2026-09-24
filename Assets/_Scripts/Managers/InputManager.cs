using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    #region Events

    public event Action startAcceleratorPedalAction;
    public event Action cancelAcceleratorPedalAction;

    public event Action startBrakePedalAction;
    public event Action cancelBrakePedalAction;

    public event Action startSwitchPedalAction;
    public event Action cancelSwitchPedalAction;

    public event Action carSteeringWheelAction;
    public event Action automaticTransmissionAction;

    public event Action randomButton1Action;
    public event Action randomButton2Action;
    public event Action randomButton3Action;
    public event Action randomButton3CanceledAction;

    public event Action powerButtonAction;

    public event Action gearUpAction;
    public event Action gearDownAction;

    #endregion


    #region Input Actions

    private InputAction _acceleratorPedalInputAction;
    private InputAction _brakePedalInputAction;
    private InputAction _switchPedalInputAction;
    private InputAction _carSteeringWheelInputAction;
    private InputAction _automaticTransmissionInputAction;
    private InputAction _randomButton1InputAction;
    private InputAction _randomButton2InputAction;
    private InputAction _randomButton3InputAction;
    private InputAction _powerButtonInputAction;

    private InputActionMap _vehiculeMap;

    #endregion


    #region Singleton

    private static InputManager _instance;

    public static InputManager Instance => _instance;

    #endregion


    #region Control Changes

    private bool _acceleratorContactSwapped;
    private bool _brakeSecondGearSwapped;
    private bool _switchPedalWarningsSwapped;
    private bool _steeringInverted;
    private bool _reverseDiscoSwapped;

    #endregion


    #region Input States

    private bool _powerWasPressed;
    private bool _randomButton2WasPressed;

    private bool _secondGearWasUsed;
    private bool _reverseWasUsed;

    #endregion


    #region Properties

    public bool SteeringInverted => _steeringInverted;

    public Vector2 AutomaticTransmission
    {
        get
        {
            if (_automaticTransmissionInputAction == null)
                return Vector2.zero;

            return _automaticTransmissionInputAction.ReadValue<Vector2>();
        }
    }

    #endregion


    #region Built-in Methods

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);

        _vehiculeMap =
            InputSystem.actions.FindActionMap("Vehicule");

        _carSteeringWheelInputAction =
            InputSystem.actions.FindAction("CarSteeringWheel");

        _brakePedalInputAction =
            InputSystem.actions.FindAction("BrakePedal");

        _switchPedalInputAction =
            InputSystem.actions.FindAction("SwitchPedal");

        _acceleratorPedalInputAction =
            InputSystem.actions.FindAction("AcceleratorPedal");

        _automaticTransmissionInputAction =
            InputSystem.actions.FindAction("AutomaticTransmission");

        _randomButton1InputAction =
            InputSystem.actions.FindAction("RandomButton1");

        _randomButton2InputAction =
            InputSystem.actions.FindAction("RandomButton2");

        _randomButton3InputAction =
            InputSystem.actions.FindAction("RandomButton3");

        _powerButtonInputAction =
            InputSystem.actions.FindAction("PowerButton");

        if (_vehiculeMap == null)
        {
            Debug.LogError(
                "[InputManager] Vehicule action map not found!"
            );
        }
        else
        {
            _vehiculeMap.Enable();
        }

        if (_automaticTransmissionInputAction == null)
        {
            Debug.LogError(
                "[InputManager] AutomaticTransmission action not found!"
            );
        }
        else
        {
            Debug.Log(
                "[InputManager] AutomaticTransmission action found."
            );
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnEnable()
    {
        if (_vehiculeMap != null)
            _vehiculeMap.Enable();

        if (_acceleratorPedalInputAction != null)
        {
            _acceleratorPedalInputAction.started += StartAcceleratorPedal;
            _acceleratorPedalInputAction.canceled += CancelAcceleratorPedal;
        }

        if (_brakePedalInputAction != null)
        {
            _brakePedalInputAction.started += StartBrakePedal;
            _brakePedalInputAction.canceled += CancelBrakePedal;
        }

        if (_switchPedalInputAction != null)
        {
            _switchPedalInputAction.started += StartSwitchPedal;
            _switchPedalInputAction.canceled += CancelSwitchPedal;
        }

        if (_carSteeringWheelInputAction != null)
            _carSteeringWheelInputAction.performed += CarSteeringWheel;

        if (_randomButton1InputAction != null)
            _randomButton1InputAction.performed += RandomButton1;

        if (_randomButton2InputAction != null)
            _randomButton2InputAction.performed += RandomButton2;

        if (_randomButton3InputAction != null)
        {
            _randomButton3InputAction.started += RandomButton3;
            _randomButton3InputAction.canceled += CancelRandomButton3;
        }

        if (_powerButtonInputAction != null)
            _powerButtonInputAction.performed += PowerButton;
    }


    private void OnDisable()
    {
        if (_acceleratorPedalInputAction != null)
        {
            _acceleratorPedalInputAction.started -= StartAcceleratorPedal;
            _acceleratorPedalInputAction.canceled -= CancelAcceleratorPedal;
        }

        if (_brakePedalInputAction != null)
        {
            _brakePedalInputAction.started -= StartBrakePedal;
            _brakePedalInputAction.canceled -= CancelBrakePedal;
        }

        if (_switchPedalInputAction != null)
        {
            _switchPedalInputAction.started -= StartSwitchPedal;
            _switchPedalInputAction.canceled -= CancelSwitchPedal;
        }

        if (_carSteeringWheelInputAction != null)
            _carSteeringWheelInputAction.performed -= CarSteeringWheel;

        if (_randomButton1InputAction != null)
            _randomButton1InputAction.performed -= RandomButton1;

        if (_randomButton2InputAction != null)
            _randomButton2InputAction.performed -= RandomButton2;

        if (_randomButton3InputAction != null)
        {
            _randomButton3InputAction.started -= RandomButton3;
            _randomButton3InputAction.canceled -= CancelRandomButton3;
        }

        if (_powerButtonInputAction != null)
            _powerButtonInputAction.performed -= PowerButton;
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void Update()
    {
        HandleSwappedInputs();
        HandleGearJoystick();
    }

    #endregion


    #region Scene Changes

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "menu":
                ResetControls();
                break;

            case "LVL 2":
                _acceleratorContactSwapped = true;
                Debug.Log("LVL 2: Accelerator <-> Contact");
                break;

            case "LVL 4":
                _brakeSecondGearSwapped = true;
                Debug.Log("LVL 4: Brake <-> Second Gear");
                break;

            case "LVL 6":
                _switchPedalWarningsSwapped = true;
                Debug.Log("LVL 6: Switch Pedal <-> Warnings");
                break;

            case "LVL 7":
                _steeringInverted = true;
                Debug.Log("LVL 7: Steering inverted");
                break;

            case "LVL 10":
                _reverseDiscoSwapped = true;
                Debug.Log("LVL 10: Reverse <-> Disco");
                break;
        }
    }


    private void ResetControls()
    {
        _acceleratorContactSwapped = false;
        _brakeSecondGearSwapped = false;
        _switchPedalWarningsSwapped = false;
        _steeringInverted = false;
        _reverseDiscoSwapped = false;

        _powerWasPressed = false;
        _randomButton2WasPressed = false;

        _secondGearWasUsed = false;
        _reverseWasUsed = false;

        Debug.Log("ALL CONTROLS RESET");
    }

    #endregion


    #region Accelerator

    private void StartAcceleratorPedal(InputAction.CallbackContext context)
    {
        if (_acceleratorContactSwapped)
        {
            powerButtonAction?.Invoke();
        }
        else
        {
            startAcceleratorPedalAction?.Invoke();
        }
    }


    private void CancelAcceleratorPedal(InputAction.CallbackContext context)
    {
        if (_acceleratorContactSwapped)
            return;

        cancelAcceleratorPedalAction?.Invoke();
    }

    #endregion


    #region Brake

    private void StartBrakePedal(InputAction.CallbackContext context)
    {
        if (_brakeSecondGearSwapped)
        {
            gearUpAction?.Invoke();
        }
        else
        {
            startBrakePedalAction?.Invoke();
        }
    }


    private void CancelBrakePedal(InputAction.CallbackContext context)
    {
        if (_brakeSecondGearSwapped)
            return;

        cancelBrakePedalAction?.Invoke();
    }

    #endregion


    #region Switch Pedal

    private void StartSwitchPedal(InputAction.CallbackContext context)
    {
        if (_switchPedalWarningsSwapped)
            return;

        startSwitchPedalAction?.Invoke();
    }


    private void CancelSwitchPedal(InputAction.CallbackContext context)
    {
        if (_switchPedalWarningsSwapped)
            return;

        cancelSwitchPedalAction?.Invoke();
    }

    #endregion


    #region Steering

    private void CarSteeringWheel(InputAction.CallbackContext context)
    {
        carSteeringWheelAction?.Invoke();
    }

    #endregion


    #region Random Buttons

    // RANDOM BUTTON 1
    // This is the DISCO button.
    private void RandomButton1(InputAction.CallbackContext context)
    {
        if (_reverseDiscoSwapped)
        {
            gearDownAction?.Invoke();
        }
        else
        {
            randomButton1Action?.Invoke();
        }
    }


    // RANDOM BUTTON 2
    // This is the CONTINUOUS SOUND button.
    private void RandomButton2(InputAction.CallbackContext context)
    {
        randomButton2Action?.Invoke();
    }


    // RANDOM BUTTON 3
    // This is the button that currently does NOTHING.
    private void RandomButton3(InputAction.CallbackContext context)
    {
        randomButton3Action?.Invoke();
    }


    private void CancelRandomButton3(InputAction.CallbackContext context)
    {
        randomButton3CanceledAction?.Invoke();
    }

    #endregion


    #region Power

    private void PowerButton(InputAction.CallbackContext context)
    {
        if (_acceleratorContactSwapped)
        {
            startAcceleratorPedalAction?.Invoke();
        }
        else
        {
            powerButtonAction?.Invoke();
        }
    }

    #endregion


    #region Swapped Button States

    private void HandleSwappedInputs()
    {
        // LVL 2
        // Accelerator <-> Contact

        if (_acceleratorContactSwapped)
        {
            bool powerPressed =
                _powerButtonInputAction != null &&
                _powerButtonInputAction.IsPressed();

            if (powerPressed && !_powerWasPressed)
            {
                startAcceleratorPedalAction?.Invoke();
            }

            if (!powerPressed && _powerWasPressed)
            {
                cancelAcceleratorPedalAction?.Invoke();
            }

            bool acceleratorPressed =
                _acceleratorPedalInputAction != null &&
                _acceleratorPedalInputAction.IsPressed();

            if (acceleratorPressed && !_powerWasPressed)
            {
                powerButtonAction?.Invoke();
            }
        }


        // LVL 6
        // Switch Pedal <-> Warnings
        //
        // Warnings stays on its intended button.
        // This section only makes that button act as Switch Pedal.

        if (_switchPedalWarningsSwapped)
        {
            bool warningsPressed =
                _randomButton1InputAction != null &&
                _randomButton1InputAction.IsPressed();

            if (warningsPressed && !_randomButton2WasPressed)
            {
                startSwitchPedalAction?.Invoke();
            }

            if (!warningsPressed && _randomButton2WasPressed)
            {
                cancelSwitchPedalAction?.Invoke();
            }
        }


        _powerWasPressed =
            _powerButtonInputAction != null &&
            _powerButtonInputAction.IsPressed();

        _randomButton2WasPressed =
            _randomButton1InputAction != null &&
            _randomButton1InputAction.IsPressed();
    }

    #endregion


    #region Gear Joystick

    private void HandleGearJoystick()
    {
        if (_automaticTransmissionInputAction == null)
            return;

        Vector2 value =
            _automaticTransmissionInputAction.ReadValue<Vector2>();

        float y = value.y;

        // TEMPORARY DEBUG
        // This will show exactly what Unity receives.
        if (Mathf.Abs(y) > 0.01f)
        {
            Debug.Log(
                "GEAR JOYSTICK INPUT: " +
                value
            );
        }


        // =========================
        // SECOND GEAR
        // =========================

        if (y > 0.8f)
        {
            if (!_secondGearWasUsed)
            {
                Debug.Log("GEAR JOYSTICK: UP");

                if (_brakeSecondGearSwapped)
                {
                    startBrakePedalAction?.Invoke();
                }
                else
                {
                    gearUpAction?.Invoke();
                }

                _secondGearWasUsed = true;
            }
        }
        else if (y < 0.3f)
        {
            if (_secondGearWasUsed)
            {
                if (_brakeSecondGearSwapped)
                {
                    cancelBrakePedalAction?.Invoke();
                }

                _secondGearWasUsed = false;
            }
        }


        // =========================
        // REVERSE
        // =========================

        if (y < -0.8f)
        {
            if (!_reverseWasUsed)
            {
                Debug.Log("GEAR JOYSTICK: DOWN");

                if (_reverseDiscoSwapped)
                {
                    // LVL 10:
                    // Reverse <-> Disco
                    randomButton1Action?.Invoke();
                }
                else
                {
                    gearDownAction?.Invoke();
                }

                _reverseWasUsed = true;
            }
        }
        else if (y > -0.3f)
        {
            _reverseWasUsed = false;
        }
    }

    #endregion
}