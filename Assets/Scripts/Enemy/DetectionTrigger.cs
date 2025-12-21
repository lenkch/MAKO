using UnityEngine;

public class DetectionTrigger : MonoBehaviour
{
    private EnemyBehaviour enemy;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyBehaviour>();
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        //Debug.Log("Entered trigger area.");
        if (trigger.CompareTag("Player"))
        {
            enemy.PlayerEnteredRange(trigger.transform.root.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D trigger)
    {
        //Debug.Log("Exit trigger area.");
        if (trigger.CompareTag("Player"))
        {
            enemy.PlayerLeftRange();
        }
    }
}
