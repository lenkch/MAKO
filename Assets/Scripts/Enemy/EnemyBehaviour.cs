using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class EnemyBehaviour : MonoBehaviour
{
    #region Public Variables

    [Header("Raycast")]
    public Transform raycast;
    public LayerMask raycastMask; 
    public float raycastLength; 

    [Header("Enemy Params")]
    public float attackDistance; // min vzdialenost pre utok
    public float enemyMovementSpeed; 
    public float offsetDistance; 
    public float timer; 

    [Header("Attack")]
    public int damage = 1;


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
    private bool isDead = false;

    private Rigidbody2D rigidBody; 
    private Vector2 targetMovePosition;
    private Vector2 direction = Vector2.left; 


    #endregion
   
    void Awake()
    {
        intTimer = timer; 
        animator = GetComponent<Animator>();

        rigidBody = GetComponent<Rigidbody2D>(); 

        GetComponent<EnemyHealth>().OnDeath.AddListener(OnDeath);
    }

    void Update()
    {
        FacePlayer();

        if (isInRange)
        {
            hit = Physics2D.Raycast(raycast.position, direction, raycastLength, raycastMask);
            RaycastDebugger();
        }


        if (isInRange)
        {
            EnemyLogic();  
        }
        else if (hit.collider == null)
        {
            //isInRange = false;
        }

        if (!isInRange)
        {
            animator.SetBool("canMove", false);
            StopAttack(); 
        }
    }

    void FixedUpdate()
    {
        if (!isInRange)
        {
            return;
        }

        if (direction == Vector2.left)
        {
            targetMovePosition.x += offsetDistance;
        } 
        else
        {
            targetMovePosition.x -= offsetDistance;
        }

        rigidBody.MovePosition(Vector2.MoveTowards(rigidBody.position, targetMovePosition, enemyMovementSpeed * Time.fixedDeltaTime));
    }

    public void PlayerEnteredRange(GameObject player)
    {
        target = player;
        isInRange = true;
    }

    public void PlayerLeftRange()
    {
        isInRange = false;
        target = null;

        animator.SetBool("canMove", false);
        StopAttack();
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);
        //Debug.Log("Distance to player: " + distance);

        if (isAttacking) return;

        if (distance > attackDistance)
        {
            Move();
            StopAttack();
        }
        else if (distance <= attackDistance && !isAttackOnCooldown)
        {
            Attack();        
        }

        if (isAttackOnCooldown)
        {
            Cooldown();
            animator.SetBool("Attack", false); 
        }
    }

    void FacePlayer()
    {
        if (target == null)
        {
            return;
        }

        float playerDirection = target.transform.position.x - transform.position.x;

        if (playerDirection < 0)
        {
            // hrac je nalavo
            direction = Vector2.left;
            transform.localScale = new Vector3(1,1,1);
        }
        else if (playerDirection > 0)
        {
            // hrac je napravo
            direction = Vector2.right;
            transform.localScale = new Vector3(-1,1,1);
        }

    }

    void Move()
    {
        animator.SetBool("canMove", true);

        distance = Mathf.Abs(target.transform.position.x - rigidBody.position.x);


        if (distance <= offsetDistance)
        {
            animator.SetBool("canMove", false);
            return;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy Snake Attack"))
        {
            targetMovePosition = new Vector2(target.transform.position.x, rigidBody.position.y);
        }

    }

    void Attack()
    {
        timer = intTimer; 

        if (isAttacking || isAttackOnCooldown)
        {
            return;
        }

        isAttacking = true; 

        animator.SetBool("canMove", false);
        animator.SetBool("Attack", true);
    }

    public void DealDamage()
    {
        if (target == null)
        {
            return;
        }

        float dist = Vector2.Distance(transform.position, target.transform.position);

        if (dist <= attackDistance)
        {
            target.GetComponent<PlayerStats>()?.TakeDamage(damage);
            Debug.Log("Snake dealt damage");
        }
    }

    void StopAttack()
    {
        isAttackOnCooldown = false; 
        isAttacking = false; 

        animator.SetBool("Attack", false);
    }

    void Cooldown()
    {
        timer -= Time.deltaTime; 

        if (timer <= 0f)
        {
            isAttackOnCooldown = false;
            timer = intTimer; 
        }
    }
    

    public void SetAttackOnCooldown()
    {
        isAttacking = false;
        isAttackOnCooldown = true;
        timer = intTimer;

        animator.SetBool("Attack", false);
    }

    void OnDeath()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        animator.SetTrigger("Die");

        // disable vsetky collidery (ci to je jedno dunno?) a pohyb
        rigidBody.linearVelocity = Vector2.zero; 
        rigidBody.simulated = false;
    }

    public void DestroyEnemy()
    {
        Debug.Log("Destroy called");
        Destroy(gameObject, 10f);
    }

    void RaycastDebugger()
    {

        if (distance > attackDistance)
        {
            Debug.DrawRay(raycast.position, direction * raycastLength, Color.red);
        }
        else if (attackDistance > distance)
        {
            Debug.DrawRay(raycast.position, direction * raycastLength, Color.green);
        }
    }
}
