using UnityEngine;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public PlayerStats PlayerStats; 
    public UnityEngine.UI.Image ImageComponent;
    public float Duration = 3.0f;
    private IEnumerator Start()
    {
        yield return null;

        PlayerStats.OnDeathTriggered += BeginDeath;
    }
    void Awake()
    {
//        ImageComponent.canvasRenderer.SetAlpha(0);
        Color fixedColor = ImageComponent.color;
        fixedColor.a = 1.0f;
        ImageComponent.color = fixedColor;
        ImageComponent.CrossFadeAlpha(0f, 0f, true);
    }
    private void OnDestroy()
    {
        PlayerStats.OnDeathTriggered -= BeginDeath;
    }
    void BeginDeath()
    {
        Debug.Log("Heehoo");
        ImageComponent.CrossFadeAlpha(1.0f, Duration, false);
    }

    void Update()
    {
    }
}
