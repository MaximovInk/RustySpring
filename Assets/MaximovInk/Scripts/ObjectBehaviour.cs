using UnityEngine;

namespace MaximovInk
{
    public class ObjectBehaviour : MonoBehaviour
    {
        protected BuildingLayer buildingLayer;
        protected ObjectTileData data;

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