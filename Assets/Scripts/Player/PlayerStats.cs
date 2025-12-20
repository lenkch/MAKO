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

    public delegate void PotionsChanged();
    public event PotionsChanged OnPotionsChanged; 

    private void Awake()
    {

        //Debug.Log("PlayerStats: Awake()");
        currentLives = maxLives - 4;
        inputActions = new MakoInputActions();
        usePotionAction = inputActions.Actions.UsePotion;

        
    }

    private void Start()
    {
       // Debug.Log("PlayerStats: Start()");
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
        Debug.Log("Player took damage: " + damage + " Current lives: " + currentLives);

        if (currentLives <= 0)
        {
            currentLives = 0;
            
            PlayerDie();
        }

        OnLivesChanged?.Invoke();

        
    }

    private void onUsePotionPress(InputAction.CallbackContext context)
    {
        // ak mame full lives tak sa nepouzike
        if (currentLives == maxLives) 
        {
            return;
        }

        if (potionCount > 0) {

            potionCount--; 
            int potionHealing = baseHealing + additiveHealingBuff; 

            RestoreLives(potionHealing);
        }

        OnPotionsChanged?.Invoke();
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

    void PlayerDie()
    {
        Debug.Log("Player died.");
    }
}
