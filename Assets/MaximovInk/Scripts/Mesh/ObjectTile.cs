using System;
using UnityEngine;

namespace MaximovInk
{
    public class ObjectTile
    {
        public string Name;

        public Func<GameObject> GetGameObject;

        public bool CanRotateX = true;
        public bool CanRotateY = true;
        public bool CanRotateZ = true;
    }
}