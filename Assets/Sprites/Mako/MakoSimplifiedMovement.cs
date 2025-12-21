using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public enum HorizontalInputAction
{
    MoveLeft,
    Idle,
    MoveRight
}
public enum VerticalInputAction
{
    JumpPressed,
    Idle,
    JumpReleased
}
public enum MakoState
{
    RunAround,
    Attack,
    WallSlide,
    Dashing
}
public enum MakoWallSlideState
{
    Inactive,
    WallHang,
    WallSlide
}
struct MakoGraphicsPacket
{
    public bool Attacking;
    public bool Dashing;
    public bool Grounded;
    public bool Moving;
    public float GroundVelocity;
    public float AirVelocity;
    public bool FacingRight;
    public bool Jumping;
    public MakoWallSlideState WallSlideState;
}
public partial class MakoSimplifiedMovement : MonoBehaviour
{
    // Constants, modifiable in editor.
    public float WalkingSpeed = 300.0f;
    public float GroundAcceleration = 10.0f;
    public float GroundDecceleration = 8.0f;
    public float JumpingSpeed = 400.0f;
    public float Gravity = -20.0f;
    public float MaxDownwardsVelocity = -1000.0f;

    public float JumpReleaseExtraGravity = -20.0f;
    public float CoyoteTimer = 0.5f;
    public float OuterSkin = 0.025f;
    public float DashSpeed = 10.0f;
    public float DashTimer = 0.25f;
    public int HorizontalRayCount = 24;
    public int VerticalRayCount = 12;
    public float AttackCooldown = 0.5f;
    public float AttackVelocityCap = 1.0f;
    public float WallStuckTimer = 1.0f;
    public Vector2 WallJumpSpeed = new Vector2(600.0f, 400.0f);

    // References that must be set in editor.
    public Collider2D BodyCollider;
    public LayerMask SolidLayerMask;

    // Physics variables.
    [SerializeField, SerializeAs("Target Ground Velocity")] private float m_targetGroundVelocity;
    [SerializeField, SerializeAs("Current Velocity")] private Vector2 m_velocity = Vector2.zero;
    
    // Collision detection variables.
    [SerializeField, SerializeAs("Is Grounded?")] private bool m_grounded = false;
    [SerializeField, SerializeAs("Current Coyote Timer")] private float m_coyoteTimer = 0;

    // Input variables.
    private MakoInputActions m_inputActions;
    private InputAction m_inputHorizontal;
    private InputAction m_inputJump;
    private InputAction m_inputDash;
    private InputAction m_inputAttack;
    
    
    [SerializeField, SerializeAs("Horizontal InputAction")] private HorizontalInputAction m_horizontalInputAction = HorizontalInputAction.Idle;
    private HorizontalInputAction m_lastHorizontalInputAction = HorizontalInputAction.MoveRight;


    [SerializeField, SerializeAs("Vertical Input Action")] private VerticalInputAction m_verticalInputAction = VerticalInputAction.Idle;
    [SerializeField, SerializeAs("Current Dash Timer")] private float m_dashTimer = 0;
    [SerializeField, SerializeAs("Current Attack Cooldown")] private float m_attackCooldown = 0;

    // Properties.
    public HorizontalInputAction HorizontalInputAction { get { return m_horizontalInputAction; } }
    public HorizontalInputAction LastHorizontalInputAction { get { return m_lastHorizontalInputAction; } }
    public VerticalInputAction VerticalInputAction { get { return m_verticalInputAction; } }
    public bool IsGrounded { get { return m_grounded; }}
    public Vector2 Velocity { get { return m_velocity; }}
    public float TargetGroundVelocity { get { return m_targetGroundVelocity; }}
    public MakoState State { get { return m_state; } }
    public float LastGroundY { get { return m_lastGroundY; }}

    // Implied component references.
    private Rigidbody2D m_rigidBody;

    private SpriteRenderer m_renderer;
    private Animator m_animator;

    private bool m_climbingSlope = false;
    private float m_slopeAngle = 0.0f;
    [SerializeField, SerializeAs("Last Ground Y")] private float m_lastGroundY = 0.0f;

