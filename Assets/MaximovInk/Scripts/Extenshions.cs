using System;
using UnityEngine;

namespace MaximovInk
{
    public static class Extenshions
    {
        public static T[] Add<T>(this T[] target, params T[] items)
        {
            // Validate the parameters
            if (target == null)
            {
                target = Array.Empty<T>();
            }
            if (items == null)
            {
                items = Array.Empty<T>();
            }

            // Join the arrays
            T[] result = new T[target.Length + items.Length];
            target.CopyTo(result, 0);
            items.CopyTo(result, target.Length);
            return result;
        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static void SetMaterialRecursively(this GameObject obj, Material mat)
        {
            var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = mat;
            }
        }
    }
}