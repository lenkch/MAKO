using UnityEngine;
using TMPro;

public class HealthPotionUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI potionCounterText;
    
    public void UpdateCounterText(int potionAmount) {

        potionCounterText.text = "X" + potionAmount;
    }
}
