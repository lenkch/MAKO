using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum HorizontalInputAction
{
    MoveRight,
    Idle,
    MoveLeft
}
public enum VerticalInputAction
{
    JumpPressed,
    Idle,
    JumpReleased
}

public class MakoSimplifiedMovement : MonoBehaviour
{
    // Constants, modifiable in editor.
    public float WalkingSpeed = 6.0f;
    public float GroundAcceleration = 10.0f;
    public float GroundDecceleration = 8.0f;
    public float AirTurnFactor = 0.7f;
    public float AirAcceleration = 1.5f;
    public float AirDecceleration = 3.0f;
    public float JumpingSpeed = 7.0f;
    public float Gravity = -20.0f;
    public float JumpReleaseExtraGravity = -20.0f;
    public float CoyoteTimeCutoff = 0.3f;
    public float OuterSkin = 0.02f;
    public float DashSpeed = 10.0f;
    public float DashTimer = 2.0f;
    public int HorizontalRayCount = 24;
    public int VerticalRayCount = 12;

    // References that must be set in editor.
    public Collider2D BodyCollider;
    public LayerMask SolidLayerMask;

    // Physics variables.
    [SerializeField, SerializeAs("Target Ground Velocity")] private float m_targetGroundVelocity;
    [SerializeField, SerializeAs("Velocity")] private Vector2 m_velocity = Vector2.zero;
    
    // Collision detection variables.
    [SerializeField, SerializeAs("Is Grounded")] private bool m_grounded = false;


    // Input variables.
    private MakoInputActions m_inputActions;
    private InputAction m_inputHorizontal;
    private InputAction m_inputJump;
    private InputAction m_inputDash;
    
    [SerializeField, SerializeAs("Horizontal Input Action")] private HorizontalInputAction m_horizontalInputAction = HorizontalInputAction.Idle;
    private HorizontalInputAction m_lastHorizontalInputAction = HorizontalInputAction.MoveRight;

    [SerializeField, SerializeAs("Vertical Input Action")] private VerticalInputAction m_verticalInputAction = VerticalInputAction.Idle;
    [SerializeField, SerializeAs("Is Dashing")] private bool m_dashing = false;
    [SerializeField, SerializeAs("Current Dash Timer")] private float m_dashTimer = 0;

    // Implied component references.
    private Rigidbody2D m_rigidBody;

    private SpriteRenderer m_renderer;
    private Animator m_animator;

    bool m_climbingSlope = false;
    float m_slopeAngle = 0.0f;


    void Awake()
    {
        m_inputActions = new MakoInputActions();
        m_inputHorizontal = m_inputActions.Movement.Horizontal;
        m_inputJump = m_inputActions.Movement.Jump;
        m_inputDash = m_inputActions.Movement.Dash;
        
    }
    void OnEnable()
    {
        m_inputHorizontal.Enable();
        m_inputJump.performed += onJumpPress;
        m_inputJump.canceled += onJumpRelease;
        m_inputJump.Enable();

        m_inputDash.performed += onDashPress;
        m_inputDash.Enable();
    }

