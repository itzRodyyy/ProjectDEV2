using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] weaponStats weapon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weapon.currentAmmo = weapon.magSize;
    }

    // Update is called once per frame
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
