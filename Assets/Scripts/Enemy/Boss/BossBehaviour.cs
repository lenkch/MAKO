using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    private Animator animator; 
     public int damage = 1;
    public float attackDistance;
    public Collider2D biteHitbox;
    public RectTransform heartsObject;
    private GameObject target;
    private bool isInRange; 

    void Awake()
    {
        animator = GetComponent<Animator>();
        biteHitbox.enabled = false; 
    }

    void Update()
    {
        if (isInRange)
        {
            animator.SetBool("Attack", true);
            animator.SetBool("Idle", false);
        }
        else
        {
            animator.SetBool("Attack", false);
            animator.SetBool("Idle", true);
        }

        if (target != null)
        {
            if (target.transform.position.x < transform.position.x)
            {
                transform.SetPositionAndRotation(transform.position, Quaternion.AngleAxis(0, Vector3.up));
                heartsObject.SetLocalPositionAndRotation(heartsObject.localPosition, Quaternion.AngleAxis(0, Vector3.up));
            } else
            {
                transform.SetPositionAndRotation(transform.position, Quaternion.AngleAxis(180, Vector3.up));
                heartsObject.SetLocalPositionAndRotation(heartsObject.localPosition, Quaternion.AngleAxis(180, Vector3.up));
            }
        }
    }

    public void PlayerEnteredRange(GameObject player)
    {
        
        target = player;
        isInRange = true;
    }

    public void PlayerLeftRange()
    {
        isInRange = false;
        //target = null;

        animator.SetBool("Idle", true);
        animator.SetBool("Attack", false);
    }

    public void EnableHitbox()
    {
        biteHitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        biteHitbox.enabled = false;
    }
    
}
