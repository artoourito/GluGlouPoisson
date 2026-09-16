using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Variables
    public event Action carSteeringWheelAction;
    public event Action brakePedalAction;
    public event Action switchPedalAction;
    public event Action acceleratorPedalAction;
    public event Action automaticTransmissionAction;
    public event Action randomButton1Action;
    public event Action randomButton2Action;
    public event Action randomButton3Action;

    private InputAction _carSteeringWheelInputAction;
    private InputAction _brakePedalInputAction;
    private InputAction _switchPedalInputAction;
    private InputAction _acceleratorPedalInputAction;
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
        _carSteeringWheelInputAction.performed += CarSteeringWheel;
        _brakePedalInputAction.performed += BrakePedal;
        _switchPedalInputAction.performed += SwitchPedal;
        _acceleratorPedalInputAction.performed += AcceleratorPedal;
        _randomButton1InputAction.performed += RandomButton1;
        _randomButton2InputAction.performed += RandomButton2;
        _randomButton3InputAction.performed += RandomButton3;
    }

    private void OnDisable()
    {
        _carSteeringWheelInputAction.performed -= CarSteeringWheel;
        _brakePedalInputAction.performed -= BrakePedal;
        _switchPedalInputAction.performed -= SwitchPedal;
        _acceleratorPedalInputAction.performed -= AcceleratorPedal;
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

    public void CarSteeringWheel(InputAction.CallbackContext callbackContext)
    {
        carSteeringWheelAction?.Invoke();
    }

    public void BrakePedal(InputAction.CallbackContext callbackContext)
    {
        brakePedalAction?.Invoke();
    }

    public void SwitchPedal(InputAction.CallbackContext context) 
    {
        switchPedalAction?.Invoke();
    }

    public void AcceleratorPedal(InputAction.CallbackContext context)
    {
        acceleratorPedalAction?.Invoke();
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
