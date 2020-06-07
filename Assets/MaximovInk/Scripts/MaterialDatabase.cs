using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MaximovInk
{
    public static class MaterialDatabase
    {
        private static readonly Dictionary<string, Material> materials = new Dictionary<string, Material>();

        public static void RegisterMaterial(Material material, bool replace = false)
        {
            if (materials.ContainsKey(material.name))
            {
                if (!replace)
                {
                    Debug.LogError("Material is already registered in database:" + material.name);
                }
                else
                {
                    materials[material.name] = material;
                }

                return;
            }
            materials.Add(material.name, material);
        }

        public static Material GetMaterial(string name)
        {
            return new Material(materials[name]);
        }

        static MaterialDatabase()
        {
            RegisterMaterial(Resources.Load<Material>("Materials/wood"));
            RegisterMaterial(Resources.Load<Material>("Materials/metal"));
            RegisterMaterial(Resources.Load<Material>("Materials/preview"));
            RegisterMaterial(Resources.Load<Material>("Materials/metal_old"));
        }
    }
}