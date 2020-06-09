using System;
using UnityEngine;

namespace MaximovInk
{
    public class EngineBehaviour : ObjectBehaviour
    {
        public float input = 1;
        private float fuel = 100;

        private float power = 10f;

        private OutPoint outPower;

        public override void OnInstantiate(BuildingLayer buildingLayer, ObjectTileData data)
        {
            base.OnInstantiate(buildingLayer, data);

            var inP = AddInput("value", "num", Vector3Int.zero);
            outPower = AddOutput("power", "num", new Vector3Int(0, 1, 0));

            inP.onValueChanged += (_, obj) => input = Convert.ToSingle(obj);
        }

        private void Update()
        {
            if (fuel > 0 && outPower != null)
            {
                outPower.ValueChanged(input * power);
            }
        }
    }
}