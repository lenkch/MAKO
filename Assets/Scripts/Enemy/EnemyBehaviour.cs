using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class EnemyBehaviour : MonoBehaviour
{
    #region Public Variables

    public Transform raycast;
    public LayerMask raycastMask; 
    public float raycastLength; 
    public float attackDistance; // min vzdialenost pre utok
    public float attachCooldown;
    public float enemyMovementSpeed; 
    public float timer; 

    #endregion 

    #region Private Variables

    private RaycastHit2D hit;
    private GameObject target;
    private Animator animator; 
    private float distance; // vzdialenost medzi hracom a enemy
    private bool isAttacking;   // ci je v stave utoku alebo nie
    private bool isInRange;     // ci je hrac in range pre utok
    private bool isAttackOnCooldown; // ci je stale cooldown po utoku 
    private float intTimer;

    #endregion
   
    void Awake()
    {
        intTimer = timer; 
        animator = GetComponent<Animator>();


    }

    void Update()
    {
            if (!isInRange)
        {
            animator.SetBool("canMove", false);
            StopAttacking();
            return;
        }

        // Always move when player is in trigger
        Move();

        // Only check raycast for attack range
        hit = Physics2D.Raycast(raycast.position, Vector2.left, raycastLength, raycastMask);
        RaycastDebugger();

        if (hit.collider != null)
        {
            EnemyLogic();  // attack logic
        }
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "Player")
        {
            target = trigger.gameObject;
            isInRange = true; 
        }
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > attackDistance)
        {
            Move();
            StopAttacking(); 

        }
        else if (distance <= attackDistance && !isAttackOnCooldown)
        {
            Attack(); 
        }

        if (isAttackOnCooldown)
        {
            animator.SetBool("Attack", false); 
        }
    }

    void Move()
    {
        animator.SetBool("canMove", true);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy Snake Attack"))
        {
            Vector2 moveToPosition = new Vector2(target.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, moveToPosition, enemyMovementSpeed * Time.deltaTime);
            
            Vector2 before = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, moveToPosition, enemyMovementSpeed * Time.deltaTime);
            Debug.Log($"Enemy moved from {before} to {transform.position}");
        }
    }

    void Attack()
    {
        timer = intTimer; // resetuje timer 
        isAttacking = true; 

        animator.SetBool("canMove", false);
        animator.SetBool("Attack", true);
    }

    void StopAttacking()
    {
        isAttackOnCooldown = false; 
        isAttacking = false;

        animator.SetBool("Attack", false);
        animator.SetBool("canMove", true);
    }

    void RaycastDebugger()
    {
        if (distance > attackDistance)
        {
            Debug.DrawRay(raycast.position, Vector2.left * raycastLength, Color.red);
        }
        else if (attackDistance > distance)
        {
            Debug.DrawRay(raycast.position, Vector2.left * raycastLength, Color.green);
        }
    }
}
