using UnityEngine;

public class playerInteraction : MonoBehaviour, iPickup
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
