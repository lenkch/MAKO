using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class MakoInput : MonoBehaviour
{
    private MakoInputActions m_inputActions;
    private InputAction m_inputHorizontal;
    private InputAction m_inputJump;

    [SerializeField, SerializeAs("Horizontal Movement")] private float m_horizontalMovement;
    public float HorizontalMovement { get { return m_horizontalMovement; } }
    private Action<CallbackContext> m_jumpAction;

    void emptyJumpAction(CallbackContext context)
    {
        
    }
    public void RegisterJumpAction(Action<CallbackContext> action)
    {
        m_inputJump.performed -= m_jumpAction;
        m_jumpAction = action;

        if (enabled)
            m_inputJump.performed += m_jumpAction;
    }
    public void UnregisterJumpAction()
    {
        RegisterJumpAction(emptyJumpAction);
    }

    void Awake()
    {
        m_inputActions = new MakoInputActions();
        m_inputHorizontal = m_inputActions.Movement.Horizontal;
        m_inputJump = m_inputActions.Movement.Jump;
    }

    void Start()
    {
        UnregisterJumpAction();
    }

    void OnEnable()
    {
        m_inputHorizontal.Enable();
        
        m_inputJump.performed += m_jumpAction;
        m_inputJump.Enable();   
    }

    void OnDisable()
    {
        m_inputJump.performed -= m_jumpAction;
        m_inputJump.Disable();
        m_inputHorizontal.Disable();
    }

    void Update()
    {
        m_horizontalMovement = m_inputHorizontal.ReadValue<float>();
    }
}