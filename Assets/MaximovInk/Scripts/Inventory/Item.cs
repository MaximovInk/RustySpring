using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MaximovInk
{
    public class Item
    {
        public string Name { get; set; }
        public Texture2D Icon { get; set; }
        public Texture2D DroppedIcon { get; set; }
        public string Description { get; set; }
        public int MaxStack { get; set; }

        public bool CanDrop { get; set; } = true;

        private readonly ItemProperty[] properties;

        public List<ItemFunction> functions = new List<ItemFunction>();

        public event Action<Slot> OnSelectSlot;

        public event Action<Slot> OnDeselectSlot;

        public ItemProperty GetProperty(string key)
        {
            return Array.Find(properties, n => n.Key == key);
        }

        public void AddProperty(ItemProperty property)
        {
            if (properties.Any(n => n.Key == property.Key))
                return;

            properties.Add(property);
        }

        public Item(params ItemProperty[] properties)
        {
            this.properties = properties;
        }
    }

    public class ItemProperty
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public bool AsBool() => (bool)Value;

        public string AsString() => (string)Value;

        public int AsInt() => (int)Value;

        public float AsFloat() => (float)Value;
    }

    public struct ItemFunction
    {
        public string Name { get; set; }
        public Action<ItemData> Action { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ItemFunction))
                return false;

            return Name
                == ((ItemFunction)obj).Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(ItemFunction left, ItemFunction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemFunction left, ItemFunction right)
        {
            return !(left == right);
        }
    }
}