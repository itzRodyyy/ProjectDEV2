using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] weaponStats weapon;
    void Start()
    {
        weapon.currentAmmo = weapon.magSize;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        iPickup pickupable = other.GetComponent<iPickup>();

        if (pickupable != null)
        { 
            pickupable.GetWeaponStats(weapon);
            Destroy(gameObject);
        }
    }   
}
