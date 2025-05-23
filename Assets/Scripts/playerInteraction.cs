using UnityEngine;

public class playerInteraction : MonoBehaviour, iAmmoPickup, iPickup
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] CharacterController controller;
    [Range(5, 10)][SerializeField] int interactRange;
    [SerializeField] float knockbackDuration = 0.2f; // Interaction
    [SerializeField] float knockbackForce = 8f; // Interaction

    float knockbackTimer;

    Vector3 knockbackDirection;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Interact();
    }

    void Knockback()
    {
        if (knockbackTimer > 0)
        {
            controller.Move(knockbackDirection * Time.deltaTime);
            knockbackTimer -= Time.deltaTime;
        }
    }

    public void GetWeaponStats(weaponStats weapon) // Interaction
    {
        GameManager.instance.combat_script.currentWeapon = weapon;
        GameManager.instance.combat_script.weaponModel.GetComponent<MeshFilter>().sharedMesh = GameManager.instance.combat_script.currentWeapon.weaponModel.GetComponent<MeshFilter>().sharedMesh;
        GameManager.instance.combat_script.weaponModel.GetComponent<MeshRenderer>().sharedMaterial = GameManager.instance.combat_script.currentWeapon.weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
        if (weapon.isMelee && GameManager.instance.ammoActive)
            GameManager.instance.ToggleAmmoUI(); 
        else if (!weapon.isMelee && GameManager.instance.ammoActive)
            GameManager.instance.UpdateAmmoUI();
        else if (!weapon.isMelee && !GameManager.instance.ammoActive)
        {
            GameManager.instance.ToggleAmmoUI();
            GameManager.instance.UpdateAmmoUI();
        }
    }

    public void addAmmo(int amount, GameManager.AmmoType ammoType) // Interaction
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

    public void Interact() // Interaction
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

    public void ApplyKnockback(Vector3 source, float force) // Interaction
    {
        Vector3 direction = (transform.position - source).normalized;
        direction.y = 0f;

        knockbackDirection = direction * force;
        knockbackTimer = knockbackDuration;
    }
}
