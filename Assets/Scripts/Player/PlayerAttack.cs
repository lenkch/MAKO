using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int playerDamage = 1;

    void Awake()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        EnemyHealth enemyHealth = trigger.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
        {
            Debug.Log("Triggered ClawHitbox");
            enemyHealth.TakeDamage(playerDamage);
        }
    }

}
