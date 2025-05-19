using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class playerController : MonoBehaviour, IDamage, iPickup, iAmmoPickup
{
    // Essentials
    [Header("--- Components ---")]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;
    [Range(5, 10)][SerializeField] int interactRange;

    // Movement
    [Header("--- Movement ---")]
    public player pStats;

    [SerializeField] float checkOffset = 1f;
    [SerializeField] float checkRadius = 2f;

    [Header("----- Steps Audio -----")]
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;

    [Header("----- Jump Audio -----")]
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;

    [Header("----- Hurt Audio -----")]
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;

    // Stats
    [Header("--- Player Stats ---")]
    public int HP;
    public int MaxHP;
    public playerStats stats;
    public GameObject weaponModel;

    [Header("----- Knockback -----")]
    [SerializeField] float knockbackDuration = 0.2f;
    [SerializeField] float knockbackForce = 8f;

    int jumpCount;
    int baseSpeed;
    int baseHP;
    float attackTimer;
    float knockbackTimer;

    Vector3 knockbackDirection;
    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;
    bool isPlayingStep;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {

    }
    void Start()
    {
        baseHP = HP;
        baseSpeed = pStats.moveSpeed;
        updateStats(); 
        UpdateHPUI();
        GameManager.instance.updateXP(0);
        spawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        if (pStats.currentWeapon != null)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pStats.currentWeapon.range);
        }

        Movement();
        Attack();
        Interact();
    }

    public void UpdateHPUI()
    {
        GameManager.instance.hpValue.text = HP.ToString() + "/" + MaxHP.ToString();

        GameManager.instance.playerHPBar.fillAmount = (float)HP / MaxHP;
    }

    void Movement()
    {
        if (controller.isGrounded)
        {
            if (moveDir.normalized.magnitude > 0.3f && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }

            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * pStats.moveSpeed * Time.deltaTime);

        Jump();
        Sprint();
        Crouch();

        playerVel.y -= pStats.gravity * Time.deltaTime;

        if (knockbackTimer > 0)
        {
            controller.Move(knockbackDirection * Time.deltaTime);
            knockbackTimer -= Time.deltaTime;
        }
        else
        {
            controller.Move(playerVel * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < pStats.jumpMax)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            jumpCount++;
            playerVel.y = pStats.jumpSpeed;
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        UpdateHPUI();
        StartCoroutine(flashDamage());
        if (HP <= 0)
        {
            GameManager.instance.youDied();
        }
    }

    void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            controller.height = Mathf.RoundToInt(controller.height * pStats.crouchHeightMod);
            pStats.moveSpeed = Mathf.RoundToInt(pStats.moveSpeed * pStats.crouchSpeedMod);
        }
        if (Input.GetButtonUp("Crouch"))
        {
            controller.height = Mathf.RoundToInt(controller.height / pStats.crouchHeightMod);
            pStats.moveSpeed = Mathf.RoundToInt(pStats.moveSpeed / pStats.crouchSpeedMod);
        }
    }
    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            pStats.moveSpeed *= pStats.sprintMod;
            isSprinting = true;
        }
        if (Input.GetButtonUp("Sprint"))
        {
            pStats.moveSpeed /= pStats.sprintMod;
            isSprinting = false;
        }
    }

    void Attack()
    {
        if (GameManager.instance.isPaused || pStats.currentWeapon == null)
        {
            return;
        }

        if (pStats.currentWeapon.isMelee)
        {
            if (Input.GetButtonDown("Fire1") && attackTimer > pStats.currentWeapon.attackRate)
            {
                attackTimer = 0;
                checkCollision();
            }
        }
        else
        {
            if (pStats.currentWeapon.isAutomatic)
            {
                if (Input.GetButton("Fire1") && attackTimer > pStats.currentWeapon.attackRate && pStats.currentWeapon.currentAmmo > 0)
                {
                    attackTimer = 0;
                    pStats.currentWeapon.currentAmmo--;
                    GameManager.instance.UpdateAmmoUI();
                    UpdateHPUI();
                    checkCollision();
                    aud.PlayOneShot(pStats.currentWeapon.shootSound[Random.Range(0, pStats.currentWeapon.shootSound.Length)], pStats.currentWeapon.shootSoundVolume);
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && attackTimer > pStats.currentWeapon.attackRate && pStats.currentWeapon.currentAmmo > 0)
                {
                    attackTimer = 0;
                    pStats.currentWeapon.currentAmmo--;
                    GameManager.instance.UpdateAmmoUI();
                    UpdateHPUI();
                    checkCollision();
                    aud.PlayOneShot(pStats.currentWeapon.shootSound[Random.Range(0, pStats.currentWeapon.shootSound.Length)], pStats.currentWeapon.shootSoundVolume);
                }
            }
        }

        reload();
    }

    IEnumerator flashDamage()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    private void checkCollision()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, pStats.currentWeapon.range, ~ignoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                if (pStats.currentWeapon.isMelee)
                    dmg.TakeDamage(pStats.currentWeapon.weaponDamage + GameManager.instance.abilityMod(stats.strength));
                else
                    dmg.TakeDamage(pStats.currentWeapon.weaponDamage);
            }

            Debug.Log(hit.collider);

            Instantiate(pStats.currentWeapon.hitEffect, hit.point, Quaternion.identity);
        }
    }

    public void GetWeaponStats(weaponStats weapon)
    {
        pStats.currentWeapon = weapon;
        weaponModel.GetComponent<MeshFilter>().sharedMesh = pStats.currentWeapon.weaponModel.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = pStats.currentWeapon.weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload") && pStats.currentWeapon != null)
        {
            pStats.currentWeapon.currentAmmo = pStats.currentWeapon.magSize;
            GameManager.instance.UpdateAmmoUI();
            UpdateHPUI();
        }
    }

    public void spawnPlayer()
    {
        controller.transform.position = GameManager.instance.playerSpawnPos.transform.position;

        HP = MaxHP;
        UpdateHPUI();
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if (isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        isPlayingStep = false;
    }

    public void updateStats()
    {
        GameManager.instance.strText.text = stats.strength.ToString("F0");
        GameManager.instance.dexText.text = stats.dexterity.ToString("F0");
        GameManager.instance.conText.text = stats.constitution.ToString("F0");
        GameManager.instance.intText.text = stats.intelligence.ToString("F0");
        GameManager.instance.chaText.text = stats.charisma.ToString("F0");
        GameManager.instance.wisText.text = stats.wisdom.ToString("F0");
        pStats.moveSpeed = baseSpeed + GameManager.instance.abilityMod(stats.dexterity);
        MaxHP = baseHP + GameManager.instance.abilityMod(stats.constitution);
        UpdateHPUI();
    }

    public void addAmmo(int amount, GameManager.AmmoType ammoType)
    {
        switch (ammoType)
        {
            case GameManager.AmmoType.ThrowingStones:
                {
                    GameManager.instance.playerAmmo.ThrowingStones += amount;
                    break;
                }
            case GameManager.AmmoType.Arrows:
                {
                    GameManager.instance.playerAmmo.Arrows += amount;
                    break;
                }
            case GameManager.AmmoType.Bolts:
                {
                    GameManager.instance.playerAmmo.Bolts += amount;
                    break;
                }
            case GameManager.AmmoType._9mm:
                {
                    GameManager.instance.playerAmmo._9mm += amount;
                    break;
                }
            case GameManager.AmmoType._556mmNATO:
                {
                    GameManager.instance.playerAmmo._556mmNATO += amount;
                    break;
                }
            case GameManager.AmmoType._50calBMG:
                {
                    GameManager.instance.playerAmmo._50calBMG += amount;
                    break;
                }
            case GameManager.AmmoType.PlasmaRounds:
                {
                    GameManager.instance.playerAmmo.PlasmaRounds += amount;
                    break;
                }
            case GameManager.AmmoType.LaserRounds:
                {
                    GameManager.instance.playerAmmo.LaserRounds += amount;
                    break;
                }
            case GameManager.AmmoType.PulseRounds:
                {
                    GameManager.instance.playerAmmo.PulseRounds += amount;
                    break;
                }
            default: break;
        }

    }

    public void Interact()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            iInteract interactable = hit.collider.GetComponent<iInteract>();
            if (interactable != null)
            {
                GameManager.instance.ShowInteractText(true);
                if (Input.GetButtonDown("Interact"))
                {
                    interactable.onInteract();
                }
            }
            else
            {
                GameManager.instance.ShowInteractText(false);
            }
        }
        else
        {
            GameManager.instance.ShowInteractText(false);
        }
    }

    public void ApplyKnockback(Vector3 source, float force)
    {
        Vector3 direction = (transform.position - source).normalized;
        direction.y = 0f;

        knockbackDirection = direction * force;
        knockbackTimer = knockbackDuration;
    }
}