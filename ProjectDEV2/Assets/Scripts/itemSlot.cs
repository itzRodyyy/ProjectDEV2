using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class itemSlot : MonoBehaviour
{
    // ITEM DATA
    public string itemName;
    public Sprite itemIcon;
    public bool isFull;

    // ITEM SLOT
    [SerializeField] private Image itemImage;

    public void AddItem(string itemName, Sprite itemIcon)
    {
        this.itemName = itemName;
        this.itemIcon = itemIcon;
        isFull = true;

        itemImage.sprite = itemIcon;
    }

}
