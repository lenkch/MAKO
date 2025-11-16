using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // References that must be set in editor.
    public Collider2D BodyCollider;
    public Collider2D FeetCollider;
    public LayerMask SolidLayerMask;

    // Physics variables.
    private float m_targetHorizontalSpeed;
    private float m_horizontalSpeed;
    private float m_verticalSpeed;
    
    // Collision detection variables.
    [SerializeField, SerializeAs("Is Grounded")] private bool m_grounded = false;
    private RaycastHit2D m_groundHit;


    // Input variables.
    private MakoInputActions m_inputActions;
    private InputAction m_inputHorizontal;
    private InputAction m_inputJump;
    [SerializeField, SerializeAs("Movement Action")] private int m_movementAction; // 1 if going right, 0 if nothing, -1 if going left.
    private int m_lastMovementAction = 1;
    [SerializeField, SerializeAs("Jump Action")] private int m_jumpAction; // 1 if pressed, 0 if no, -1 if released.

    // Implied component references.
    private Rigidbody2D m_rigidBody;

    private SpriteRenderer m_renderer;
    private Animator m_animator;


    void Awake()
    {
        m_inputActions = new MakoInputActions();
        m_inputHorizontal = m_inputActions.Movement.Horizontal;
        m_inputJump = m_inputActions.Movement.Jump;
        
    }
    void OnEnable()
    {
        m_inputHorizontal.Enable();
        m_inputJump.performed += onJumpPress;
        m_inputJump.canceled += onJumpRelease;
        m_inputJump.Enable();
    }

    void OnDisable()
    {
        m_inputJump.Disable();
        m_inputJump.performed -= onJumpPress;
        m_inputJump.canceled -= onJumpRelease;
        m_inputHorizontal.Disable();
    }
    private void onJumpPress(InputAction.CallbackContext context)
    {
        if (m_jumpAction == 0)
            m_jumpAction = 1;
    }
    private void onJumpRelease(InputAction.CallbackContext context)
    {
        if (m_jumpAction == 1)
            m_jumpAction = -1;
    }
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        m_movementAction = (int)m_inputHorizontal.ReadValue<float>();
        if (m_movementAction != 0)
            m_lastMovementAction = m_movementAction;
        //m_jumpAction = (Input.GetKeyDown(KeyCode.Space) ? 1 : 0) - (Input.GetKeyUp(KeyCode.Space) ? 1 : 0);
        m_targetHorizontalSpeed = m_movementAction * WalkingSpeed;
    }
    void FixedUpdate()
    {
        Vector2 finalPosition = m_rigidBody.position;

        // Apply X velocity.
        m_horizontalSpeed = Mathf.Lerp(m_horizontalSpeed, m_targetHorizontalSpeed, (m_movementAction == 0 ? GroundDecceleration : GroundAcceleration) * Time.fixedDeltaTime);
        doWallCollision(m_horizontalSpeed * Time.fixedDeltaTime);
        finalPosition.x += m_horizontalSpeed * Time.fixedDeltaTime;

        // Apply Y velocity.
        doGroundedCheck();
        if (m_grounded)
        {
            // If going down and have landed, nudge Mako down to be flush with the ground.
            if (m_verticalSpeed < 0)
            {
                m_verticalSpeed = 0.0f;
                finalPosition.y -= m_groundHit.distance - FeetCollider.bounds.size.y / 2.0f;
                m_jumpAction = 0;
            }

            // If a jump has been requested, give a burst of speed.
            if (m_jumpAction > 0)
            {
                m_verticalSpeed = JumpingSpeed;
            }
        }
        else
        {
            m_verticalSpeed += (Gravity + ((m_jumpAction < 0) ? JumpReleaseExtraGravity : 0.0f)) * Time.fixedDeltaTime;
        }
        finalPosition.y += m_verticalSpeed * Time.fixedDeltaTime;

        m_rigidBody.MovePosition(finalPosition);

        // Apply graphics variables.
        m_animator.SetFloat("GroundVelocity", Mathf.Abs(m_horizontalSpeed));
        m_renderer.flipX = m_lastMovementAction > 0;
    }

    void doWallCollision(float speed)
    {
        var N = 10;
        var goingLeft = speed < 0;

        var start = new Vector2(goingLeft ? BodyCollider.bounds.min.x : BodyCollider.bounds.max.x, BodyCollider.bounds.min.y);
        var end = new Vector2(start.x, BodyCollider.bounds.max.y);
        var diff = (end - start) / N;

        for (int i = N - 1; i >= 0; i--)
        {
            var point = start + diff * i;

            var cast = Physics2D.Raycast(point, Vector2.right, speed, SolidLayerMask);
            
            Color debugColor = (cast.collider == null) ? Color.green : Color.red;
            Debug.DrawRay(point, Vector2.right * speed, debugColor);

            if (cast.collider == null)
                continue;
            
            if (point.y > BodyCollider.bounds.center.y)
            {
                m_horizontalSpeed = 0.0f; 
                return;
            }
        }
    }
    
    void doGroundedCheck()
    {
        Vector2 castPosition = new Vector2(FeetCollider.bounds.center.x, FeetCollider.bounds.max.y);
        Vector2 castSize = FeetCollider.bounds.size;

        m_groundHit = Physics2D.BoxCast(castPosition, castSize, 0.0f, Vector2.down, FeetCollider.bounds.size.y, SolidLayerMask);
        m_grounded = m_groundHit.collider != null;

        Color debugColor = m_grounded ? Color.red : Color.green;
        Debug.DrawRay(castPosition - new Vector2(castSize.x / 2.0f, 0), Vector2.down * castSize.y, debugColor);
        Debug.DrawRay(castPosition + new Vector2(castSize.x / 2.0f, 0), Vector2.down * castSize.y, debugColor);
        Debug.DrawRay(castPosition - new Vector2(castSize.x / 2.0f, castSize.y), Vector2.right * castSize.x, debugColor);
    }
    
}
