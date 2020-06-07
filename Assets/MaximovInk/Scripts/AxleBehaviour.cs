using UnityEngine;

namespace MaximovInk
{
    public class AxleBehaviour : ObjectBehaviour
    {
        private BuildingLayer parent;
        private HingeJoint hJoint;

        private void Awake()
        {
            parent = GetComponentInParent<BuildingLayer>();
            if (parent == null)
            {
                return;
            }

            parent.OnSetKinematic += OnSetKinematic;
        }

        private void OnSetKinematic(bool isKinematic)
        {
            if (hJoint == null || hJoint.connectedBody == null)
                return;

            Debug.Log("set kinmeatic2");

            hJoint.connectedBody.isKinematic = isKinematic;
        }

        public override void OnBlockPreview(GameObject BlockPreview)
        {
            base.OnBlockPreview(BlockPreview);
            BlockPreview.transform.position = transform.position + (transform.up * BuildingLayer.HalfBlockSize);
            BlockPreview.transform.rotation = transform.rotation;
        }

        public override void OnObjectPreview(GameObject ObjectPreview)
        {
            base.OnObjectPreview(ObjectPreview);
            ObjectPreview.transform.position = transform.position + (transform.up * BuildingLayer.HalfBlockSize);
            ObjectPreview.transform.rotation = transform.rotation;
        }

        public override void OnBlockPlace(BlockTile tile)
        {
            /* if (parent == null)
             {
                 return;
             }

             base.OnBlockPlace(tile);

             var blockMesh = CreateL();
             blockMesh.AddBlock(Vector3Int.zero, tile);*/
        }

        public override void OnObjectPlace(ObjectTile objectShape)
        {
            /*base.OnObjectPlace(objectShape);

            if (objectShape.GetGameObject().GetComponent<AxleBehaviour>() != null)
                return;

            var blockMesh = CreateL();
            blockMesh.AddObject(objectShape, Vector3Int.zero, Vector3Int.up);*/
        }

        /*
        private BuildingLayer CreateL()
        {
            var blockMesh = new GameObject().AddComponent<BuildingLayer>();

            print("create mesh " + parent.name);

            parent.OnSetKinematic += (value) => { blockMesh.IsKinematic = value; Debug.Log("set kinmeatic1"); };
            parent.OnSetAVelocity += (value) => blockMesh.AngularVelocity = value;
            parent.OnSetVelocity += (value) => blockMesh.Velocity = value;
            parent.OnSetPreview += (value) => { blockMesh.SetPreview(value); Debug.Log("set preview"); };
            parent.OnLayerChange += (value) => { blockMesh.SetLayer(value); Debug.Log("set layer"); };

            blockMesh.transform.position = transform.position + (transform.up * BlockMesh.HalfBlockSize);
            blockMesh.transform.rotation = transform.rotation;

            hJoint = parent.gameObject.AddComponent<HingeJoint>();
            blockMesh.ConnectJointToThis(hJoint);
            blockMesh.IsKinematic = parent.IsKinematic;

            hJoint.anchor = transform.localPosition;
            hJoint.axis = parent.transform.InverseTransformDirection(transform.TransformDirection(Vector3.up));
            hJoint.autoConfigureConnectedAnchor = false;
            hJoint.connectedAnchor = Vector3.one * BlockMesh.HalfBlockSize;

            return blockMesh;
        }*/
    }
}