using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class mechdudeAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;
    [SerializeField] AudioSource aud;


    [Header("----- Gun -----")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] float fireRate = 1.5f;
    [SerializeField] float fireRange = 15f;
    [SerializeField] AudioClip[] audShoot;
    [Range(0, 1)][SerializeField] float shootVol;

    [Header("----- Stats -----")]
    [SerializeField] int hp;
    [SerializeField] int XP;
    [SerializeField] float chaseFOV = 100f;
    [SerializeField] int faceTargetSpeed = 5;
    [SerializeField] float aggroRange = 20f;

    [Header("----- Roaming -----")]
    [SerializeField] float roamDist = 10f;
    [SerializeField] float roamPauseTime = 3f;

    [Header("----- Laser Eye -----")]
    [SerializeField] Transform eyeTransform;
    [SerializeField] LineRenderer laserLine;
    [SerializeField] float laserRange = 20f;
    [SerializeField] float laserDuration = 0.1f;
    [SerializeField] float laserCooldown = 5f;
    [SerializeField] int laserDamagePerSecond = 5;
    [SerializeField] AudioClip laserSFX;
    [Range(0, 1)][SerializeField] float laserVol;


    bool isFiringLaser = false;
    float laserCooldownTimer = 0f;
    float fireCooldown;
    float roamTimer;
    bool isShooting = false;
    bool playerAggroed = false;

    Vector3 playerDirection;
    Vector3 startingPosition;
    Color originalColor;

    void Start()
    {
        originalColor = model.material.color;
        startingPosition = transform.position;
    }

    void Update()
    {
        setAnimLocomotion();

        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;

        if (isShooting)
            return;

        if (canSeePlayer())
        {
            playerAggroed = true;

            float dist = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

            if (dist <= fireRange && fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = fireRate;
                return;
            }

            agent.stoppingDistance = 5f;
            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
                faceTarget();
        }
        else
        {
            if (playerAggroed)
            {
                float dist = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);
                if (dist > aggroRange)
                    playerAggroed = false;
            }

            if (!playerAggroed)
            {
                checkRoam();
            }
        }

        if (laserCooldownTimer > 0)
            laserCooldownTimer -= Time.deltaTime;

        if (playerAggroed && !isFiringLaser && laserCooldownTimer <= 0f)
        {
            StartCoroutine(FireLaserEye());
            laserCooldownTimer = laserCooldown;
        }
    }

    void setAnimLocomotion()
    {
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
        agent.stoppingDistance = 0f;

        Vector3 ranPos = Random.insideUnitSphere * roamDist + startingPosition;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(ranPos, out hit, roamDist, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    bool canSeePlayer()
    {
        playerDirection = GameManager.instance.player.transform.position - headPos.position;
        float angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);
        float distanceToPlayer = playerDirection.magnitude;

        if (distanceToPlayer > aggroRange)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= chaseFOV)
            {
                return true;
            }
        }
        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void Shoot()
    {
        if (isShooting) return;
        StartCoroutine(DelayedShoot(1f));
    }

    IEnumerator DelayedShoot(float delay)
    {
        isShooting = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        anim.SetBool("isMoving", false);

        float timer = 0f;
        float rotationSpeed = 200f;
        while (timer < delay)
        {
            Vector3 dirToPlayer = GameManager.instance.player.transform.position - transform.position;
            dirToPlayer.y = 0f;

            if (dirToPlayer.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        anim.SetTrigger("Shoot");

        if (audShoot.Length > 0)
        {
            aud.PlayOneShot(audShoot[Random.Range(0, audShoot.Length)], shootVol);
        }

        Vector3 targetDir = (GameManager.instance.player.transform.position - shootPoint.position).normalized;
        Quaternion bulletRotation = Quaternion.LookRotation(targetDir);
        Instantiate(bulletPrefab, shootPoint.position, bulletRotation);

        yield return new WaitForSeconds(0.6f);
        isShooting = false;
        agent.isStopped = false;
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        StartCoroutine(DamageFlash());

        if (hp <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageFlash()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = originalColor;
    }

    void Die()
    {
        anim.SetTrigger("Death");
        agent.isStopped = true;
        GameManager.instance.updateXP(XP);
        Destroy(gameObject, 3f);
    }

    IEnumerator FireLaserEye()
    {
        isFiringLaser = true;
        agent.isStopped = true;

        if (laserSFX)
            aud.PlayOneShot(laserSFX, laserVol);

        Vector3 start = eyeTransform.position;
        Vector3 direction = (GameManager.instance.player.transform.position - start).normalized;
        Vector3 end = start + direction * laserRange;

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, laserRange))
        {
            end = hit.point;

            if (hit.collider.CompareTag("Player"))
            {
                GameManager.instance.player.GetComponent<IDamage>().TakeDamage(Mathf.RoundToInt(laserDamagePerSecond * laserDuration));
            }
        }

        laserLine.positionCount = 2;
        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);
        laserLine.enabled = true;

        yield return new WaitForSeconds(laserDuration);

        yield return StartCoroutine(ShrinkLaserLine(start, end));

        laserLine.enabled = false;
        isFiringLaser = false;
        agent.isStopped = false;
    }

    IEnumerator ShrinkLaserLine(Vector3 start, Vector3 end)
    {
        float shrinkTime = 0.5f;
        float elapsed = 0f;

        while (elapsed < shrinkTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shrinkTime);

            Vector3 currentEnd = Vector3.Lerp(end, start, t);
            laserLine.SetPosition(0, start);
            laserLine.SetPosition(1, currentEnd);

            yield return null;
        }

        laserLine.SetPosition(1, start);
    }
}
