using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaximovInk
{
    public class OutPoint : Point
    {
        public List<InPoint> ConnectedPoints = new List<InPoint>();

        private object cachedValue;

        public void ValueChanged(object newValue, bool forceUpdate = false)
        {
            if (newValue != cachedValue || forceUpdate)
            {
                cachedValue = newValue;

                for (int i = 0; i < ConnectedPoints.Count; i++)
                {
                    ConnectedPoints[i].ValueChanged(newValue);
                }
            }
        }

        public void ForceUpdate()
        {
            ValueChanged(cachedValue, true);
        }

        public override void InitGraphic()
        {
            base.InitGraphic();
            GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", PointDatabase.GetPointData(Type).IconOut);
        }
    }
}