    [SerializeField, SerializeAs("Mako State")] private MakoState m_state;

    private MakoGraphicsPacket m_graphicsPacket;

    void Awake()
    {
        m_inputActions = new MakoInputActions();
        m_inputHorizontal = m_inputActions.Movement.Horizontal;
        m_inputJump = m_inputActions.Movement.Jump;
        m_inputDash = m_inputActions.Movement.Dash;
        m_inputAttack = m_inputActions.Movement.Attack;
    }
    
    private void enableState(MakoState state)
    {
        switch (state)
        {
            case MakoState.RunAround: enableState_RunAround(); break;
            case MakoState.Dashing: enableState_Dashing(); break;
            case MakoState.Attack: enableState_Attacking(); break;
            case MakoState.WallSlide: enableState_WallSlide(); break;
            default: Debug.LogError("Unimplemented state!"); break;
        }
    }
    private void disableState(MakoState state)
    {
        switch (state)
        {
            case MakoState.RunAround: disableState_RunAround(); break;
            case MakoState.Dashing: disableState_Dashing(); break;
            case MakoState.Attack: disableState_Attacking(); break;
            case MakoState.WallSlide: disableState_WallSlide(); break;
            default: Debug.LogError("Unimplemented state!"); break;
        }
    }
    private void initState(MakoState state)
    {
        switch (state)
        {
            case MakoState.RunAround: initState_RunAround(); break;
            case MakoState.Dashing: initState_Dashing(); break;
            case MakoState.Attack: initState_Attacking(); break;
            case MakoState.WallSlide: initState_WallSlide(); break;
            default: Debug.LogError("Unimplemented state!"); break;
        }
    }
    private void deinitState(MakoState state)
    {
        switch (state)
        {
            case MakoState.RunAround: deinitState_RunAround(); break;
            case MakoState.Dashing: deinitState_Dashing(); break;
            case MakoState.Attack: deinitState_Attacking(); break;
            case MakoState.WallSlide: deinitState_WallSlide(); break;
            default: Debug.LogError("Unimplemented state!"); break;
        }
    }
    private void updateState(MakoState state)
    {
        switch (state)
        {
            case MakoState.RunAround: updateState_RunAround(); break;
            case MakoState.Dashing: updateState_Dashing(); break;
            case MakoState.Attack: updateState_Attacking(); break;
            case MakoState.WallSlide: updateState_WallSlide(); break;
            default: Debug.LogError("Unimplemented state!"); break;
        }
    }
    private void fixedUpdateState(MakoState state)
    {
        switch (state)
        {
            case MakoState.RunAround: fixedUpdateState_RunAround(); break;
            case MakoState.Dashing: fixedUpdateState_Dashing(); break;
            case MakoState.Attack: fixedUpdateState_Attacking(); break;
            case MakoState.WallSlide: fixedUpdateState_WallSlide(); break;
            default: Debug.LogError("Unimplemented state!"); break;
        }
    }
    private void switchState(MakoState state)
    {
        disableState(m_state);
        deinitState(m_state);
        m_state = state;
        enableState(m_state);
        initState(m_state);
    }

    void OnEnable()
    {
        enableState(m_state);

        m_inputHorizontal.Enable();        
        m_inputJump.Enable();
        m_inputDash.Enable();
        m_inputAttack.Enable();
    }

