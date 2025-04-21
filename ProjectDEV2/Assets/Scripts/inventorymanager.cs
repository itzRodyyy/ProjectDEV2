using UnityEngine;

public class inventorymanager : MonoBehaviour
{
    public static inventorymanager instance;

    [SerializeField] GameObject menuInventory;
    bool isInventoryOpen = false;
    public itemSlot[] itemslot;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            inventory();
        }
    }

    public void inventory()
    {
        isInventoryOpen = !isInventoryOpen;

        menuInventory.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            GameManager.instance.statePause();
        }
        else
        {
            GameManager.instance.stateUnpause();
        }
    }

    public void AddItem(string itemName, int quantity, Sprite itemIcon)
    {
        for (int i = 0; i < itemslot.Length; i++)
        {
            if (itemslot[i].isFull == false)
            {
                itemslot[i].AddItem(itemName, quantity, itemIcon);
                return;
            }
        }

    }
}

