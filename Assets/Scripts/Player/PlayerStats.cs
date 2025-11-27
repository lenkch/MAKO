using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    // control actions
    private MakoInputActions inputActions;
    private InputAction usePotionAction;


    // headers sluzia na lepsiu orientaciu v inspectore
    [Header("Player Lives")]
    public int maxLives = 9;
    public int currentLives;

    [Header("Player Health Potions")]
    public int potionCount = 2;
    public int baseHealing = 1;
    public int additiveHealingBuff = 0;

    private void Awake()
    {
        inputActions = new MakoInputActions();
        usePotionAction = inputActions.Actions.UsePotion;
    }
    private void Start()
    {
        currentLives = 5; // zmenit na max lives po testovani
    }

    private void OnEnable()
    {
        usePotionAction.performed += onUsePotionPress;
        usePotionAction.Enable();
    }

    private void OnDisable()
    {
        usePotionAction.Disable();
    }

    public void TakeDamage(int damage)
    {
        currentLives -= damage;

        if (currentLives <= 0)
        {
            // you died
        }
    }

    private void onUsePotionPress(InputAction.CallbackContext context)
    {
        Debug.Log("Use Potion pressed");

        if (potionCount > 1) {

            potionCount--; 
            int potionHealing = baseHealing + additiveHealingBuff; 

            RestoreLives(potionHealing);
        }
    }

    public void RestoreLives(int healing)
    {
        if (currentLives + healing > maxLives)
        {
            currentLives = maxLives;
        }
        else 
        {
            currentLives += healing;
        }

        PlayerUIController.instance.RestoreLivesUI(healing);
    }
}
