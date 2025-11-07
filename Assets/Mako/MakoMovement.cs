using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CapsuleCollider2D))]
public class MakoMovement : MonoBehaviour
{
    public LayerMask SolidLayerMask;
    public float WalkingSpeed = 12.0f;
    public float GroundAcceleration = 2.0f;
    public float GroundDecceleration = 4.0f;
    public float AirAcceleration = 1.5f;
    public float AirDecceleration = 3.0f;
    public float JumpingSpeed = 40.0f;
    public float Gravity = -20.0f;

    [SerializeField, SerializeAs("Current Velocity")] private Vector2 m_velocity;
    [SerializeField, SerializeAs("Vel. Target X")] private float m_horizontalVelocity;
    [SerializeField, SerializeAs("Vel. Target Y")] private float m_verticalVelocity;

    private Rigidbody2D m_rigidBody;
    private CapsuleCollider2D m_bodyCollider;
    private MakoInputActions m_inputActions;
    private InputAction m_inputHorizontal;
    private InputAction m_inputJump;
    void Awake()
    {
        m_inputActions = new MakoInputActions();
        m_inputHorizontal = m_inputActions.Movement.Horizontal;
        m_inputJump = m_inputActions.Movement.Jump;
    }

    void OnEnable()
    {
        m_inputHorizontal.Enable();
        m_inputJump.performed += onJump;
        m_inputJump.Enable();   
    }

    void OnDisable()
    {
        m_inputJump.performed -= onJump;
        m_inputJump.Disable();

        m_inputHorizontal.Disable();
    }

    private void onJump(InputAction.CallbackContext obj)
    {
        m_verticalVelocity = JumpingSpeed;
    }

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_bodyCollider = GetComponent<CapsuleCollider2D>();
    }

    void ApplyFriction(float dt)
    {
        if (Mathf.Abs(m_horizontalVelocity) < 0.015f)
        {
            m_velocity.x = Mathf.Lerp(m_velocity.x, m_horizontalVelocity, GroundDecceleration * dt);
        }
        else
        {
            m_velocity.x = Mathf.Lerp(m_velocity.x, m_horizontalVelocity, GroundAcceleration * dt);
        }

    }

    void ApplyGravity(float dt)
    {
        m_verticalVelocity += Gravity * dt;
    }
    
    bool CheckSolidHorizontal(float velocity)
    {
        var result = Physics2D.CapsuleCastAll(m_rigidBody.position, m_bodyCollider.size, m_bodyCollider.direction, 0, (velocity > 0) ? Vector2.right : Vector2.left, Mathf.Abs(velocity), SolidLayerMask);
        return result.Length != 0;
    }

    bool CheckSolidVertical(float velocity)
    {
        var result = Physics2D.BoxCastAll(m_rigidBody.position, m_bodyCollider.size, 0, (velocity > 0) ? Vector2.up : Vector2.down, Mathf.Abs(velocity), SolidLayerMask);
        return result.Length != 0;
    }

    void FixedUpdate()
    {
        ApplyFriction(Time.fixedDeltaTime);

        m_velocity.y = m_verticalVelocity * Time.fixedDeltaTime;
        ApplyGravity(Time.fixedDeltaTime);

        if (CheckSolidHorizontal(m_velocity.x))
            m_velocity.x = 0;

        if (CheckSolidVertical(m_velocity.y))
        { 
            m_velocity.y = 0;
            m_verticalVelocity = 0;
        }

        m_rigidBody.MovePosition(m_rigidBody.position + m_velocity * Time.fixedDeltaTime);
    }

    void Update()
    {
        m_horizontalVelocity = m_inputHorizontal.ReadValue<float>();
    }
}
