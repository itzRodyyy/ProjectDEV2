using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Processors;
using System.ComponentModel;
using System.Collections.Generic;

public class golemAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;
    [SerializeField] AudioSource aud;

    [Header("----- Steps Audio -----")]
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;

    [Header("----- Jump Audio -----")]
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;

    [Header("----- Hurt Audio -----")]
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;

    [Header("----- Attack Audio -----")]
    [SerializeField] AudioClip[] audAttack;
    [Range(0, 1)][SerializeField] float audAttackVol;

    [Header("----- Fireball Audio -----")]
    [SerializeField] AudioClip[] audFireball;
    [Range(0, 1)][SerializeField] float audFireballVol;

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

    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float landDelay = 1f;
    [SerializeField] float aoeRadius = 3f;
    [SerializeField] float jumpTriggerRange = 7f;
    [SerializeField] int aoeDamage = 20;
    [SerializeField] float jumpCooldown = 5f;
    [SerializeField] float jumpPauseDelay = 0.4f;

    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform fireballSpawnPoint;
    [SerializeField] float fireballCooldown = 6f;
    [SerializeField] float fireballSpeed = 15f;
    [SerializeField] float fireballRange = 10f;

    [SerializeField] LayerMask playerLayer;

    Vector3 jumpTarget;

    float angleToPlayer;
    bool playerInRange;
    bool isAttacking = false;
    bool isJumping = false;
    bool isSprinting;
    bool isPlayingStep;

    float attackCDTimer = 0f;
    float attackTimer;
    float roamTimer;
    float stoppingDistanceOrig;
    float jumpCooldownTimer = 0f;
    float fireballCooldownTimer = 0f;

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

        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        if (fireballCooldownTimer > 0f)
        {
            fireballCooldownTimer -= Time.deltaTime;
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

    IEnumerator playStep()
    {
        isPlayingStep = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        yield return new WaitForSeconds(0.5f);

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

    bool inJumpRange()
    {
        Transform player = GameManager.instance.player.transform;
        float distance = Vector3.Distance(transform.position, player.position);

        return distance <= jumpTriggerRange && distance > attackRange;
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

                if (!isJumping && !isAttacking && inJumpRange() && jumpCooldownTimer <= 0f)
                {
                    PerformJump();
                    jumpCooldownTimer = jumpCooldown;
                }

                if (!isJumping && !isAttacking && fireballCooldownTimer <= 0f && Vector3.Distance(transform.position, GameManager.instance.player.transform.position) <= fireballRange)
                {
                    SpitFireball();
                    fireballCooldownTimer = fireballCooldown;
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, jumpTriggerRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }

    public void ResumeMovement()
    {
        isAttacking = false;
        agent.isStopped = false;
    }

    void PerformJump()
    {
        if (isJumping || isAttacking) return;

        isJumping = true;
        isAttacking = true;
        agent.isStopped = true;

        jumpTarget = GameManager.instance.player.transform.position;
        transform.LookAt(new Vector3(GameManager.instance.player.transform.position.x, transform.position.y, GameManager.instance.player.transform.position.z));
        anim.SetTrigger("Jump");
        StartCoroutine(DoJump());
    }

    IEnumerator DoJump()
    {
        yield return new WaitForSeconds(jumpPauseDelay);

        Vector3 start = transform.position;
        Vector3 end = new Vector3(jumpTarget.x, start.y, jumpTarget.z);
        float peakHeight = jumpHeight;
        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            Vector3 horizontal = Vector3.Lerp(start, end, t);

            float height = 4 * peakHeight * (t - t * t);
            Vector3 arcPos = new Vector3(horizontal.x, start.y + height, horizontal.z);

            transform.position = arcPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        Land();
    }

    void Land()
    {
        anim.SetTrigger("Land");

        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius, playerLayer);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                IDamage dmg = hit.GetComponent<IDamage>();
                if (dmg != null)
                {
                    dmg.TakeDamage(aoeDamage);
                }
            }
        }

        StartCoroutine(RecoverFromJump());
    }

    IEnumerator RecoverFromJump()
    {
        yield return new WaitForSeconds(landDelay);
        isJumping = false;
        isAttacking = false;
        agent.isStopped = false;
    }

    void SpitFireball()
    {
        isAttacking = true;
        agent.isStopped = true;
        transform.LookAt(new Vector3(GameManager.instance.player.transform.position.x, transform.position.y, GameManager.instance.player.transform.position.z));
        anim.SetTrigger("Rage");

        StartCoroutine(FireballAttack());
    }

    IEnumerator FireballAttack()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 dir = (GameManager.instance.player.transform.position - fireballSpawnPoint.position).normalized;
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.LookRotation(dir));

        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir * fireballSpeed;
        }

        yield return new WaitForSeconds(1f);
        isAttacking = false;
        agent.isStopped = false;
    }

    public void StepSound()
    {
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
    }

    public void FireballSound()
    {
        aud.PlayOneShot(audFireball[Random.Range(0, audFireball.Length)], audFireballVol);
    }
}