    void OnDisable()
    {
        m_inputDash.Disable();
        m_inputAttack.Disable();
        m_inputJump.Disable();
        m_inputHorizontal.Disable();

        disableState(m_state);
    }
    
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        updateState(m_state);
    }

    void FixedUpdate()
    {
        fixedUpdateState(m_state);
        m_animator.SetBool("Attacking", m_graphicsPacket.Attacking);
        m_animator.SetBool("Dashing", m_graphicsPacket.Dashing);
        m_animator.SetBool("Grounded", m_graphicsPacket.Grounded);
        m_animator.SetFloat("GroundVelocity", m_graphicsPacket.GroundVelocity);
        m_animator.SetFloat("AirVelocity", m_graphicsPacket.AirVelocity);
        m_animator.SetBool("Moving", m_graphicsPacket.Moving);
        m_animator.SetBool("Jumping", m_graphicsPacket.Jumping);

        m_animator.SetBool("WallSlideActive", m_graphicsPacket.WallSlideState != MakoWallSlideState.Inactive);
        m_animator.SetFloat("WallSlide", m_graphicsPacket.WallSlideState == MakoWallSlideState.WallSlide ? 1.0f : 0.0f);
        m_renderer.flipX = m_graphicsPacket.FacingRight;
    }

    void verticalCollisions(Vector2 position, ref Vector2 rawVelocity)
    {
        var velocity = rawVelocity * Time.fixedDeltaTime;
        var direction = Mathf.Sign(velocity.y);
        var distance = Mathf.Abs(velocity.y) + (direction < 0 ? Mathf.Abs(velocity.x) : 0) + OuterSkin;
        
        var bounds = BodyCollider.bounds;
        bounds.Expand(-2 * OuterSkin);
        
        // Collision detection.
        var start = new Vector2(bounds.min.x, direction < 0 ? bounds.min.y : bounds.max.y);
        var end = new Vector2(bounds.max.x, start.y);
        
        bool grounded = m_climbingSlope;
        for (var i = 0; i < VerticalRayCount; i++)
        {
            var origin = Vector2.Lerp(start, end, i / (float) (VerticalRayCount - 1));
            var hit = Physics2D.Raycast(origin, Vector2.up * direction, distance, SolidLayerMask);
            Debug.DrawRay(origin, Vector2.up * direction * distance, (hit.collider == null) ? Color.greenYellow : Color.red);

            if (hit.collider != null)
            {
                distance = hit.distance;
                rawVelocity.y = direction * (hit.distance - OuterSkin) / Time.fixedDeltaTime;
                m_lastGroundY = hit.point.y;
                grounded = !m_climbingSlope ? direction < 0 : m_climbingSlope;
            }
        }

        if (m_climbingSlope)
        {
            var xDirection = Mathf.Sign(velocity.x);
            var origin = new Vector2(xDirection < 0 ? bounds.min.x : bounds.max.x, bounds.max.x) + Vector2.up * direction;
            distance = Mathf.Abs(velocity.x) + OuterSkin;

            var hit = Physics2D.Raycast(origin, Vector2.right * xDirection, distance, SolidLayerMask);
            if (hit.collider != null)
            {
                var angle = Vector2.Angle(hit.normal, Vector2.up);
                if (angle != m_slopeAngle)
                {
                    rawVelocity.x = xDirection * (hit.distance - OuterSkin) / Time.fixedDeltaTime;
                    m_slopeAngle = angle;
                }
            }
        }

        if (m_grounded && !grounded)
        {
            if (direction == Vector2.down.y && m_coyoteTimer <= 0)
            {
                m_coyoteTimer = CoyoteTimer;
            } else
            {
                m_grounded = false;
            }
        }  else if (!m_grounded && grounded)
        {
            // Update immediately.
            m_grounded = true;
            
        }

    }

    void horizontalCollisions(Vector2 position, ref Vector2 rawVelocity)
    {
        var velocity = rawVelocity * Time.fixedDeltaTime;
        var direction = Mathf.Sign(velocity.x);
        var distance = Mathf.Abs(velocity.x) + OuterSkin;
        
        var bounds = BodyCollider.bounds;
        bounds.Expand(-2 * OuterSkin);
        DebugDrawBounds(bounds, Color.purple);

        var start = new Vector2(direction < 0 ? bounds.min.x : bounds.max.x, bounds.min.y + Mathf.Min(0, velocity.y));
        var end = new Vector2(start.x, bounds.max.y + Mathf.Max(0, velocity.y));
        m_climbingSlope = false;

        if (m_state == MakoState.RunAround)
            m_wallDirection = 0;
        for (var i = 0; i < HorizontalRayCount; i++)
        {
            var origin = Vector2.Lerp(start, end, i / (float) (HorizontalRayCount - 1));
            var hit = Physics2D.Raycast(origin, Vector2.right * direction, distance, SolidLayerMask);
            Debug.DrawRay(origin, Vector2.right * direction * distance, (hit.collider == null) ? Color.greenYellow : Color.red);

            if (hit.collider != null)
            {
                var angle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && angle < 70)
                {
                    var trueDistance = distance - OuterSkin;
                    rawVelocity.x = (Mathf.Cos(angle * Mathf.Deg2Rad) * trueDistance * direction) / Time.fixedDeltaTime;
                    rawVelocity.y = (Mathf.Sin(angle * Mathf.Deg2Rad) * trueDistance) / Time.fixedDeltaTime;
                    m_climbingSlope = true;
                    m_slopeAngle = angle;
                } else {
                    rawVelocity.x = (hit.distance - OuterSkin) * direction / Time.fixedDeltaTime;
                    distance = hit.distance;

                    if (!m_climbingSlope && m_state == MakoState.RunAround)
                        m_wallDirection = (int) direction;
                }
            }
        }
    }

    void DebugDrawBounds(Bounds bounds, Color color)
    {
        var start = bounds.center;
        var size = bounds.max - bounds.min;
        DebugDrawRectangle(start, size, color);
    }
    void DebugDrawRectangle(Vector2 start, Vector2 size, Color color)
    {
        var min = start - size / 2.0f;
        var max = start + size / 2.0f;
        Debug.DrawLine(new Vector2(min.x, min.y), new Vector2(max.x, min.y), color);
        Debug.DrawLine(new Vector2(min.x, max.y), new Vector2(max.x, max.y), color);
        Debug.DrawLine(new Vector2(min.x, min.y), new Vector2(min.x, max.y), color);
        Debug.DrawLine(new Vector2(max.x, min.y), new Vector2(max.x, max.y), color);
    }
}

