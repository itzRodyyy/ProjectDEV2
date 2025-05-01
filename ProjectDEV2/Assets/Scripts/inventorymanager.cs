//using UnityEngine;

//public class inventorymanager : MonoBehaviour
//{
//    public static inventorymanager instance;

//    [SerializeField] GameObject menuInventory;
//    [SerializeField] GameObject hotBar;
//    bool isInventoryOpen = false;
//    public itemSlot[] itemslot;

//    public int currentWeapon = 0;
//    private const int maxHotbarSlots = 10;

//    void Awake()
//    {
//        if (instance == null)
//            instance = this;
//        else
//            Destroy(gameObject);
//    }

//    void Update()
//    {
//        if (Input.GetButtonDown("Inventory"))
//        {
//            inventory();
//        }

//        SwitchWeapon();
//    }

//    void SwitchWeapon()
//    {
//        if (itemslot.Length == 0)
//            return;

//        float scroll = Input.GetAxis("Mouse ScrollWheel");
//        if (scroll > 0f)
//        {
//            ScrollToNextWeapon();
//        }
//        else if (scroll < 0f)
//        {
//            ScrollToPreviousWeapon();
//        }

//        if (Input.GetKeyDown(KeyCode.Alpha1))
//            SelectWeapon(0);
//        if (Input.GetKeyDown(KeyCode.Alpha2))
//            SelectWeapon(1);
//        if (Input.GetKeyDown(KeyCode.Alpha3))
//            SelectWeapon(2);
//        if (Input.GetKeyDown(KeyCode.Alpha4))
//            SelectWeapon(3);
//        if (Input.GetKeyDown(KeyCode.Alpha5))
//            SelectWeapon(4);
//        if (Input.GetKeyDown(KeyCode.Alpha6))
//            SelectWeapon(5);
//        if (Input.GetKeyDown(KeyCode.Alpha7))
//            SelectWeapon(6);
//        if (Input.GetKeyDown(KeyCode.Alpha8))
//            SelectWeapon(7);
//        if (Input.GetKeyDown(KeyCode.Alpha9))
//            SelectWeapon(8);
//    }

//    void ScrollToNextWeapon()
//    {
//        int originalSlot = currentWeapon;
//        do
//        {
//            currentWeapon++;
//            if (currentWeapon >= maxHotbarSlots)
//                currentWeapon = 0;

//            if (itemslot[currentWeapon].isFull)
//            {
//                SelectWeapon(currentWeapon);
//                return;
//            }

//        } while (currentWeapon != originalSlot);
//    }

//    void ScrollToPreviousWeapon()
//    {
//        int originalSlot = currentWeapon;

//        do
//        {
//            currentWeapon--;
//            if (currentWeapon < 0)
//                currentWeapon = maxHotbarSlots - 1;

//            if (itemslot[currentWeapon].isFull)
//            {
//                SelectWeapon(currentWeapon);
//                return;
//            }

//        } while (currentWeapon != originalSlot);
//    }

//    public void inventory()
//    {
//        isInventoryOpen = !isInventoryOpen;

//        menuInventory.SetActive(isInventoryOpen);

//        if (isInventoryOpen)
//        {
//            GameManager.instance.statePause();
//            GameManager.instance.menuActive = menuInventory;
//            GameManager.instance.menuActive.SetActive(true);
//        }
//        else
//        {
//            GameManager.instance.stateUnpause();
//        }
//    }

//    public void AddItem(string itemName, Sprite itemIcon, weaponStats weapon)
//    {
//        for (int i = 0; i < itemslot.Length; i++)
//        {
//            if (itemslot[i].isFull == false)
//            {
//                itemslot[i].AddItem(itemName, itemIcon, weapon);
//                return;
//            }
//        }

//    }

//    public void SelectWeapon(int slot)
//    {
//        if (slot < 0 || slot >= maxHotbarSlots)
//            return;

//        if (itemslot[slot].isFull)
//        {
//            currentWeapon = slot;
//            weaponStats selectedWeapon = itemslot[slot].storedItem;

//            GameManager.instance.player.GetComponent<playerController>().GetWeaponStats(selectedWeapon);

//            UpdateSlotHighlights();
//        }
//    }

//    void UpdateSlotHighlights()
//    {
//        for (int i = 0; i < maxHotbarSlots; i++)
//        {
//            if (i == currentWeapon && itemslot[i].isFull)
//            {
//                itemslot[i].SetHighlight(true);
//            }
//            else
//            {
//                itemslot[i].SetHighlight(false);
//            }
//        }
//    }

//}

