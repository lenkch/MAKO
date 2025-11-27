using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public static PlayerUIController instance {get; private set; }

    public LivesUI livesUI;
    public HealthPotionUI healthPotionUI; 

    private PlayerStats playerStats; 

    private void Awake() 
    {
        playerStats = GetComponent<PlayerStats>();
    }

    public void RestoreLivesUI(int healing)
    {
        livesUI.RestoreLives(healing);
    }
}
