using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] WeaponDatabase weaponDB;
    private weaponStats weaponInstance;
    private bool isManuallyAssigned = false;
    void Start()
    {
        if (!isManuallyAssigned && weaponDB != null && weaponDB.allWeapons.Length > 0)
        {
            weaponStats selectedWeapon = weaponDB.allWeapons[Random.Range(0, weaponDB.allWeapons.Length)];
            weaponInstance = Instantiate(selectedWeapon);
            weaponInstance.currentAmmo = weaponInstance.magSize;
        }
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        iPickup pickupable = other.GetComponent<iPickup>();

        if (pickupable != null && other.CompareTag("Player"))
        {
            if (inventoryManager.instance != null)
            {
                inventoryManager.instance.AddItem(weaponInstance);
            }
            
            pickupable.GetWeaponStats(weaponInstance);
            Destroy(gameObject);
        }
    }

    public void AssignWeapon(weaponStats weapon)
    {
        weaponInstance = Instantiate(weapon);
        weaponInstance.currentAmmo = weapon.currentAmmo;
        isManuallyAssigned = true;
    }    
}
