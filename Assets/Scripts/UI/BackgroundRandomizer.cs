using UnityEngine;
using UnityEngine.UI;

public class BackgroundRandomizer : MonoBehaviour
{
    public Image backgroundImage;
    public Sprite[] backgrounds;

    void Start()
    {
        int randomIndex = Random.Range(0, backgrounds.Length);
        backgroundImage.sprite = backgrounds[randomIndex];
    }
}
