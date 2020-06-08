using MessagePack;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaximovInk
{
    [MessagePackObject]
    public class ObjectTileData
    {
        [IgnoreMember]
        public GameObject Instance;

        [Key(3)]
        public Dictionary<string, object> parameters
            = new Dictionary<string, object>();

        public void AddParam(string key, object data)
        {
            CheckParams();

            parameters.Add(key, data);
        }

        public void SetOrAddParam(string key, object data)
        {
            CheckParams();

            if (parameters.ContainsKey(key))
            {
                parameters[key] = data;
            }
            else
            {
                AddParam(key, data);
            }
        }

        private void CheckParams()
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }
        }

        public void RemoveParam(string key)
        {
            CheckParams();

            parameters.Remove(key);
        }

        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public Vector3 Normal { get; set; }

        [Key(2)]
        public Vector3Int Position { get; set; }
    }

    public class ObjectTile
    {
        public string Name;

        public Func<GameObject> GetGameObject;

        public bool CanRotateX = true;
        public bool CanRotateY = true;
        public bool CanRotateZ = true;
    }
}