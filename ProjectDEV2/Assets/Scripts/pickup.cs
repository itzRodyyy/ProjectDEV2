using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] weaponStats weapon;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;

    private inventorymanager inventoryManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weapon.currentAmmo = weapon.magSize;
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<inventorymanager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        iPickup pickupable = other.GetComponent<iPickup>();
        if (pickupable != null && other.CompareTag("Player"))
        {
            inventoryManager.AddItem(itemName, itemIcon, weapon);
            pickupable.GetWeaponStats(weapon);
            Destroy(gameObject);
        }
    }
}
