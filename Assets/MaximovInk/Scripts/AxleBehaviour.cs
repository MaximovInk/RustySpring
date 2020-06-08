using System;
using UnityEngine;

namespace MaximovInk
{
    public class AxleBehaviour : ObjectBehaviour
    {
        private ConfigurableJoint cJoint;
        private BuildingLayer connectedTo;

        public override void OnInstantiate(BuildingLayer buildingLayer, ObjectTileData data)
        {
            base.OnInstantiate(buildingLayer, data);
            buildingLayer.Building.OnLayerAdd += (_) => UpdateData();
            buildingLayer.Building.OnLayerRemove += (_) => UpdateData();
            buildingLayer.CanCombineCallback += CanCombineCallback;
        }

        private void CanCombineCallback(BuildingLayer other, ref bool can)
        {
            can &= connectedTo != other;
        }

        private void UpdateData()
        {
            if (connectedTo == null)
            {
                if (data.parameters?.ContainsKey("connectedTo") == true)
                {
                    data.RemoveParam("connectedTo");
                }
            }
            else
            {
                data.SetOrAddParam("connectedTo", buildingLayer.Building.GetIndexOf(connectedTo));
            }
        }

        public override void OnBlockPreview(GameObject blockPreview)
        {
            base.OnBlockPreview(blockPreview);
            blockPreview.transform.position = transform.position + (transform.up * BuildingLayer.HalfBlockSize);
            blockPreview.transform.rotation = transform.rotation;
        }

        public override void OnObjectPreview(GameObject objectPreview)
        {
            base.OnObjectPreview(objectPreview);

            if (objectPreview.CompareTag("LayerBridge"))
                return;
            objectPreview.transform.position = transform.position + (transform.up * BuildingLayer.HalfBlockSize);
            objectPreview.transform.rotation = transform.rotation;
        }

        public override void OnBlockPlace(BlockTile blockTile)
        {
            base.OnBlockPlace(blockTile);

            connectedTo = buildingLayer.Building.AddNewLayer();

            connectedTo.AddBlock(blockTile, data.Position);

            ConfigurateJoint();
        }

        public override void OnObjectPlace(ObjectTile objectTile)
        {
            base.OnObjectPlace(objectTile);

            if (objectTile.GetGameObject().CompareTag("LayerBridge"))
                return;

            connectedTo = buildingLayer.Building.AddNewLayer();

            connectedTo.AddObject(objectTile, data.Position, transform.up);

            ConfigurateJoint();
        }

        private void ConfigurateJoint()
        {
            if (cJoint != null)
                Destroy(cJoint);

            cJoint = buildingLayer.gameObject.AddComponent<ConfigurableJoint>();
            connectedTo.ConnectJointToThis(cJoint);
            cJoint.anchor = buildingLayer.GridToLocal(data.Position);
            cJoint.axis = buildingLayer.transform.InverseTransformDirection(transform.up);
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
                connectedTo = buildingLayer.Building[Convert.ToInt32(data.parameters["connectedTo"])];

                ConfigurateJoint();
            }
        }
    }
}