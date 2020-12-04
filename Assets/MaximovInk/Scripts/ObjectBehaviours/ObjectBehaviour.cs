using System.Collections.Generic;
using System;
using UnityEngine;

namespace MaximovInk
{
    public class ObjectBehaviour : MonoBehaviour
    {
        protected BuildingLayer buildingLayer;
        protected ObjectTileData data;

        protected List<InPoint> inPoints = new List<InPoint>();
        protected List<OutPoint> outPoints = new List<OutPoint>();

        public const float kPointSize = 0.25f;

        protected InPoint AddInput(string name, string type, Vector3Int position)
        {
            var point = new GameObject(name).AddComponent<InPoint>();

            InitPoint(point, name, type, position);

            return point;
        }

        protected OutPoint AddOutput(string name, string type, Vector3Int position)
        {
            var point = new GameObject(name).AddComponent<OutPoint>();

            InitPoint(point, name, type, position);

            return point;
        }

        private void InitPoint(Point point, string name, string type, Vector3Int position)
        {
            point.Name = name;
            point.Type = type;
            point.gameObject.layer = LayerMask.NameToLayer("Connections");

            point.transform.localScale = Vector3.one * kPointSize;
            point.transform.parent = transform;
            point.transform.localPosition = ((Vector3)position * kPointSize);
            point.transform.localRotation = Quaternion.identity;
            point.InitGraphic();
        }

        public virtual void RemoveObject()
        {
            buildingLayer.RemoveObject(data);
        }

        public virtual void OnInstantiate(BuildingLayer buildingLayer, ObjectTileData data)
        {
            this.buildingLayer = buildingLayer;
            this.data = data;
        }

        public virtual void OnBlockPreview(GameObject BlockPreview)
        {
        }

        public virtual void OnBlockPlace(BlockTile tile)
        {
        }

        public virtual void OnObjectPreview(GameObject ObjectPreview)
        {
        }

        public virtual void OnObjectPlace(ObjectTile objectShape)
        {
        }

        public virtual void OnSerialize()
        {
        }

        public virtual void OnDeserialize()
        {
        }
    }
}