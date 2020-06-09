using System;
using UnityEngine;

namespace MaximovInk
{
    public enum AxleType
    {
        Free,
        Control,
        Motor
    }

    public class AxleBehaviour : ObjectBehaviour
    {
        private ConfigurableJoint cJoint;
        private BuildingLayer connectedTo;

        public AxleType axleType;

        public float valueTest = 0;

        public override void OnInstantiate(BuildingLayer buildingLayer, ObjectTileData data)
        {
            base.OnInstantiate(buildingLayer, data);

            var inP = AddInput("steering", "num", Vector3Int.zero);

            // buildingLayer.Building.OnLayerAdd += (_) => UpdateData();
            //buildingLayer.Building.OnLayerRemove += (_) => UpdateData();
            buildingLayer.CanCombineCallback += CanCombineCallback;

            inP.onValueChanged += (_, value) => { valueTest = Convert.ToSingle(value); };
        }

        private void CanCombineCallback(BuildingLayer other, ref bool can)
        {
            can &= connectedTo != other;
        }

        /*private void UpdateData()
        {
        }*/

        private void Update()
        {
            if (cJoint != null)
                cJoint.targetAngularVelocity = new Vector3(valueTest, valueTest, valueTest);
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

        public override void RemoveObject()
        {
            Destroy(cJoint);
            connectedTo = null;
            //UpdateData();
            base.RemoveObject();
        }

        public override void OnBlockPlace(BlockTile blockTile)
        {
            base.OnBlockPlace(blockTile);

            connectedTo = buildingLayer.Building.AddNewLayer();
            //UpdateData();

            connectedTo.AddBlock(blockTile, data.Position);

            ConfigurateJoint();
        }

        public override void OnObjectPlace(ObjectTile objectTile)
        {
            base.OnObjectPlace(objectTile);

            if (objectTile.GetGameObject().CompareTag("LayerBridge"))
                return;

            connectedTo = buildingLayer.Building.AddNewLayer();
            //UpdateData();

            connectedTo.AddObject(objectTile, data.Position, transform.up);

            ConfigurateJoint();
        }

        private void ConfigurateJoint()
        {
            print("configurate");

            cJoint = buildingLayer.gameObject.AddComponent<ConfigurableJoint>();
            connectedTo.ConnectJointToThis(cJoint);
            cJoint.anchor = buildingLayer.GridToLocal(data.Position);
            cJoint.axis = buildingLayer.transform.InverseTransformDirection(transform.up);
            cJoint.angularXMotion = ConfigurableJointMotion.Free;
            cJoint.angularYMotion = ConfigurableJointMotion.Locked;
            cJoint.angularZMotion = ConfigurableJointMotion.Locked;

            var angDrive = new JointDrive() { positionDamper = 1f, maximumForce = 5f };

            cJoint.angularXDrive = angDrive;
            cJoint.angularYZDrive = angDrive;

            cJoint.xMotion = ConfigurableJointMotion.Locked;
            cJoint.yMotion = ConfigurableJointMotion.Locked;
            cJoint.zMotion = ConfigurableJointMotion.Locked;
        }

        public override void OnSerialize()
        {
            base.OnSerialize();

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