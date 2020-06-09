using System;
using UnityEngine;

namespace MaximovInk
{
    public class SwitchBehaviour : ObjectBehaviour, IInteractable
    {
        public Transform ToggleTransform;

        public bool Value
        {
            get => value;
            set
            {
                this.value = value;
                outP.ValueChanged(value);
                UpdateModel();
            }
        }

        private bool value;

        private OutPoint outP;

        public override void OnInstantiate(BuildingLayer buildingLayer, ObjectTileData data)
        {
            base.OnInstantiate(buildingLayer, data);

            outP = AddOutput("value", "bool", Vector3Int.zero);
            Value = false;

            UpdateModel();
        }

        public override void OnSerialize()
        {
            base.OnSerialize();
            data.SetOrAddParam("toggle", value);
        }

        public override void OnDeserialize()
        {
            base.OnDeserialize();
            value = (bool)data.parameters["toggle"];
        }

        private void UpdateModel()
        {
            ToggleTransform.localRotation = Quaternion.Euler(9 * (Value ? 1 : -1), 0, 0);
        }

        public void Interact()
        {
            Value = !Value;
        }
    }
}