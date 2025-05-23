using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class playerCombat : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] AudioSource aud;
    [SerializeField] LayerMask ignoreLayer;
    public GameObject weaponModel;
    public weaponStats currentWeapon;

    float attackTimer;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime; // Combat
        if (currentWeapon != null) // Combat
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * currentWeapon.range);
        }

        Attack();
    }

    void Attack() // Combat
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
                    checkCollision();
                    aud.PlayOneShot(currentWeapon.shootSound[Random.Range(0, currentWeapon.shootSound.Length)], currentWeapon.shootSoundVolume);
                }
            }
        }

        reload();
    }

    private void checkCollision() // Combat
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, currentWeapon.range, ~ignoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                if (currentWeapon.isMelee)
                    dmg.TakeDamage(currentWeapon.weaponDamage + GameManager.instance.abilityMod(GameManager.instance.hp_stats_script.stats.strength));
                else
                    dmg.TakeDamage(currentWeapon.weaponDamage);
            }

            Debug.Log(hit.collider);

            Instantiate(currentWeapon.hitEffect, hit.point, Quaternion.identity);
        }
    }

    void reload() // Combat
    {
        if (Input.GetButtonDown("Reload") && currentWeapon != null)
        {
            currentWeapon.currentAmmo = currentWeapon.magSize;
            GameManager.instance.UpdateAmmoUI();
        }
    }
}
