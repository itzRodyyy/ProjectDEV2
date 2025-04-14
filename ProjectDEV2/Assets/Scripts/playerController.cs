using UnityEngine;

public class playerController : MonoBehaviour
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

    // Shooting
    [SerializeField] float shootRate;
    [SerializeField] int shootDmg;
    [SerializeField] int shootDist;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject projectile;

    int jumpCount;
    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist);
        Movement();
        shootTimer += Time.deltaTime;
        Shoot();
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


}
