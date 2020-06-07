using UnityEngine;

namespace MaximovInk
{
    public class ObjectBehaviour : MonoBehaviour
    {
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
    }
}