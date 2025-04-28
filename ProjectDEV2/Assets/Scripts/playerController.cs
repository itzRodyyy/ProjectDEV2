using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class playerController : MonoBehaviour, IDamage, iPickup
{
    // Essentials
    [Header("--- Components ---")]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    // Movement
    [Header("--- Movement ---")]
    [SerializeField] int moveSpeed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] float crouchHeightMod;
    [SerializeField] float crouchSpeedMod;

    // Shooting
    [Header("--- Gun ---")]
    [SerializeField] GameObject weaponModel;
    bool isMelee;
    bool isAutomatic;
    int weaponDamage;
    float attackRate;
    int range;
    int currentAmmo;
    int magSize;
    Transform shootPos;


    // Stats
    [Header("--- Player Stats ---")]
    public int HP;

    int HPOrig;
    int jumpCount;
    float attackTimer;

    Vector3 moveDir;
    Vector3 playerVel;


    // HP Bar UI
    public Slider hpBarSlider;
    public TextMeshProUGUI hpBarValueText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        UpdateHPUI();
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range);
        Movement();
        Attack();

    }

    public void UpdateHPUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    void Movement()
    {
        if (controller.isGrounded)
        {
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
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;
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
        }
        if (Input.GetButtonUp("Sprint"))
        {
            moveSpeed /= sprintMod; 
        }
    }

    void Attack()
    {
        if (isAutomatic)
        {
            if (Input.GetButton("Fire1") && attackTimer > attackRate && currentAmmo > 0)
            {
                attackTimer = 0;
                checkCollision();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && attackTimer > attackRate && currentAmmo > 0)
            {
                attackTimer = 0;
                checkCollision();
            }
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
        }
    }

    public void GetWeaponStats(weaponStats weapon)
    {
        weaponModel.GetComponent<MeshFilter>().sharedMesh = weapon.weaponModel.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weapon.weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
        isMelee = weapon.isMelee;
        isAutomatic = weapon.isAutomatic;
        weaponDamage = weapon.weaponDamage;
        attackRate = weapon.attackRate;
        range = weapon.range;
        currentAmmo = weapon.currentAmmo;
        magSize = weapon.magSize;
        shootPos = weapon.shootPos;
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            currentAmmo = magSize;
        }
    }
}
