using UnityEngine;

namespace MaximovInk
{
    public class LightBehaviour : ObjectBehaviour
    {
        public GameObject Light;

        public override void OnInstantiate(BuildingLayer buildingLayer, ObjectTileData data)
        {
            base.OnInstantiate(buildingLayer, data);

            var inP = AddInput("value", "bool", Vector3Int.zero);
            inP.onValueChanged += (_, obj) =>
                Light.SetActive((bool)obj);
        }
    }
}