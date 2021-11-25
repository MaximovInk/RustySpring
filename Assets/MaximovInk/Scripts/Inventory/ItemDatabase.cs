
using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabase
{
    private static readonly Dictionary<string, Item> items = new Dictionary<string, Item>();

    public static void RegisterItem(Item item, bool replace = false)
    {
        if (item == null)
        {
            throw new System.Exception("Cannot register block , because its null");
        }

        if (items.ContainsKey(item.Name))
        {
            if (!replace)
            {
                Debug.LogError("Tile is already registered in database:" + item.Name);
            }
            else
            {
                items[item.Name] = item;
            }

            return;
        }

        items.Add(item.Name, item);
    }

    public static Item GetItem(string name)
    {
        return items[name];
    }

    static ItemDatabase()
    {
        RegisterItem(new Item()
        {
            Name = "Lift",
            Image = Resources.Load<Sprite>("Sprites/cog"),
            OnHotbarDeselect = LiftPlacer.TerminatePlace,
            OnHotbarSelect = LiftPlacer.BeginPlace
        }) ;
    }
}
