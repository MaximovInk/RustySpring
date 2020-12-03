using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MaximovInk
{
    public static class ItemDatabase
    {
        private static readonly Dictionary<string, Item> items = new Dictionary<string, Item>();

        public static void RegisterItem(Item item, bool replace = false)
        {
            if (item == null)
            {
                throw new System.Exception("Cannot register item , because its null");
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

        public static List<string> GetAllItems()
        {
            return items.Keys.ToList();
        }

        public static Item GetBlock(string name)
        {
            return items[name];
        }

        static ItemDatabase()
        {
            RegisterItem(new Item()
            {
                Name = "Gun",
                Description = "No Desc",
                MaxStack = 256,
                CanDrop = true
            });

            RegisterItem(new Item()
            {
                Name = "Wood block",
                Description = "Not bad",
                MaxStack = 256,
                CanDrop = true
            });
        }
    }
}