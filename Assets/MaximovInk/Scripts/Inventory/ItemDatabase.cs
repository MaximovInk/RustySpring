using System.Collections.Generic;

namespace MaximovInk
{
    public static class ItemDatabase
    {
        public static List<StaticItemData> items = new List<StaticItemData>();

        static ItemDatabase()
        {
            var lift = new StaticItemData()
            {
                Name = "Lift",
                Key = "Lift",
                Description = "Freeze your building on lift",
                MaxStack = 1,
                CanDrop = false
            };

            items.Add(lift);

            lift.OnSelectSlot += (slot) => { };

            items.Add(new StaticItemData()
            {
                Name = "Wood block",
                Key = "Default:Wood_block",
                Description = "Not bad",
                MaxStack = 256,
                CanDrop = true
            });
        }
    }
}