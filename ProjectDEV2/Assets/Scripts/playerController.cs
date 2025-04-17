using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class playerController : MonoBehaviour, IDamage
{
    // Essentials
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;
    
    // Movement
    [SerializeField] int moveSpeed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] float crouchHeightMod;
    [SerializeField] float crouchSpeedMod;

    // Shooting
    [SerializeField] float shootRate;
    [SerializeField] int shootDmg;
    [SerializeField] int shootDist;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject projectile;

    // Stats
    [SerializeField] public int HP;

    int HPOrig;
    int jumpCount;
    float shootTimer;

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
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist);
        Movement();
        shootTimer += Time.deltaTime;
        Shoot();

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

    void Shoot()
    {
        if (Input.GetButtonDown("Fire1") && shootTimer >= shootRate)
        {
            shootTimer = 0;

            Instantiate(projectile, shootPos.position, Camera.main.transform.rotation);
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

    IEnumerator flashDamage()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }
}
