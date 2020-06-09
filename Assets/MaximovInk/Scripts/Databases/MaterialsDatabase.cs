using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MaximovInk
{
    public static class MaterialsDatabase
    {
        private static readonly Dictionary<string, Material> materials = new Dictionary<string, Material>();

        public static void RegisterMaterial(Material material, bool replace = false)
        {
            if (material == null)
            {
                throw new System.Exception("Cannot register material , because its null");
            }
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

        static MaterialsDatabase()
        {
            RegisterMaterial(Resources.Load<Material>("Materials/wood"));
            RegisterMaterial(Resources.Load<Material>("Materials/metal"));
            RegisterMaterial(Resources.Load<Material>("Materials/preview"));
            RegisterMaterial(Resources.Load<Material>("Materials/metal_old"));
            RegisterMaterial(Resources.Load<Material>("Materials/cutout"));
            RegisterMaterial(Resources.Load<Material>("Materials/flow"));
        }
    }
}