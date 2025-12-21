using UnityEngine;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour
{
    public int maxBossHealth;
    public int currentBossHealth;

    public UnityEvent OnHealthChanged;
    public UnityEvent OnDeath; 

    private Animator animator; 

    private bool isDead;

    void Awake()
    {
        currentBossHealth = maxBossHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        Debug.Log("Boss: took damage");

        currentBossHealth -= damage;
        OnHealthChanged?.Invoke();

        if (currentBossHealth <= 0)
        {
            currentBossHealth = 0;
            Die();
        }
        
    }

    void Die()
    {
        Debug.Log("Boss died!");
        if (isDead)
        {
            return;
        }
        isDead = true;

        animator.enabled = false;

        transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        OnDeath?.Invoke();
    }
}
