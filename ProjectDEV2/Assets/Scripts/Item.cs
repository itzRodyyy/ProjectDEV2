using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;

    [SerializeField] private int quantity;

    [SerializeField] private Sprite icon;

    private inventorymanager inventoryManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<inventorymanager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inventoryManager.AddItem(itemName, quantity, icon);
            Destroy(gameObject);
        }
    }
}
