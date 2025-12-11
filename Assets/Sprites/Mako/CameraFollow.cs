using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform FollowTransform;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        transform.position = FollowTransform.position - new Vector3(0, 0, 10);
    }
}
