using MessagePack;
using System.Collections.Generic;
using UnityEngine;

namespace MaximovInk
{
    [MessagePackObject]
    public class BlockTileData
    {
        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public Vector3Int Position { get; set; }

        [Key(2)]
        public Dictionary<string, object> parameters { get; set; }
            = new Dictionary<string, object>();
    }

    public struct BlockTile
    {
        public string Name;
        public string MaterialName;
        public Vector4 UV;
        public Vector2 TillingFactor;

        public override bool Equals(object obj)
        {
            if (!(obj is BlockTile))
                return false;

            return ((BlockTile)obj).Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            return 17 * Name.GetHashCode();
        }

        public static bool operator ==(BlockTile left, BlockTile right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BlockTile left, BlockTile right)
        {
            return !(left == right);
        }
    }
}