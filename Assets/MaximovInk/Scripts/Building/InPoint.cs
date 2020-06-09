using System;
using UnityEngine;

namespace MaximovInk
{
    public class InPoint : Point
    {
        public OutPoint ConnectedPoint;

        public event Action<InPoint, object> onValueChanged;

        public void ValueChanged(object newValue)
        {
            onValueChanged?.Invoke(this, newValue);
        }

        public override void InitGraphic()
        {
            base.InitGraphic();
            GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", PointDatabase.GetPointData(Type).IconIn);
        }
    }
}