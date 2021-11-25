using MaximovInk;
using MaximovInk.UI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData ItemData { get { return itemData; } set { itemData = value; UpdateInfo(); } }
    private ItemData itemData;
    private Inventory parent;

    public int id;

    private static Image MovingItem { get { if (movingItem == null) InitMovingItem(); return movingItem; } }
    private static Image movingItem;
    private static void InitMovingItem()
    {
        var go = new GameObject();
        var canvas = new GameObject().AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.gameObject.name = "Canvas";
        canvas.sortingOrder = 999;
        movingItem = new GameObject().AddComponent<Image>();
        movingItem.transform.SetParent(canvas.transform);
        movingItem.gameObject.name = "MovingItem";
        movingItem.gameObject.SetActive(false);
        movingItem.preserveAspect = true;
    }

    private static Slot begin;

    private Image icon;
    private UIEventColor EventColor;

    private bool isSelected;

    private void Awake()
    {
        icon = transform.GetChild(0).GetComponent<Image>();
        parent = GetComponentInParent<Inventory>();
        EventColor = GetComponent<UIEventColor>();
    }

    private void UpdateInfo() {

        if (ItemData != null)
            icon.sprite = ItemDatabase.GetItem(ItemData.ID).Image;
        else
            icon.sprite = null;

        var oldKey = EventColor.key;
        EventColor.key = isSelected ? "slot_selected" : "slot";

        icon.color = icon.sprite == null ? Color.clear : (isSelected?Color.yellow : Color.white);

        if(oldKey != EventColor.key)
            UIColorManager.instance.UpdateChange();
    }

    public void Select()
    {
        isSelected = true;
        if (ItemData != null)
            ItemDatabase.GetItem(ItemData.ID).OnHotbarSelect();
        UpdateInfo();
    }

    public void Deselect()
    {
        isSelected = false;
        if (ItemData != null)
            ItemDatabase.GetItem(ItemData.ID).OnHotbarDeselect();
        UpdateInfo();

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        parent.SelectedSlot = id;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        MovingItem.gameObject.SetActive(false);

        var end = eventData.pointerCurrentRaycast.gameObject?.GetComponent<Slot>();


        if (begin != null && end != null && begin != end)
        {
            Swap(begin, end);
        }
        begin = null;
    }
    private void Swap(Slot a, Slot b)
    {
        if (a == b)
            return;

        var temp = a.ItemData;
        if(parent.SelectedSlot == a.id || parent.SelectedSlot == b.id)
        {
            parent.SelectedSlot = -1;
        }

        a.ItemData = b.ItemData;
        b.ItemData = temp;
    }
    public void OnDrag(PointerEventData eventData)
    {
        MovingItem.transform.position = Input.mousePosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ItemData == null)
            return;

        MovingItem.gameObject.SetActive(true);
        MovingItem.sprite = ItemDatabase.GetItem(ItemData.ID).Image;
        begin = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

}
