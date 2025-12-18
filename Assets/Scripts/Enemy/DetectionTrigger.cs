using UnityEngine;

public class DetectionTrigger : MonoBehaviour
{
    private EnemyBehaviour enemy;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyBehaviour>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.PlayerEnteredRange(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.PlayerLeftRange();
        }
    }
}
