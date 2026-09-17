using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Variables
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

    private InputAction _acceleratorPedalInputAction;
    private InputAction _brakePedalInputAction;
    private InputAction _switchPedalInputAction;
    private InputAction _carSteeringWheelInputAction;
    private InputAction _automaticTransmissionInputAction;
    private InputAction _randomButton1InputAction;
    private InputAction _randomButton2InputAction;
    private InputAction _randomButton3InputAction;

    private InputActionMap _vehiculeMap;

    private static InputManager _instance;
    #endregion

    #region Properties
    public Vector2 AutomaticTransmission => _automaticTransmissionInputAction.ReadValue<Vector2>();
    public static InputManager Instance => _instance;
    #endregion

    #region Built-in Methods
    void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _vehiculeMap = InputSystem.actions.FindActionMap("Vehicule");

        _carSteeringWheelInputAction = InputSystem.actions.FindAction("CarSteeringWheel");
        _brakePedalInputAction = InputSystem.actions.FindAction("BrakePedal");
        _switchPedalInputAction = InputSystem.actions.FindAction("SwitchPedal");
        _acceleratorPedalInputAction = InputSystem.actions.FindAction("AcceleratorPedal");
        _automaticTransmissionInputAction = InputSystem.actions.FindAction("AutomaticTransmission");
        _randomButton1InputAction = InputSystem.actions.FindAction("RandomButton1");
        _randomButton2InputAction = InputSystem.actions.FindAction("RandomButton2");
        _randomButton3InputAction = InputSystem.actions.FindAction("RandomButton3");
    }

    private void OnEnable()
    {
        _acceleratorPedalInputAction.started += StartAcceleratorPedal;
        _acceleratorPedalInputAction.canceled += CancelAcceleratorPedal;
        _brakePedalInputAction.started += StartBrakePedal;
        _brakePedalInputAction.canceled += CancelBrakePedal;
        _switchPedalInputAction.started += StartSwitchPedal;
        _switchPedalInputAction.canceled += CancelSwitchPedal;
        _carSteeringWheelInputAction.performed += CarSteeringWheel;
        _randomButton1InputAction.performed += RandomButton1;
        _randomButton2InputAction.performed += RandomButton2;
        _randomButton3InputAction.performed += RandomButton3;
    }

    private void OnDisable()
    {
        _acceleratorPedalInputAction.started -= StartAcceleratorPedal;
        _acceleratorPedalInputAction.canceled -= CancelAcceleratorPedal;
        _brakePedalInputAction.started -= StartBrakePedal;
        _brakePedalInputAction.canceled -= CancelBrakePedal;
        _switchPedalInputAction.started -= StartSwitchPedal;
        _switchPedalInputAction.canceled -= CancelSwitchPedal;
        _carSteeringWheelInputAction.performed -= CarSteeringWheel;
        _randomButton1InputAction.performed -= RandomButton1;
        _randomButton2InputAction.performed -= RandomButton2;
        _randomButton3InputAction.performed -= RandomButton3;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    public void StartAcceleratorPedal(InputAction.CallbackContext context)
    {
        startAcceleratorPedalAction?.Invoke();
    }

    public void CancelAcceleratorPedal(InputAction.CallbackContext context)
    {
        cancelAcceleratorPedalAction?.Invoke();
    }

    public void StartBrakePedal(InputAction.CallbackContext callbackContext)
    {
        startBrakePedalAction?.Invoke();
    }

    public void CancelBrakePedal(InputAction.CallbackContext callbackContext)
    {
        cancelBrakePedalAction?.Invoke();
    }

    public void StartSwitchPedal(InputAction.CallbackContext context) 
    {
        startSwitchPedalAction?.Invoke();
    }

    public void CancelSwitchPedal(InputAction.CallbackContext context)
    {
        cancelSwitchPedalAction?.Invoke();
    }

    public void CarSteeringWheel(InputAction.CallbackContext callbackContext)
    {
        carSteeringWheelAction?.Invoke();
    }

    public void RandomButton1(InputAction.CallbackContext callbackContext) 
    {
        randomButton1Action?.Invoke();
    }

    public void RandomButton2(InputAction.CallbackContext callbackContext)
    {
        randomButton2Action?.Invoke();
    }

    public void RandomButton3(InputAction.CallbackContext callbackContext)
    {
        randomButton3Action?.Invoke();
    }
}
