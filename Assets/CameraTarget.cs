using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public float HorizontalDistanceMultiplier = 2.0f;
    public float HorizontalSpeed = 6.0f;
    public float VerticalSpeed = 3.0f;
    public float VerticalOffset = 1.5f;
    
    public GameObject Mako;
    private Vector3 m_target;
    private MakoSimplifiedMovement m_makoMovement;
    [SerializeField, SerializeAs("Current Vertical Offset")] private float m_currentVerticalOffset;

    public void ResetVerticalOffset()
    {
        m_currentVerticalOffset = VerticalOffset;
    }
    public void ReplaceVerticalOffset(float value)
    {
        m_currentVerticalOffset = value;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_makoMovement = Mako.GetComponent<MakoSimplifiedMovement>();
        m_currentVerticalOffset = VerticalOffset;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        m_target = Mako.transform.position;
        //if (Mathf.Abs(m_makoMovement.Velocity.x) > 0.5f && Mathf.Abs(m_makoMovement.TargetGroundVelocity) > (3.0f * m_makoMovement.WalkingSpeed / 4.0f) * Time.fixedDeltaTime)
        //    m_target.x += (m_makoMovement.LastHorizontalInputAction == HorizontalInputAction.MoveLeft ? -1 : 1) * HorizontalDistanceMultiplier;

        var diff = m_target - transform.position;
        //if (Mathf.Abs(diff.x) > HorizontalSpeed * Time.fixedDeltaTime)
        //{
        //    //var lerped = Mathf.Lerp(transform.position.x, m_target.x, HorizontalSpeed * Time.deltaTime);
        //    var dx = Mathf.Sign(diff.x) * HorizontalSpeed * Time.fixedDeltaTime;
        //    transform.Translate(dx, 0, 0);    
        //}
        transform.Translate(diff.x, 0, 0);

        // Track last stood on position.
        var dy = VerticalSpeed * Time.fixedDeltaTime;
        var ty = m_makoMovement.LastGroundY + m_currentVerticalOffset;
        if (Mathf.Abs(transform.position.y - ty) > VerticalSpeed * Time.fixedDeltaTime)
        {
            var sign = Mathf.Sign(ty - transform.position.y);
            transform.Translate(0, sign * dy, 0);
        }
    }
}
