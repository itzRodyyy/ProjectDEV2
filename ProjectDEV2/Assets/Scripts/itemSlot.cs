//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class itemSlot : MonoBehaviour, IDropHandler
//{
//    // ITEM DATA
//    public string itemName;
//    public Sprite itemIcon;
//    public bool isFull;
//    public weaponStats storedItem;
//    private GameObject selectedHighlight;

//    // ITEM SLOT
//    [SerializeField] private Image itemImage;

//    private void Awake()
//    {
//        selectedHighlight = transform.Find("SelectedPanel").gameObject;
//        if (selectedHighlight != null)
//            selectedHighlight.SetActive(false);
//    }

//    public void AddItem(string itemName, Sprite itemIcon, weaponStats storedItem)
//    {
//        this.itemName = itemName;
//        this.itemIcon = itemIcon;
//        this.storedItem = storedItem;
//        isFull = true;

//        itemImage.sprite = itemIcon;
//    }

//    public void SetHighlight(bool isActive)
//    {
//        if (selectedHighlight != null)
//            selectedHighlight.SetActive(isActive);
//    }

//    public void OnDrop(PointerEventData eventData)
//    {
//        GameObject dropped = eventData.pointerDrag;
//        draggable draggableItem = dropped.GetComponent<draggable>();
//        draggableItem.parentAfterDrag = transform;
//    }
//}

