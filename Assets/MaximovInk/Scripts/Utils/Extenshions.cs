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

        public static Transform FindParentByNamePart(this Transform transform, string part)
        {
            if (transform.gameObject.name.Contains("part"))
                return transform;

            if (transform.parent != null)
            {
                return FindParentByNamePart(transform.parent, part);
            }
            return null;
        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static float AngleInRad(this Vector3 vec1, Vector3 vec2)
        {
            return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
        }

        public static float AngleInDeg(this Vector3 vec1, Vector3 vec2)
        {
            return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
        }

        public static void SetTagRecursively(this GameObject obj, string tag)
        {
            obj.tag = tag;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetTagRecursively(tag);
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