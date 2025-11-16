using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController2D)), RequireComponent(typeof(MakoStateMachine)), RequireComponent(typeof(MakoInput))]
public class MakoMovement : MonoBehaviour
{
    public LayerMask SolidLayerMask;
    public float WalkingSpeed = 8.0f;
    public float GroundAcceleration = 15.0f;
    public float GroundDecceleration = 12.0f;
    public float AirTurnFactor = 0.7f;
    public float AirAcceleration = 1.5f;
    public float AirDecceleration = 3.0f;
    public float JumpingSpeed = 15.0f;
    public float Gravity = -20.0f;
    public float CoyoteTimeCutoff = 1.0f;

    // Values not yet multiplied by deltatime
    [SerializeField, SerializeAs("Vel. Target X")] private float m_targetHorizontalVelocity;
    [SerializeField, SerializeAs("Vel. Current X")] private float m_currentHorizontalVelocity;
    [SerializeField, SerializeAs("Jumping Velocity")] private float m_verticalVelocity;
    
    public float TargetHorizontalVelocity { get { return m_targetHorizontalVelocity; }}

    private CharacterController2D m_controller;
    private MakoStateMachine m_stateMachine;

    void Start()
    {
        m_controller = GetComponent<CharacterController2D>();
        m_stateMachine = GetComponent<MakoStateMachine>();
    }

    public void JumpBegin()
    {
        if (m_controller.TimeSinceAirborne <= CoyoteTimeCutoff)
            m_controller.AirVelocity = JumpingSpeed;
    }
    public void JumpStop()
    {
        if (m_controller.AirVelocity > 0)
            m_controller.AirVelocity /= 2;
    }

    void LateUpdate()
    {
    }

    public float SmoothVelocity(float current, float target, float acceleration, float decceleration, float dt)
    {
        if (Mathf.Abs(target) < 0.015f)
            return Mathf.Lerp(current, 0, decceleration * dt);
        else
            return Mathf.Lerp(current, target, acceleration * dt);
    }

}
