using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MaximovInk
{
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