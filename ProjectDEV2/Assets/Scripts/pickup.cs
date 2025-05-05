using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] WeaponDatabase weaponDB;
    private weaponStats weaponInstance;
    void Start()
    {
        weaponStats selectedWeapon = weaponDB.allWeapons[Random.Range(0, weaponDB.allWeapons.Length)];
        weaponInstance = Instantiate(selectedWeapon);
        weaponInstance.currentAmmo = weaponInstance.magSize;
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
}