// RunAround state.
public partial class MakoSimplifiedMovement
{
    private void onAttackPress_RunAround(InputAction.CallbackContext context)
    {
        if (!m_grounded || m_attackCooldown > 0)
            return;
        switchState(MakoState.Attack);
    }
    private void onDashPress_RunAround(InputAction.CallbackContext context)
    {
        if (!m_grounded)
            return;
        
        if (m_verticalInputAction == VerticalInputAction.JumpPressed)
            m_verticalInputAction = VerticalInputAction.Idle;
        switchState(MakoState.Dashing);
    }
    private void onJumpPress_RunAround(InputAction.CallbackContext context)
    {
        if (m_verticalInputAction == VerticalInputAction.Idle)
            m_verticalInputAction = VerticalInputAction.JumpPressed;
    }
    private void onJumpRelease_RunAround(InputAction.CallbackContext context)
    {
        if (m_verticalInputAction == VerticalInputAction.JumpPressed)
            m_verticalInputAction = VerticalInputAction.JumpReleased;
    }
    void enableState_RunAround()
    {
        m_inputJump.performed += onJumpPress_RunAround;
        m_inputJump.canceled += onJumpRelease_RunAround;
        m_inputDash.performed += onDashPress_RunAround;
        m_inputAttack.performed += onAttackPress_RunAround;

    }
    void disableState_RunAround()
    {
        m_inputDash.performed -= onDashPress_RunAround;
        m_inputAttack.performed -= onAttackPress_RunAround;
        m_inputJump.performed -= onJumpPress_RunAround;
        m_inputJump.canceled -= onJumpRelease_RunAround;
    }
    void initState_RunAround()
    {
        
    }
    void deinitState_RunAround()
    {
        m_targetGroundVelocity = 0.0f;
    }
    void updateState_RunAround()
    {
        int hor = (int)m_inputHorizontal.ReadValue<float>();
        switch (hor)
        {
            case 1: m_horizontalInputAction = HorizontalInputAction.MoveRight; break;
            case -1: m_horizontalInputAction = HorizontalInputAction.MoveLeft; break;
            default: m_horizontalInputAction = HorizontalInputAction.Idle; break;
        }
        if (m_horizontalInputAction != HorizontalInputAction.Idle)
            m_lastHorizontalInputAction = m_horizontalInputAction;

        if (hor != 0 && hor == m_wallDirection && !m_grounded)
        {
            switchState(MakoState.WallSlide);
        }
        m_targetGroundVelocity = hor * WalkingSpeed;

        if (m_attackCooldown > 0)
        {
            m_attackCooldown -= Time.deltaTime;
        }
        
        if (m_coyoteTimer > 0)
        {
            m_coyoteTimer -= Time.deltaTime;
            if (m_coyoteTimer < 0)
            {
                m_grounded = false;
                m_coyoteTimer = 0;
            }
        }
        
    }
    void fixedUpdateState_RunAround()
    {
        // If a jump has been requested, give a burst of speed.
        if (m_verticalInputAction == VerticalInputAction.JumpPressed)
        {
            if (m_grounded)
            {
                m_graphicsPacket.Jumping = true;
                m_velocity.y = JumpingSpeed * Time.fixedDeltaTime;
            }
        }

        // Apply horizontal and vertical acceleration.
        m_velocity.x = Mathf.Lerp(m_velocity.x, m_targetGroundVelocity * Time.fixedDeltaTime, ((m_targetGroundVelocity * Time.fixedDeltaTime < m_velocity.x) ? GroundDecceleration : GroundAcceleration) * Time.fixedDeltaTime);
        m_velocity.y += (Gravity + ((m_verticalInputAction == VerticalInputAction.JumpReleased) ? JumpReleaseExtraGravity : 0.0f)) * Time.fixedDeltaTime;
        m_velocity.y = Mathf.Max(m_velocity.y, MaxDownwardsVelocity * Time.fixedDeltaTime);

        // Apply collisions and velocity.
        horizontalCollisions(m_rigidBody.position, ref m_velocity);
        verticalCollisions(m_rigidBody.position, ref m_velocity);
        // If going down and have landed, update jumping state.
        if (m_grounded)
        {
            m_graphicsPacket.Jumping = false;

            if (m_verticalInputAction != VerticalInputAction.JumpPressed)
                m_verticalInputAction = VerticalInputAction.Idle;
        }

        // Apply new position.
        m_rigidBody.MovePosition(m_rigidBody.position + m_velocity * Time.fixedDeltaTime);

        // Update graphics variables.
        m_graphicsPacket.Grounded = m_grounded;
        m_graphicsPacket.GroundVelocity = Mathf.Abs(m_velocity.x);
        m_graphicsPacket.AirVelocity = Math.Abs(m_velocity.y) > OuterSkin ? m_velocity.y : 0;
        m_graphicsPacket.Moving = m_horizontalInputAction != HorizontalInputAction.Idle;
        m_graphicsPacket.FacingRight = m_lastHorizontalInputAction == HorizontalInputAction.MoveRight;
    }
}

