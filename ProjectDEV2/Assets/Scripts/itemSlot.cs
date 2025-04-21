using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class itemSlot : MonoBehaviour
{
    // ITEM DATA
    public string itemName;
    public int quantity;
    public Sprite itemIcon;
    public bool isFull;

    // ITEM SLOT
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;

    public void AddItem(string itemName, int quantity, Sprite itemIcon)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemIcon = itemIcon;
        isFull = true;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemIcon;
    }

}
