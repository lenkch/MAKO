using UnityEngine;

public class CameraVerticalOffsetZone : MonoBehaviour
{
    public float ReplacementOffset = 1.5f;
    private CameraTarget m_camTarget;
    void Start()
    {
        var cameraTarget = GameObject.Find("CameraTarget");
        m_camTarget = cameraTarget?.GetComponent<CameraTarget>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (!trigger.CompareTag("Player"))
            return;
        m_camTarget?.ReplaceVerticalOffset(ReplacementOffset);
    }

    void OnTriggerExit2D(Collider2D trigger)
    {
        if (!trigger.CompareTag("Player"))
            return;
        m_camTarget?.ResetVerticalOffset();
    }
}
