using UnityEngine;

public class inventorymanager : MonoBehaviour
{
    public static inventorymanager instance;

    [SerializeField] GameObject menuInventory;
    [SerializeField] GameObject hotBar;
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
            GameManager.instance.menuActive = menuInventory;
            GameManager.instance.menuActive.SetActive(true);
        }
        else
        {
            GameManager.instance.stateUnpause();
        }
    }

    public void AddItem(string itemName, Sprite itemIcon)
    {
        for (int i = 0; i < itemslot.Length; i++)
        {
            if (itemslot[i].isFull == false)
            {
                itemslot[i].AddItem(itemName, itemIcon);
                return;
            }
        }

    }
}

