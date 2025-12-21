using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxEnemyHealth = 5;
    public int currentEnemyHealth; 

    public UnityEvent OnHealthChanged;
    public UnityEvent OnDeath; 

    private bool isDead;

    void Awake()
    {
        currentEnemyHealth = maxEnemyHealth;
        Debug.Log("Enemy Health awake, curr health: " + currentEnemyHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentEnemyHealth -= damage;
        OnHealthChanged?.Invoke();

        if (currentEnemyHealth <= 0)
        {
            currentEnemyHealth = 0;
            Die();
        }
        
    }

    void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;
        OnDeath?.Invoke();
    }
}
