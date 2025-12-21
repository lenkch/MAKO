using UnityEngine;

public class BossDetectionTrigger : MonoBehaviour
{
    private BossBehaviour boss;

   

    void Awake()
    {
        boss = GetComponentInParent<BossBehaviour>();
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.CompareTag("Player"))
        {
            boss.PlayerEnteredRange(trigger.transform.root.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D trigger)
    {
        //Debug.Log("Exit trigger area.");
        if (trigger.CompareTag("Player"))
        {
            boss.PlayerLeftRange();
        }
    }
}
