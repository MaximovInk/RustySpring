using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MaximovInk
{
    [Serializable]
    public struct Line
    {
        public Vector3 start;
        public Vector3 end;
    }

    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class LineMesh : MonoBehaviour
    {
        public List<Line> lines = new List<Line>();

        private MeshData data;

        public Transform target;

        public Material Material { get => meshRenderer.material; set => meshRenderer.material = value; }

        public float width = 1f;

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            data = new MeshData();

            GetComponent<MeshFilter>().mesh = data.GetMesh();

            meshRenderer = GetComponent<MeshRenderer>();

            UpdateMesh();
        }

        public void UpdateMesh()
        {
            data.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                var vMin = transform.InverseTransformPoint(lines[i].start);
                var vMax = transform.InverseTransformPoint(lines[i].end);
                var delta = vMax - vMin;

                var center = lines[i].start + (lines[i].start - lines[i].end);

                var normal = Vector3.Cross(delta, (center - target.position)).normalized / 2f * width;

                data.AddQuad(
                    vMin - normal,
                    vMin + normal,
                    vMax + normal,
                    vMax - normal,
                    new Vector4(0, 0, 1 * delta.magnitude, 1),
                    Color.white,
                    0
                    );
            }
            data.ApplyToMesh();
        }

        private void LateUpdate()
        {
            UpdateMesh();
        }
    }
}