// Dashing state
public partial class MakoSimplifiedMovement
{
    void enableState_Dashing()
    {
        
    }
    void disableState_Dashing()
    {
        
    }
    void initState_Dashing()
    {
        m_graphicsPacket.Dashing = true;
        m_dashTimer = DashTimer;
    }
    void deinitState_Dashing()
    {
        m_graphicsPacket.Dashing = false;
    }
    void updateState_Dashing()
    {
        
    }
    void fixedUpdateState_Dashing()
    {
        m_dashTimer -= Time.fixedDeltaTime;
        if (m_dashTimer < 0)
        {
            m_dashTimer = 0;
            switchState(MakoState.RunAround);
        }
        m_velocity.x = DashSpeed * ((m_lastHorizontalInputAction == HorizontalInputAction.MoveLeft) ? -1 : 1);

        // Apply collisions and velocity.
        horizontalCollisions(m_rigidBody.position, ref m_velocity);
        verticalCollisions(m_rigidBody.position, ref m_velocity);
        
        // Apply new position.
        m_rigidBody.MovePosition(m_rigidBody.position + m_velocity * Time.fixedDeltaTime);
    }
}

// Attack state.
public partial class MakoSimplifiedMovement
{
    public void OnAttackFinish()
    {
        m_attackCooldown = AttackCooldown;
        switchState(MakoState.RunAround);
    }
    void enableState_Attacking()
    {
    }
    void disableState_Attacking()
    {
        
    }
    void initState_Attacking()
    {
        m_graphicsPacket.Attacking = true;
    }
    void deinitState_Attacking()
    {
        m_graphicsPacket.Attacking = false;
    }
    void updateState_Attacking()
    {
        
    }
    void fixedUpdateState_Attacking()
    {
        m_velocity.x = Mathf.Clamp(m_velocity.x, -AttackVelocityCap, AttackVelocityCap);
        
        // Apply collisions and velocity.
        horizontalCollisions(m_rigidBody.position, ref m_velocity);
        verticalCollisions(m_rigidBody.position, ref m_velocity);
        
        // Apply new position.
        m_rigidBody.MovePosition(m_rigidBody.position + m_velocity * Time.fixedDeltaTime);
    }
}


