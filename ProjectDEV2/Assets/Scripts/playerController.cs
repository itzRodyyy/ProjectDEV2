using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public class playerController : MonoBehaviour, IDamage, iPickup
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

    // Shooting
    [Header("--- Gun ---")]
    [SerializeField] GameObject weaponModel;
    [SerializeField] List<weaponStats> weapons = new List<weaponStats>();
    bool isAutomatic;
    int weaponDamage;
    float attackRate;
    int range;
    int currentAmmo;
    int magSize;
    Vector3 shootPosOffset;
    [Range(0, 3)] int currentWeapon;

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

    int jumpCount;
    float attackTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    float trailDuration;
    LineRenderer lineRenderer;

    bool isSprinting;
    bool isPlayingStep;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        MaxHP = HP;
        UpdateHPUI();
        GameManager.instance.updateXP(0);
        spawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range);
        Movement();
        Attack();
        selectWeapon();

    }

    public void UpdateHPUI()
    {
        GameManager.instance.hpValue.text = HP.ToString() + "/" + MaxHP.ToString();

        GameManager.instance.playerHPBar.fillAmount = (float)HP / MaxHP;

        if (weapons.Count > 0)
        {
            GameManager.instance.ammoCurr.text = weapons[currentWeapon].currentAmmo.ToString("F0");
            GameManager.instance.ammoMax.text = weapons[currentWeapon].magSize.ToString("F0");
        }
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
            // You died
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
        if (!GameManager.instance.isPaused && weapons.Count > 0) 
        {
            if (isAutomatic)
            {
                if (Input.GetButton("Fire1") && attackTimer > attackRate && weapons[currentWeapon].currentAmmo > 0)
                {
                    attackTimer = 0;
                    weapons[currentWeapon].currentAmmo--;
                    UpdateHPUI();
                    checkCollision();
                    StartCoroutine(showTrail());
                    aud.PlayOneShot(weapons[currentWeapon].shootSound[Random.Range(0, weapons[currentWeapon].shootSound.Length)], weapons[currentWeapon].shootSoundVolume);
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && attackTimer > attackRate && weapons[currentWeapon].currentAmmo > 0)
                {
                    attackTimer = 0;
                    weapons[currentWeapon].currentAmmo--;
                    UpdateHPUI();
                    checkCollision();
                    StartCoroutine(showTrail());
                    aud.PlayOneShot(weapons[currentWeapon].shootSound[Random.Range(0, weapons[currentWeapon].shootSound.Length)], weapons[currentWeapon].shootSoundVolume);
                }
            }
            reload(); 
        }
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
        if (Physics.Raycast(transform.position, transform.forward, out hit, range, ~ignoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.TakeDamage(weaponDamage);
            }
            Debug.Log(hit.collider);
            Instantiate(weapons[currentWeapon].hitEffect, hit.point, Quaternion.identity);
            SetTrailPoints(Camera.main.transform.position + Camera.main.transform.forward * hit.distance);
        }
        else
            SetTrailPoints(Camera.main.transform.position + Camera.main.transform.forward * range);
    }

    public void GetWeaponStats(weaponStats weapon)
    {
        if (!weapons.Contains(weapon))
        {
            weapons.Add(weapon);

            if (weapons.Count == 1)
            {
                currentWeapon = 0;
                changeWeapon();
            }

        }
        UpdateHPUI();
    }

    void selectWeapon()
    {
        if (weapons.Count == 0)
        {
            return;
        }

        int prevWeapon = currentWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentWeapon = (currentWeapon - 1 + weapons.Count) % weapons.Count;

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentWeapon = (currentWeapon + 1) % weapons.Count;

        }

        if (prevWeapon != currentWeapon)
        {
            changeWeapon();
            UpdateHPUI();
            inventoryManager.instance.SelectSlot(currentWeapon);
        }


    }

    void changeWeapon()
    {
        Debug.Log("Current Weapon Index: " + currentWeapon);
        Debug.Log("Weapons Count: " + weapons.Count);
        Debug.Log("weaponModel: " + weaponModel);
        Debug.Log("weapons[currentWeapon]: " + weapons[currentWeapon]);
        Debug.Log("weapons[currentWeapon].weaponModel: " + weapons[currentWeapon].weaponModel);

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weapons[currentWeapon].weaponModel.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weapons[currentWeapon].weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
        isAutomatic = weapons[currentWeapon].isAutomatic;
        weaponDamage = weapons[currentWeapon].weaponDamage;
        attackRate = weapons[currentWeapon].attackRate;
        range = weapons[currentWeapon].range;
        shootPosOffset = weapons[currentWeapon].shootPosOffset;
        UpdateTrail(weapons[currentWeapon].trailColour, weapons[currentWeapon].trailThickness, weapons[currentWeapon].trailDuration);
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            weapons[currentWeapon].currentAmmo = weapons[currentWeapon].magSize;
            UpdateHPUI();
        }
    }

    void UpdateTrail(Color trailColour, float thickness, float duration)
    {
        lineRenderer.startColor = trailColour;
        lineRenderer.endColor = trailColour;
        lineRenderer.startWidth = thickness;
        lineRenderer.endWidth = thickness;
        trailDuration = duration;
    }

    void SetTrailPoints(Vector3 endpt)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, weaponModel.transform.position + shootPosOffset);
        lineRenderer.SetPosition(1, endpt);
    }

    IEnumerator showTrail()
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(trailDuration);
        lineRenderer.enabled = false;
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
}
