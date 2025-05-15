using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class playerController : MonoBehaviour, IDamage, iPickup, iAmmoPickup
{
    // Essentials
    [Header("--- Components ---")]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;

    // Movement
    [Header("--- Movement ---")]
    public int moveSpeed;
    [SerializeField] int sprintMod;
    public int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] float crouchHeightMod;
    [SerializeField] float crouchSpeedMod;

    [SerializeField] float checkOffset = 1f;
    [SerializeField] float checkRadius = 2f;

    // Shooting
    [Header("--- Gun ---")]
    [SerializeField] GameObject weaponModel;
    public weaponStats currentWeapon;

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

    int jumpCount;
    int baseSpeed;
    int baseHP;
    float attackTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;
    bool isPlayingStep;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseHP = HP;
        baseSpeed = moveSpeed;
        updateStats(); 
        UpdateHPUI();
        GameManager.instance.updateXP(0);
        spawnPlayer();
        weaponModel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        if (currentWeapon != null)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * currentWeapon.range);
        }

        Movement();
        Attack();

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position + new Vector3(0, checkOffset, 0), checkRadius, Vector3.up);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.tag == "Zipline")
                {
                    hit.collider.GetComponent<zipLine>().StartZipLine(gameObject);
                }
            }
        }
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
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        Jump();
        Sprint();
        Crouch();

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            jumpCount++;
            playerVel.y = jumpSpeed;
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
            controller.height = Mathf.RoundToInt(controller.height * crouchHeightMod);
            moveSpeed = Mathf.RoundToInt(moveSpeed * crouchSpeedMod);
        }
        if (Input.GetButtonUp("Crouch"))
        {
            controller.height = Mathf.RoundToInt(controller.height / crouchHeightMod);
            moveSpeed = Mathf.RoundToInt(moveSpeed / crouchSpeedMod);
        }
    }
    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            moveSpeed *= sprintMod;
            isSprinting = true;
        }
        if (Input.GetButtonUp("Sprint"))
        {
            moveSpeed /= sprintMod;
            isSprinting = false;
        }
    }

    void Attack()
    {
        if (GameManager.instance.isPaused || currentWeapon == null)
        {
            return;
        }

        if (currentWeapon.isMelee)
        {
            if (Input.GetButtonDown("Fire1") && attackTimer > currentWeapon.attackRate)
            {
                attackTimer = 0;
                checkCollision();
            }
        }
        else
        {
            if (currentWeapon.isAutomatic)
            {
                if (Input.GetButton("Fire1") && attackTimer > currentWeapon.attackRate && currentWeapon.currentAmmo > 0)
                {
                    attackTimer = 0;
                    currentWeapon.currentAmmo--;
                    GameManager.instance.UpdateAmmoUI();
                    UpdateHPUI();
                    checkCollision();
                    aud.PlayOneShot(currentWeapon.shootSound[Random.Range(0, currentWeapon.shootSound.Length)], currentWeapon.shootSoundVolume);
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && attackTimer > currentWeapon.attackRate && currentWeapon.currentAmmo > 0)
                {
                    attackTimer = 0;
                    currentWeapon.currentAmmo--;
                    GameManager.instance.UpdateAmmoUI();
                    UpdateHPUI();
                    checkCollision();
                    aud.PlayOneShot(currentWeapon.shootSound[Random.Range(0, currentWeapon.shootSound.Length)], currentWeapon.shootSoundVolume);
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

        if (Physics.Raycast(transform.position, transform.forward, out hit, currentWeapon.range, ~ignoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                if (currentWeapon.isMelee)
                    dmg.TakeDamage(currentWeapon.weaponDamage + GameManager.instance.abilityMod(stats.strength));
                else
                    dmg.TakeDamage(currentWeapon.weaponDamage);
            }

            Debug.Log(hit.collider);

            Instantiate(currentWeapon.hitEffect, hit.point, Quaternion.identity);
        }
    }

    public void GetWeaponStats(weaponStats weapon)
    {
        currentWeapon = weapon;
        weaponModel.GetComponent<MeshFilter>().sharedMesh = weapon.weaponModel.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weapon.weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
        if ((weapon.isMelee && GameManager.instance.ammoActive) || (!weapon.isMelee && !GameManager.instance.ammoActive))
        {
            GameManager.instance.ToggleAmmoUI();
            if (!weapon.isMelee)
            {
                GameManager.instance.UpdateAmmoUI();
            }
        }
        UpdateHPUI();
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload") && currentWeapon != null)
        {
            currentWeapon.currentAmmo = currentWeapon.magSize;
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
        moveSpeed = baseSpeed + GameManager.instance.abilityMod(stats.dexterity);
        MaxHP = baseHP + GameManager.instance.abilityMod(stats.constitution);
        UpdateHPUI();
    }

    public void addAmmo(int amount, GameManager.AmmoType ammoType)
    {
        switch ((int)ammoType)
        {
            case 0:
                {
                    GameManager.instance.playerAmmo.ThrowingStones += amount;
                    break;
                }
            case 1:
                {
                    GameManager.instance.playerAmmo.Arrows += amount;
                    break;
                }
            case 2:
                {
                    GameManager.instance.playerAmmo.Bolts += amount;
                    break;
                }
            case 3:
                {
                    GameManager.instance.playerAmmo._9mm += amount;
                    break;
                }
            case 4:
                {
                    GameManager.instance.playerAmmo._556mmNATO += amount;
                    break;
                }
            case 5:
                {
                    GameManager.instance.playerAmmo._50calBMG += amount;
                    break;
                }
            case 6:
                {
                    GameManager.instance.playerAmmo.PlasmaRounds += amount;
                    break;
                }
            case 7:
                {
                    GameManager.instance.playerAmmo.LaserRounds += amount;
                    break;
                }
            case 8:
                {
                    GameManager.instance.playerAmmo.PulseRounds += amount;
                    break;
                }
            default: break;
        }

    }
}