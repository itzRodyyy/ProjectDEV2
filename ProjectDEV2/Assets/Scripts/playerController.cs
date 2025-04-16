using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] float crouchHeight;
    [SerializeField] float standingHeight;
    [SerializeField] float crouchSpeedMod;

    // Shooting
    [SerializeField] float shootRate;
    [SerializeField] int shootDmg;
    [SerializeField] int shootDist;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject projectile;

    // Stats
    [SerializeField] int HP;

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
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist);

        Movement();

        // HP Bar UI
        hpBarValueText.text = HP.ToString() + "/" + HPOrig.ToString();

        hpBarSlider.value = HP;
        hpBarSlider.maxValue = HPOrig;

    }

    void Movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(moveDir * speed * Time.deltaTime);

        Jump();

        if (Input.GetButtonDown("Crouch"))
        {
            crouch(true);
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch(false);
        }

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void sprint()
    {

        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void crouch(bool crouch)
    {
        isCrouching = crouch;

        controller.height = crouch ? crouchHeight : standingHeight;

        if (crouch)
        {
            speed = Mathf.RoundToInt(speed * crouchSpeedMod);
        }
        else
        {
            speed = Mathf.RoundToInt(speed / crouchSpeedMod);
        }
    }

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if(HP <= 0)
        {
            // You died
            GameManager.instance.youDied();
        }
    }
}