// Attack state.
public partial class MakoSimplifiedMovement
{
    [SerializeField, SerializeAs("Wall Stuck Direction")] private int m_wallDirection = 0;
    [SerializeField, SerializeAs("Wall Stuck Timer")] private float m_wallStuckTimer = 0;
    private MakoWallSlideState m_wallSlideState = MakoWallSlideState.Inactive;

    private void onJumpPress_WallSlide(InputAction.CallbackContext context)
    {
        m_velocity = WallJumpSpeed * Time.fixedDeltaTime;
        m_velocity.x *= -m_wallDirection;
        m_lastGroundY = transform.position.y;
        switchState(MakoState.RunAround);
    }
    void enableState_WallSlide()
    {
        m_inputJump.performed += onJumpPress_WallSlide;
    }
    void disableState_WallSlide()
    {
        m_inputJump.performed -= onJumpPress_WallSlide;
    }
    void initState_WallSlide()
    {
        m_wallSlideState = MakoWallSlideState.WallHang;
        m_wallStuckTimer = WallStuckTimer;
        m_velocity.x = 0;
        m_velocity.y = 0;
    }
    void deinitState_WallSlide()
    {
        m_wallSlideState = MakoWallSlideState.Inactive;
        m_graphicsPacket.WallSlideState = MakoWallSlideState.Inactive;
    }
    void updateState_WallSlide()
    {
        m_wallStuckTimer -= Time.deltaTime;
        if (m_wallStuckTimer < 0)
        {
            m_wallSlideState = MakoWallSlideState.WallSlide;
        }

        int hor = (int)m_inputHorizontal.ReadValue<float>();
        if (hor != 0 && hor != m_wallDirection)
            switchState(MakoState.RunAround);
    }
    void wallSlideCollision(Vector2 position)
    {
        var direction = (float) m_wallDirection;
        var distance = OuterSkin;
        
        var bounds = BodyCollider.bounds;
        bounds.Expand(-2 * OuterSkin);

        Debug.DrawRay(bounds.center, new Vector2(direction, 0) * distance, Color.aquamarine);
        var result = Physics2D.BoxCast(bounds.center, bounds.size, 0.0f, new Vector2(direction, 0), distance, SolidLayerMask);
        if (!result)
            m_wallDirection = 0;
    }
    void fixedUpdateState_WallSlide()
    {
        if (m_wallSlideState == MakoWallSlideState.WallSlide)
        {
            m_velocity.y = Gravity * Time.fixedDeltaTime;
        }

        // Apply collisions and velocity.
        horizontalCollisions(m_rigidBody.position, ref m_velocity);
        verticalCollisions(m_rigidBody.position, ref m_velocity);
        wallSlideCollision(m_rigidBody.position);

        // Apply new position.
        m_rigidBody.MovePosition(m_rigidBody.position + m_velocity * Time.fixedDeltaTime);
        m_graphicsPacket.WallSlideState = m_wallSlideState;

        if (m_grounded || m_wallDirection == 0)
        {
            switchState(MakoState.RunAround);
        }
    }
}