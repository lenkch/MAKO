using System;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public float HorizontalDistanceMultiplier = 16.0f;
    public float HorizontalSpeed = 3.0f;
    public GameObject Mako;
    private Vector3 m_target;
    private MakoSimplifiedMovement m_makoMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_makoMovement = Mako.GetComponent<MakoSimplifiedMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        var diff = Mako.transform.position - transform.position;
        Debug.Log(diff);

        if (Mathf.Abs(diff.x) > 1.0f)
        {
            m_target = Mako.transform.position;
            m_target.x += Mathf.Sign(m_makoMovement.TargetGroundVelocity) * HorizontalDistanceMultiplier * Time.deltaTime;
        }

        var newDiff = m_target.x - transform.position.x;
        if (Mathf.Abs(newDiff) > HorizontalSpeed * Time.deltaTime)
        {
            transform.Translate(Mathf.Sign(newDiff) * HorizontalSpeed * Time.deltaTime, 0, 0);    
        }
    }

    void FixedUpdate()
    {
    }
}
