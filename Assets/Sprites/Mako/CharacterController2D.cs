using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    public Collider2D BodyCollider;
    public Collider2D FeetCollider;
    public LayerMask SolidLayerMask;
    public float SkinWidth = 0.25f;

    [SerializeField, SerializeAs("Ground Velocity")] public float GroundVelocity;
    [SerializeField, SerializeAs("Air Velocity")] public float AirVelocity;
    [SerializeField, SerializeAs("Real Velocity")] private Vector2 m_velocity;

    [SerializeField, SerializeAs("Is Grounded")] private bool m_grounded = false;
    public bool Grounded { get { return m_grounded; } }
    
    [SerializeField, SerializeAs("Time Since Airborne")] private float m_timeSinceAirborne;
    public float TimeSinceAirborne { get { return m_timeSinceAirborne; }}

    private Rigidbody2D m_rigidBody;
    private RaycastHit2D m_groundHit;

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        doGroundCheck();

        // TODO: Slopes
        Vector2 finalPosition = m_rigidBody.position;

        // X component
        m_velocity.x = GroundVelocity * Time.fixedDeltaTime;
        finalPosition.x += m_velocity.x;

        // Y component
        if (m_grounded)
        {
            if (AirVelocity < 0)
            {
                AirVelocity = 0.0f;
                finalPosition.y -= m_groundHit.distance - FeetCollider.bounds.size.y / 2.0f;
            }
            m_timeSinceAirborne = 0.0f;
        } else
        {
            m_timeSinceAirborne += Time.fixedDeltaTime;
        }
        m_velocity.y = AirVelocity * Time.fixedDeltaTime;
        finalPosition.y += m_velocity.y;

        m_rigidBody.MovePosition(finalPosition);
    }

    void doGroundCheck()
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
