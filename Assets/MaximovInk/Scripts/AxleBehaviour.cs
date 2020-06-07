using System;
using UnityEngine;

namespace MaximovInk
{
    public class AxleBehaviour : ObjectBehaviour
    {
        private HingeJoint hJoint;
        private BuildingLayer connectedTo;

        private void Awake()
        {
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
            connectedTo = buildingLayer.Building.AddNewLayer();

            connectedTo.AddBlock(data.Position, tile);

            ConfigurateJoint();

            data.AddParam("connectedTo", buildingLayer.Building.layers.IndexOf(connectedTo));

            base.OnBlockPlace(tile);
        }

        private void ConfigurateJoint()
        {
            hJoint = buildingLayer.gameObject.AddComponent<HingeJoint>();
            hJoint.autoConfigureConnectedAnchor = false;
            connectedTo.ConnectJointToThis(hJoint);
            hJoint.anchor = transform.localPosition;
            hJoint.axis = buildingLayer.transform.InverseTransformDirection(transform.TransformDirection(Vector3.up));
            hJoint.connectedAnchor = Vector3.one * BuildingLayer.HalfBlockSize;
        }

        public override void OnDeserialize()
        {
            base.OnDeserialize();
            if (data.parameters?.ContainsKey("connectedTo") == true)
            {
                connectedTo = buildingLayer.Building.layers[Convert.ToInt32(data.parameters["connectedTo"])];

                ConfigurateJoint();
            }
        }
    }
}