using System.Collections;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] AudioSource aud;
    [SerializeField] CharacterController controller;
    public movementStats moveStats;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    public bool onLadder = false;

    int jumpCount;

    Vector3 moveDir; // Movement
    Vector3 playerVel; // Movement

    bool isSprinting; // Movement
    bool isPlayingStep; // Movement
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement() // Movement
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

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * (onLadder ? transform.up : transform.forward));
        controller.Move(moveDir * moveStats.moveSpeed * Time.deltaTime);

        Jump();
        Sprint();
        Crouch();

        playerVel.y -= (onLadder ? 0 : moveStats.gravity) * Time.deltaTime;

        controller.Move(playerVel * Time.deltaTime);
    }

    void Jump() // Movement
    {
        if (Input.GetButtonDown("Jump") && jumpCount < moveStats.jumpMax)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            jumpCount++;
            playerVel.y = moveStats.jumpSpeed;
        }
    }

    void Crouch() // Movement
    {
        if (Input.GetButtonDown("Crouch"))
        {
            controller.height = Mathf.RoundToInt(controller.height * moveStats.crouchHeightMod);
            moveStats.moveSpeed = Mathf.RoundToInt(moveStats.moveSpeed * moveStats.crouchSpeedMod);
        }
        if (Input.GetButtonUp("Crouch"))
        {
            controller.height = Mathf.RoundToInt(controller.height / moveStats.crouchHeightMod);
            moveStats.moveSpeed = Mathf.RoundToInt(moveStats.moveSpeed / moveStats.crouchSpeedMod);
        }
    }

    void Sprint() // Movement
    {
        if (Input.GetButtonDown("Sprint"))
        {
            moveStats.moveSpeed *= moveStats.sprintMod;
            isSprinting = true;
        }
        if (Input.GetButtonUp("Sprint"))
        {
            moveStats.moveSpeed /= moveStats.sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator playStep() // Movement
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
