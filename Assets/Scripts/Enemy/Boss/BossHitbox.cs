using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.CompareTag("Player"))
        {
            Debug.Log("BOSS: player hit");
            trigger.GetComponent<PlayerStats>()?.TakeDamage(damage);
            trigger.GetComponent<MakoSimplifiedMovement>()?.Knockback(transform.position);
        }

    }
}
