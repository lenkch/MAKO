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

    public delegate void LivesChanged();
    public event LivesChanged OnLivesChanged;

    [Header("Player Health Potions")]
    public int potionCount = 2;
    public int baseHealing = 1;
    public int additiveHealingBuff = 0;

    private void Awake()
    {

        Debug.Log("PlayerStats: Awake()");
        currentLives = maxLives - 4;
        inputActions = new MakoInputActions();
        usePotionAction = inputActions.Actions.UsePotion;

        
    }

    private void Start()
    {
        Debug.Log("PlayerStats: Start()");
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
            currentLives = 0;
            // you died
            // death animation and menu screen or whatever
            // asi pridat animator neskor
        }

        OnLivesChanged?.Invoke();
    }

    private void onUsePotionPress(InputAction.CallbackContext context)
    {
        Debug.Log("Use Potion pressed");

        if (potionCount > 0) {

            potionCount--; 
            int potionHealing = baseHealing + additiveHealingBuff; 

            RestoreLives(potionHealing);
        }
    }

    public void RestoreLives(int healing)
    {
        currentLives += healing;
        if (currentLives > maxLives)
        {
            currentLives = maxLives;
        }

        OnLivesChanged?.Invoke();
    }
}