    void OnDisable()
    {
        m_inputJump.Disable();
        m_inputJump.performed -= onJumpPress;
        m_inputJump.canceled -= onJumpRelease;
        m_inputHorizontal.Disable();
    }
    private void onDashPress(InputAction.CallbackContext context)
    {
        if (m_dashing)
            return;
        m_dashing = true;
        m_dashTimer = DashTimer;
        if (m_verticalInputAction == VerticalInputAction.JumpPressed)
            m_verticalInputAction = VerticalInputAction.Idle;
    }
    private void onJumpPress(InputAction.CallbackContext context)
    {
        if (m_verticalInputAction == VerticalInputAction.Idle)
            m_verticalInputAction = VerticalInputAction.JumpPressed;
        
        m_dashing = false;
        m_dashTimer = 0;
    }
    private void onJumpRelease(InputAction.CallbackContext context)
    {
        if (m_dashing)
            return;
        if (m_verticalInputAction == VerticalInputAction.JumpPressed)
            m_verticalInputAction = VerticalInputAction.JumpReleased;
    }
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!m_dashing)
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

            m_targetGroundVelocity = hor * WalkingSpeed;
        }
    }

    void FixedUpdate()
    {
        if (m_grounded)
        {
            // If a jump has been requested, give a burst of speed.
            if (m_verticalInputAction == VerticalInputAction.JumpPressed)
            {
                m_animator.SetBool("Jumping", true);
                m_velocity.y = JumpingSpeed;
            }
        }

        // Apply horizontal and vertical acceleration.
        m_velocity.x = Mathf.Lerp(m_velocity.x, m_targetGroundVelocity, ((m_targetGroundVelocity * Time.fixedDeltaTime < m_velocity.x) ? GroundDecceleration : GroundAcceleration) * Time.fixedDeltaTime);
        if (m_dashing)
        {
            m_dashTimer -= Time.fixedDeltaTime;
            if (m_dashTimer < 0)
            {
                m_dashTimer = 0;
                m_dashing = false;
            }
            m_velocity.x = DashSpeed * ((m_lastHorizontalInputAction == HorizontalInputAction.MoveLeft) ? -1 : 1);
        }

        m_velocity.y += (Gravity + ((m_verticalInputAction == VerticalInputAction.JumpReleased) ? JumpReleaseExtraGravity : 0.0f)) * Time.fixedDeltaTime;

        // Apply collisions and velocity.
        horizontalCollisions(m_rigidBody.position, ref m_velocity);
        verticalCollisions(m_rigidBody.position, ref m_velocity);
        cornerCollisions(m_rigidBody.position, ref m_velocity);
        // If going down and have landed, update jumping state.
        if (m_grounded)
        {
            if (m_velocity.y < 0)
                m_animator.SetBool("Jumping", false);

            if (m_verticalInputAction != VerticalInputAction.JumpPressed)
                m_verticalInputAction = VerticalInputAction.Idle;
        }

        //wallCollide(finalPosition, ref m_velocity);
        //groundCollide(finalPosition, ref m_velocity);

        // Apply new position.
        m_rigidBody.MovePosition(m_rigidBody.position + m_velocity * Time.fixedDeltaTime);

        // Update graphics variables.
        m_animator.SetBool("Dashing", m_dashing);
        m_animator.SetBool("Grounded", m_grounded);
        m_animator.SetFloat("GroundVelocity", Mathf.Abs(m_velocity.x));
        m_animator.SetFloat("AirVelocity", Math.Abs(m_velocity.y) > OuterSkin ? m_velocity.y : 0);
        m_animator.SetBool("Moving", m_horizontalInputAction != HorizontalInputAction.Idle);

        m_renderer.flipX = m_lastHorizontalInputAction == HorizontalInputAction.MoveRight;
    }   
    void cornerCollisions(Vector2 position, ref Vector2 rawVelocity)
    {
        var velocity = rawVelocity * Time.fixedDeltaTime;
        var direction = velocity.normalized;
        var distance = velocity.magnitude;
        
        var bounds = BodyCollider.bounds;
        bounds.Expand(-2 * OuterSkin);

        var cast = Physics2D.BoxCast(bounds.center, bounds.size, 0, direction, distance, SolidLayerMask);
        if (cast.collider != null)
        {
            //Debug.Break();
        }
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
        
        m_grounded = m_climbingSlope;
        for (var i = 0; i < VerticalRayCount; i++)
        {
            var origin = Vector2.Lerp(start, end, i / (float) (VerticalRayCount - 1));
            var hit = Physics2D.Raycast(origin, Vector2.up * direction, distance, SolidLayerMask);
            Debug.DrawRay(origin, Vector2.up * direction * distance, (hit.collider == null) ? Color.greenYellow : Color.red);

            if (hit.collider != null)
            {
                distance = hit.distance;
                rawVelocity.y = (direction * (hit.distance - OuterSkin)) / Time.fixedDeltaTime;
                
                m_grounded = !m_climbingSlope ? direction < 0 : m_climbingSlope;
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
                    rawVelocity.x = (xDirection * (hit.distance - OuterSkin)) / Time.fixedDeltaTime;
                    m_slopeAngle = angle;
                }
            }
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
                }
                //rawVelocity.x = Mathf.Min(Mathf.Abs(rawVelocity.x) * direction, (hit.distance - OuterSkin) * direction / Time.fixedDeltaTime);
                //distance = Mathf.Min(Mathf.Abs(rawVelocity.x) + OuterSkin, hit.distance);
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
    /*private RaycastHit2D[] m_verticalRayCasts = new RaycastHit2D[VerticalRayCount];
    private RaycastHit2D[] m_horizontalRayCasts  = new RaycastHit2D[HorizontalRayCount];
    void wallCollide(Vector2 position, ref Vector2 rawVelocity)
    {
        var bounds = BodyCollider.bounds;
        bounds.Expand(-2 * OuterSkin);

        DebugDrawBounds(BodyCollider.bounds, Color.white);
        DebugDrawBounds(bounds, Color.violet);
        
        var velocity = rawVelocity * Time.fixedDeltaTime;
        var direction = (velocity.x >= 0) ? Vector2.right : Vector2.left;

        var distance = bounds.size.x / 2.0f + Mathf.Abs(velocity.x);

        var start = new Vector2(bounds.center.x, bounds.min.y);
        var end = new Vector2(bounds.center.x, bounds.max.y);

        Debug.Log("----");
        for (int i = 0; i < HorizontalRayCount; i++)
        {
            var origin = Vector2.Lerp(start, end, (float) i / (float) (HorizontalRayCount - 1));
            var ray = Physics2D.Raycast(origin, direction, distance, SolidLayerMask);
            Debug.DrawRay(origin, direction * distance, (ray.collider != null) ? Color.red : Color.green);

            if (ray.collider == null)
                continue;

            var angle = Vector2.Angle(Vector3.left, ray.normal);

            if (angle == 0 || angle == 180)
            {
                rawVelocity.x = direction.x * (ray.fraction * distance - bounds.size.x / 2.0f) / Time.fixedDeltaTime;
                return;
            } else if (i >= HorizontalRayCount / 2)
            {
                rawVelocity.x = 0;
                return;
            }

            
            var slopeDirection = new Vector2(ray.normal.y, -ray.normal.x) * direction.x;

            var directedVelocity = slopeDirection * rawVelocity.magnitude;
            rawVelocity = directedVelocity;

            Debug.DrawLine(ray.point, ray.point + directedVelocity * 2.0f, Color.pink);
            Debug.DrawLine(ray.point + slopeDirection, ray.point + directedVelocity * 2.0f, Color.blue);

            m_horizontalRayCasts[i] = ray;
            Debug.Log($"{i}: {angle}");
        }
    }
    void verticalCollide(Vector2 position, ref Vector2 rawVelocity)
    {
        var bounds = BodyCollider.bounds;
        var feetBounds = FeetCollider.bounds;
        bounds.Expand(-2 * OuterSkin);
        feetBounds.Expand(-2 * OuterSkin);
        
        var velocity = rawVelocity * Time.fixedDeltaTime;
        var direction = (velocity.y >= 0) ? Vector2.up : Vector2.down;

        var distance = feetBounds.size.y / 2.0f + Mathf.Abs(velocity.y);

        var start = new Vector2(bounds.min.x, (velocity.y > 0) ? bounds.max.y : bounds.min.y);
        var end = new Vector2(bounds.max.x, (velocity.y > 0) ? bounds.max.y : bounds.min.y);

        bool hit = false;
        for (var i = 0; i < VerticalRayCount; i++)
        {
            var origin = Vector2.Lerp(start, end, (float) i / (float) (VerticalRayCount - 1));
            var ray = Physics2D.Raycast(origin, direction, distance, SolidLayerMask);
            Debug.DrawRay(origin, direction * distance, (ray.collider != null) ? Color.red : Color.green);

            if (ray.collider == null)
                continue;

            hit = true;
            distance = ray.distance;
        }

        if (hit)
        {
            // Nudge Mako to be flush with collision surface
            rawVelocity.y = (direction.y * distance) / Time.fixedDeltaTime;
        }
    }
    void groundCollide(Vector2 position, ref Vector2 rawVelocity)
    {
        var velocity = rawVelocity * Time.fixedDeltaTime;
        var start = new Vector2(velocity.x + FeetCollider.bounds.center.x, FeetCollider.bounds.center.y);
        var distance = Math.Abs(velocity.y);
        var direction = rawVelocity.y > 0 ? Vector2.up : Vector2.down;

        m_groundHit = Physics2D.BoxCast(start, FeetCollider.bounds.size, 0.0f, Vector2.down, distance, SolidLayerMask);
        m_grounded = m_groundHit.collider != null;

        if (m_grounded)
        {
            // If going down and have landed, nudge Mako down to be flush with the ground.
            if (rawVelocity.y < 0)
            {
                rawVelocity.y = (direction.y * m_groundHit.distance) / Time.fixedDeltaTime;
                m_animator.SetBool("Jumping", false);
                if (m_verticalInputAction != VerticalInputAction.JumpPressed)
                    m_verticalInputAction = VerticalInputAction.Idle;
            }

            // If a jump has been requested, give a burst of speed.
            if (m_verticalInputAction == VerticalInputAction.JumpPressed)
            {
                m_animator.SetBool("Jumping", true);
                rawVelocity.y = JumpingSpeed;
            }
        }
    }*/
}
