using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Processors;
using System.ComponentModel;
using System.Collections.Generic;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class KillerController : MonoBehaviour, IDamage
{
    [Header("----- Arsenal -----")]
    public Transform rightGunBone;
    public Transform leftGunBone;
    public Arsenal[] arsenal;

    private Animator animator;
    private Actions actions;
    private NavMeshAgent agent;
    private Transform player;

    [Header("----- AI Settings -----")]
    [SerializeField] float chaseRange = 20f;
    [SerializeField] float attackRange = 10f;
    [SerializeField] float attackCooldown = 2f;
    [SerializeField] float crouchRunChance = 0.2f;
    [SerializeField] float crouchDuration = 3f;
    [SerializeField] float jumpCooldown = 5f;
    [SerializeField] float jumpChance = 0.15f;
    [SerializeField] Transform rightShootPoint;
    [SerializeField] Transform leftShootPoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] int bulletDamage = 10;
    [SerializeField] int hp;
    [SerializeField] int XP;


    [Header("----- Audio -----")]
    [SerializeField] AudioSource aud;

    [SerializeField] AudioClip runSFX;
    [SerializeField] AudioClip walkSFX;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip jumpSFX;

    [Range(0, 1)][SerializeField] float runVol = 0.5f;
    [Range(0, 1)][SerializeField] float walkVol = 0.5f;
    [Range(0, 1)][SerializeField] float shootVol = 0.6f;
    [Range(0, 1)][SerializeField] float deathVol = 0.7f;
    [Range(0, 1)][SerializeField] float jumpVol = 0.6f;

    private float jumpTimer = 0f;
    private bool shootRightNext = true;
    private bool isCrouchRunning = false;
    private float attackTimer;
    private bool playerInCombatZone = false;
    private bool isAiming = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        actions = GetComponent<Actions>();
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindWithTag("Player").transform;

        if (arsenal.Length > 0)
            SetArsenal(arsenal[0].name);
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // If player is in combat zone (triggered)
        if (playerInCombatZone)
        {
            // Stop moving
            agent.isStopped = true;

            // Rotate toward player
            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200f * Time.deltaTime);

            // Shoot if within attack range
            if (dist <= attackRange && attackTimer <= 0f)
            {
                StartCoroutine(AttackRoutine());
                attackTimer = attackCooldown;
            }

            if (!isCrouchRunning && Random.value < crouchRunChance * Time.deltaTime)
            {
                StartCoroutine(CrouchingRunRoutine());
            }

            if (jumpTimer <= 0f && Random.value < jumpChance * Time.deltaTime)
            {
                StartCoroutine(JumpRoutine());
            }

            if (jumpTimer > 0f)
                jumpTimer -= Time.deltaTime;
        }
        else if (dist <= chaseRange)
        {
            // Not in combat zone, chase
            agent.isStopped = false;
            agent.SetDestination(player.position);

            float speed = agent.velocity.magnitude;
            if (speed > 2f)
            {
                actions.Run();
                if (!aud.isPlaying) PlaySound(runSFX, runVol);
            }

            else if (speed > 0.1f)
            {
                actions.Walk();
                if (!aud.isPlaying) PlaySound(walkSFX, walkVol);
            }
                
            else
                actions.Stay();
        }

        // Cooldown timer
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

    }

    void PlaySound(AudioClip clip, float volume)
    {
        if (clip && aud)
            aud.PlayOneShot(clip, volume);
    }
    IEnumerator AttackRoutine()
    {
        actions.Attack();
        PlaySound(shootSFX, shootVol);

        // Choose which hand to shoot from
        Transform shootPoint = shootRightNext ? rightShootPoint : leftShootPoint;

        // Instantiate the bullet
        if (bulletPrefab && shootPoint)
        {
            Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        }

        shootRightNext = !shootRightNext; // alternate hands

        yield return new WaitForSeconds(1f); // Adjust to match attack animation
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInCombatZone = true;

            if (!isAiming)
            {
                isAiming = true;
                actions.Aiming(); // only once
                agent.isStopped = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInCombatZone = false;
            isAiming = false;
            actions.Run();
            agent.isStopped = false;
        }
    }

    public void SetArsenal(string name)
    {
        foreach (Arsenal hand in arsenal)
        {
            if (hand.name == name)
            {
                if (rightGunBone.childCount > 0)
                    Destroy(rightGunBone.GetChild(0).gameObject);
                if (leftGunBone.childCount > 0)
                    Destroy(leftGunBone.GetChild(0).gameObject);

                if (hand.rightGun != null)
                {
                    GameObject newRightGun = Instantiate(hand.rightGun);
                    newRightGun.transform.SetParent(rightGunBone);
                    newRightGun.transform.localPosition = Vector3.zero;
                    newRightGun.transform.localRotation = Quaternion.Euler(90, 0, 0);

                    rightShootPoint = newRightGun.transform.Find("ShootPoint");
                }

                if (hand.leftGun != null)
                {
                    GameObject newLeftGun = Instantiate(hand.leftGun);
                    newLeftGun.transform.SetParent(leftGunBone);
                    newLeftGun.transform.localPosition = Vector3.zero;
                    newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);

                    leftShootPoint = newLeftGun.transform.Find("ShootPoint");
                }

                animator.runtimeAnimatorController = hand.controller;
                return;
            }
        }
    }

    IEnumerator CrouchingRunRoutine()
    {
        isCrouchRunning = true;
        actions.Sitting(); // Assuming this toggles crouch
        actions.Run();     // Still use run speed

        yield return new WaitForSeconds(crouchDuration);

        actions.Sitting(); // Toggle crouch off
        isCrouchRunning = false;
    }

    IEnumerator JumpRoutine()
    {
        jumpTimer = jumpCooldown;

        actions.Jump(); // This triggers the "Jump" animation via your Actions.cs
        PlaySound(jumpSFX, jumpVol);
        yield return new WaitForSeconds(1f); // adjust based on animation length
    }

    public void Die()
    {
        actions.Death();
        PlaySound(deathSFX, deathVol);

        agent.isStopped = true;
        this.enabled = false;
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;

        agent.SetDestination(GameManager.instance.player.transform.position);

        if (hp <= 0)
        {
            Die();
            agent.isStopped = true;
            Destroy(gameObject, 2f);
            GameManager.instance.updateXP(XP);
        }
    }


    [System.Serializable]
    public struct Arsenal
    {
        public string name;
        public GameObject rightGun;
        public GameObject leftGun;
        public RuntimeAnimatorController controller;
    }


}