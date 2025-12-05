using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthPotionUI : MonoBehaviour
{

    public PlayerStats playerStats; 

    [SerializeField] private TextMeshProUGUI potionCounterText;

     private IEnumerator Start()
    {
        yield return null;

        playerStats.OnPotionsChanged += UpdateCounterText;

        UpdateCounterText();
    }
    
    private void OnDestroy()
    {
        playerStats.OnPotionsChanged -= UpdateCounterText;
    }

    public void UpdateCounterText() {

        potionCounterText.text = "X" + playerStats.potionCount;

    }

    
}
