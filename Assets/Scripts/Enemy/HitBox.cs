using UnityEngine;

public class HitBox : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.CompareTag("Player"))
        {
            trigger.GetComponent<PlayerStats>()?.TakeDamage(damage);
        }
    }
}