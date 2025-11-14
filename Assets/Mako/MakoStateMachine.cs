using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using MakoObjectState = ObjectState<MakoStates>;
public enum MakoStates : int
{
    Grounded = 0,
    Airborne
}
class MakoGroundedObjectState : MakoObjectState
{
    private MakoMovement m_movement;
    private MakoInput m_input;
    private CharacterController2D m_controller;
    private SpriteRenderer m_renderer;
    private Animator m_animator;
    private float m_inputHorizontalVelocity;
    private bool m_facingRight = true;
    public override void OnStart()
    {
        m_movement = GetComponent<MakoMovement>();
        m_controller = GetComponent<CharacterController2D>();
        m_input = GetComponent<MakoInput>();
        m_animator = GetComponent<Animator>();
        m_renderer = GetComponent<SpriteRenderer>();
    }

    public override bool OnEnter(MakoStates previousState)
    {
        if (previousState == MakoStates.Airborne)
        {
            m_input.RegisterJumpAction((CallbackContext c) =>
            {
                m_input.UnregisterJumpAction();
                m_movement.JumpBegin();
            });
            return true;
        }
        
        return false;
    }

    public override void OnExit()
    {
    }

    public override MakoStates OnUpdate()
    {
        m_inputHorizontalVelocity = m_input.HorizontalMovement * m_movement.WalkingSpeed;
        if (m_input.HorizontalMovement > 0.5)
            m_facingRight = true;
        else if (m_input.HorizontalMovement < -0.5)
            m_facingRight = false;

        return MakoStates.Grounded;
    }

    public override MakoStates OnLateUpdate()
    {
        return MakoStates.Grounded;
    }

    public override MakoStates OnFixedUpdate()
    {
        if (!m_controller.Grounded)
            return MakoStates.Airborne;

        m_animator.SetFloat("GroundVelocity", Mathf.Abs(m_input.HorizontalMovement));
        m_renderer.flipX = m_facingRight;
        m_controller.GroundVelocity = m_movement.SmoothVelocity(m_controller.GroundVelocity, m_inputHorizontalVelocity, m_movement.GroundAcceleration, m_movement.GroundDecceleration, Time.fixedDeltaTime);
        return MakoStates.Grounded; 
    }
}


class MakoAirborneObjectState : MakoObjectState
{
    private CharacterController2D m_controller;
    private MakoMovement m_movement;
    private MakoInput m_input;
    private float m_inputHorizontalVelocity;
    public override void OnStart()
    {
        m_movement = GetComponent<MakoMovement>();
        m_controller = GetComponent<CharacterController2D>();
        m_input = GetComponent<MakoInput>();
    }

    public override bool OnEnter(MakoStates previousState)
    {
        if (previousState == MakoStates.Grounded)
            return true;
        return false;
    }

    public override void OnExit() { }
    public override MakoStates OnUpdate()
    {
        m_inputHorizontalVelocity = m_input.HorizontalMovement * m_movement.AirTurnSpeed;
        return MakoStates.Airborne;
    }

    public override MakoStates OnLateUpdate()
    {
        return MakoStates.Airborne;
    }
    
    public override MakoStates OnFixedUpdate()
    {
        if (m_controller.Grounded)
            return MakoStates.Grounded;

        //m_controller.GroundVelocity = m_movement.SmoothVelocity(m_controller.GroundVelocity, m_inputHorizontalVelocity, m_movement.AirAcceleration, m_movement.AirDecceleration, Time.fixedDeltaTime);
        m_controller.AirVelocity += m_movement.Gravity * Time.fixedDeltaTime;
        return MakoStates.Airborne; 
    }
}

public class MakoStateMachine : ObjectStateMachine<MakoObjectState, MakoStates>
{
    public override void InitializeStates()
    {
        m_statePile[(int) MakoStates.Grounded] = gameObject.AddComponent<MakoGroundedObjectState>();
        m_statePile[(int) MakoStates.Airborne] = gameObject.AddComponent<MakoAirborneObjectState>();
    }
}
