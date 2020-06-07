using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MaximovInk
{
    public static class TileDatabase
    {
        private static readonly Dictionary<string, BlockTile> blocks = new Dictionary<string, BlockTile>();
        private static readonly Dictionary<string, ObjectTile> objects = new Dictionary<string, ObjectTile>();

        public static void RegisterBlock(BlockTile blockTile, bool replace = false)
        {
            if (blocks.ContainsKey(blockTile.Name))
            {
                if (!replace)
                {
                    Debug.LogError("Tile is already registered in database:" + blockTile.Name);
                }
                else
                {
                    blocks[blockTile.Name] = blockTile;
                }

                return;
            }

            blocks.Add(blockTile.Name, blockTile);
        }

        public static void RegisterObject(ObjectTile objectTile, bool replace = false)
        {
            if (objects.ContainsKey(objectTile.Name))
            {
                if (!replace)
                {
                    Debug.LogError("Tile is already registered in database:" + objectTile.Name);
                }
                else
                {
                    objects[objectTile.Name] = objectTile;
                }

                return;
            }

            objects.Add(objectTile.Name, objectTile);
        }

        public static List<string> GetAllBlocks()
        {
            return blocks.Keys.ToList();
        }

        public static BlockTile GetBlock(string name)
        {
            return blocks[name];
        }

        public static ObjectTile GetObject(string name)
        {
            return objects[name];
        }

        static TileDatabase()
        {
            RegisterBlock(new BlockTile() { UV = new Vector4(0, 0, 1, 1), Name = "wood", MaterialName = "wood", TillingFactor = Vector2.one });
            RegisterBlock(new BlockTile() { UV = new Vector4(0, 0, 1, 1), Name = "metal", MaterialName = "metal", TillingFactor = Vector2.one });
            RegisterBlock(new BlockTile() { UV = new Vector4(0, 0, 1, 1), Name = "metal_old", MaterialName = "metal_old", TillingFactor = Vector2.one });
        }
    }
}