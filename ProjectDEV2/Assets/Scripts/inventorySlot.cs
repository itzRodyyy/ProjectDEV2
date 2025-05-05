using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class inventorySlot : MonoBehaviour
{
    public Image icon;
    public weaponStats item;
    public GameObject highlight;

    public void AddItem(weaponStats newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        highlight.SetActive(false);
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        highlight.SetActive(false);
    }

    public void SetHighlight(bool on)
    {
        if (highlight != null)
        { 
        highlight.SetActive(on);
        }
    }
}

