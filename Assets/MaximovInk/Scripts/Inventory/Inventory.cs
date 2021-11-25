using UnityEngine;

    public class Inventory : MonoBehaviour
    {
    private Slot[] slots = new Slot[0];

    public bool Selectable;

    public int SelectedSlot
    {
        get => selectedSlot;
        set
        {
            if (!Selectable)
                return;
            var lastSelected = selectedSlot;
            selectedSlot = value;
            UpdateSelectables(lastSelected);
        }
    }
    private int selectedSlot = -1;

    private void UpdateSelectables(int lastSelected)
    {
        if (lastSelected >= 0 && lastSelected < slots.Length)
        {
            slots[lastSelected].Deselect();
        }
        if (selectedSlot >= 0 && lastSelected < slots.Length)
        {
            slots[selectedSlot].Select();
        }
    }

    private void Awake()
    {
        slots = GetComponentsInChildren<Slot>();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].id = i;
        }
    }

    public void AddItem(ItemData itemData)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].ItemData == null)
            {
                slots[i].ItemData = itemData;
                return;
            }
        }
    }
}

