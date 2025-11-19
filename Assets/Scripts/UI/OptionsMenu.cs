using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{

    public Slider volumeSlider;
    public TMP_Text volumeValueText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetVolumeText(volumeSlider.value);
        volumeSlider.onValueChanged.AddListener(SetVolumeText);
    }

    void SetVolumeText(float value) 
    {
        volumeValueText.text = Mathf.RoundToInt(value * 100) + "%"; 
    }
}
