using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Processors;
using System.ComponentModel;

public class golemAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;

    [SerializeField] int hp;
    [SerializeField] public int headShot;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int XP;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] int animTransSpeed;
    [SerializeField] int FOV;

    [SerializeField] Transform shootPosition;
    [SerializeField] GameObject bullet;
    [SerializeField] float attackRate;
    [SerializeField] int attackFOV;
    [SerializeField] float attackRange = 2.5f;
    [SerializeField] float attackCD = 1.2f;

    [SerializeField] MeleeHitbox meleeHitboxHead;
    [SerializeField] MeleeHitbox meleeHitboxHand;

    float angleToPlayer;
    bool playerInRange;
    bool isAttacking = false;

    float attackCDTimer = 0f;
    float attackTimer;
    float roamTimer;
    float stoppingDistanceOrig;

    Color colorOriginal;

    Vector3 playerDirection;
    Vector3 startingPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOriginal = model.material.color;
        startingPosition = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;
    }
    void Update()
    {
        setAnimLocomotion();

        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
        }

        if (playerInRange && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInRange)
        {
            checkRoam();
        }

        if (attackCDTimer > 0f)
        {
            attackCDTimer -= Time.deltaTime;
        }

    }

    void setAnimLocomotion()
    {
        if (isAttacking)
        {
            anim.SetBool("isMoving", false);
            return;
        }

        bool isWalking = agent.velocity.magnitude > 0.1f;

        anim.SetBool("isMoving", isWalking);
    }

    void checkRoam()
    {
        if (roamTimer >= roamPauseTime && agent.remainingDistance < 0.01f)
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPosition;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);

    }

    bool inMeleeRange()
    {
        Transform player = GameManager.instance.player.transform;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            return false;
        }

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, new Vector3(dirToPlayer.x, 0, dirToPlayer.z));

        return angle <= attackFOV;
    }
    bool canSeePlayer()
    {
        if (isAttacking)
        {
            return true;
        }


        playerDirection = (GameManager.instance.player.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);
        Debug.DrawRay(headPos.position, playerDirection);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {

                agent.SetDestination(GameManager.instance.player.transform.position);
             
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                attackTimer += Time.deltaTime;

                if (inMeleeRange() && attackTimer >= attackRate && attackCDTimer <= 0f)
                {
                    attack();
                }

                agent.stoppingDistance = stoppingDistanceOrig;
                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
        
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0; 
        }
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;

        StartCoroutine(flashRed());

        agent.SetDestination(GameManager.instance.player.transform.position);

        if (hp <= 0)
        {
            anim.SetTrigger("Die");
            agent.isStopped = true;
            Destroy(gameObject, 2f);
            GameManager.instance.updateXP(XP);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.yellow;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    void attack()
    {
        attackTimer = 0;
        attackCDTimer = attackCD;
        isAttacking = true;

        agent.isStopped = true;
        string[] attackTriggers = { "Hit", "Hit2" };
        string selectedAttack = attackTriggers[Random.Range(0, attackTriggers.Length)];
        anim.SetTrigger(selectedAttack);

    }
    void shoot()
    {
        anim.SetTrigger("Shoot");
    }
    public void createBullet()
    {
        Instantiate(bullet, shootPosition.position, transform.rotation);
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, transform.position.y, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void ActivateHitbox()
    {
        meleeHitboxHead.ActivateHitbox();
        meleeHitboxHand.ActivateHitbox();
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void ResumeMovement()
    {
        isAttacking = false;
        agent.isStopped = false;
    }
}
