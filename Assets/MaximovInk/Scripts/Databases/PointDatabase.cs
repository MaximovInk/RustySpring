using System.Collections.Generic;
using UnityEngine;

namespace MaximovInk
{
    public struct PointData
    {
        public string Name;
        public Texture2D IconOut;
        public Texture2D IconIn;
        public Color Color;
    }

    public static class PointDatabase
    {
        private static readonly Dictionary<string, PointData> points = new Dictionary<string, PointData>();

        public static void RegisterPointData(PointData pointData, bool replace = false)
        {
            if (points.ContainsKey(pointData.Name))
            {
                if (!replace)
                {
                    Debug.LogError("Point data is already registered in database:" + pointData.Name);
                }
                else
                {
                    points[pointData.Name] = pointData;
                }

                return;
            }

            points.Add(pointData.Name, pointData);
        }

        public static PointData GetPointData(string name)
        {
            return points[name];
        }

        static PointDatabase()
        {
            var oil = Resources.Load<Texture2D>("Textures/oil_connector");
            var boolean = Resources.Load<Texture2D>("Textures/bool_connector");
            var number = Resources.Load<Texture2D>("Textures/num_connector");
            var energy = Resources.Load<Texture2D>("Textures/energy_connector");

            RegisterPointData(new PointData() { IconOut = oil, IconIn = oil, Name = "oil", Color = Color.white });
            RegisterPointData(new PointData() { IconOut = boolean, IconIn = boolean, Name = "bool", Color = Color.white });
            RegisterPointData(new PointData() { IconOut = number, IconIn = number, Name = "num", Color = Color.white });
            RegisterPointData(new PointData() { IconOut = energy, IconIn = energy, Name = "energy", Color = Color.white });
        }
    }
}