using System;
using UnityEngine;

namespace MaximovInk
{
    public class AxleBehaviour : ObjectBehaviour
    {
        //private HingeJoint hJoint;
        private ConfigurableJoint cJoint;

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
            //hJoint = buildingLayer.gameObject.AddComponent<HingeJoint>();
            cJoint = buildingLayer.gameObject.AddComponent<ConfigurableJoint>();
            //hJoint.autoConfigureConnectedAnchor = false;
            //connectedTo.ConnectJointToThis(hJoint);
            connectedTo.ConnectJointToThis(cJoint);
            //hJoint.anchor = buildingLayer.GridToLocal(data.Position);
            cJoint.anchor = buildingLayer.GridToLocal(data.Position);
            //hJoint.axis = buildingLayer.transform.InverseTransformDirection(transform.up);
            cJoint.axis = buildingLayer.transform.InverseTransformDirection(transform.up);
            // hJoint.connectedAnchor = Vector3.one * BuildingLayer.HalfBlockSize;
            cJoint.angularXMotion = ConfigurableJointMotion.Free;
            cJoint.angularYMotion = ConfigurableJointMotion.Locked;
            cJoint.angularZMotion = ConfigurableJointMotion.Locked;

            cJoint.xMotion = ConfigurableJointMotion.Locked;
            cJoint.yMotion = ConfigurableJointMotion.Locked;
            cJoint.zMotion = ConfigurableJointMotion.Locked;
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