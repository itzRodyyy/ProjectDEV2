using UnityEngine;

public class inventoryManager : MonoBehaviour
{
    public static inventoryManager instance;
    public inventorySlot[] slots;
    public int selectedSlot = 0;

    void Awake()
    {
        instance = this;
    }

    public void AddItem(weaponStats newItem)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(newItem);

                if (CountFilledSlots() == 1)
                {
                    SelectSlot(i);
                    
                }

                return;
            }
        }
    }
    private int CountFilledSlots()
    {
        int count = 0;
        foreach (var slot in slots)
        {
            if (slot.item != null)
                count++;
        }
        return count;
    }
    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
        {
            return;
        }

        if (slots[index].item == null)
        {
            return;
        }

        slots[selectedSlot].SetHighlight(false);
        selectedSlot = index;
        slots[selectedSlot].SetHighlight(true);
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= slots.Length)
        {
            return;
        }

        slots[index].ClearSlot();
    }


}