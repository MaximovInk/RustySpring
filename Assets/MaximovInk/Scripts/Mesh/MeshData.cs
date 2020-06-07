using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MaximovInk
{
    [Serializable]
    public class MeshData
    {
        [Serializable]
        private class SubMeshTriangles
        {
            public int ID;
            public List<int> triangles;

            public SubMeshTriangles(int iD)
            {
                ID = iD;
                this.triangles = new List<int>();
            }
        }

        [HideInInspector, NonSerialized]
        private readonly List<Vector3> vertices = new List<Vector3>();

        [HideInInspector, NonSerialized]
        private readonly List<SubMeshTriangles> subMeshTriangles = new List<SubMeshTriangles>();

        private int subMeshCount;

        //[HideInInspector, NonSerialized]
        //private readonly List<int> triangles = new List<int>();

        [HideInInspector, NonSerialized]
        private readonly List<Vector2> uvs = new List<Vector2>();

        [HideInInspector, NonSerialized]
        private readonly List<Color32> colors = new List<Color32>();

        private Mesh Mesh { get { return mesh != null ? mesh : (mesh = new Mesh()); } }

        [HideInInspector, NonSerialized]
        private Mesh mesh;

        public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector4 uv, Color color, int subMeshID)
        {
            var subMeshTriangle = subMeshTriangles.Find(n => n.ID == subMeshID);
            if (subMeshTriangle == null)
            {
                subMeshCount = Mathf.Max(subMeshCount, subMeshID + 1);
                subMeshTriangle = new SubMeshTriangles(subMeshID);
                subMeshTriangles.Add(subMeshTriangle);
            }

            subMeshTriangle.triangles.Add(vertices.Count);
            subMeshTriangle.triangles.Add(vertices.Count + 1);
            subMeshTriangle.triangles.Add(vertices.Count + 2);
            subMeshTriangle.triangles.Add(vertices.Count);
            subMeshTriangle.triangles.Add(vertices.Count + 2);
            subMeshTriangle.triangles.Add(vertices.Count + 3);

            /*subMeshTriangles.Add(subMeshID, vertices.Count);
            subMeshTriangles.Add(subMeshID, vertices.Count + 1);
            subMeshTriangles.Add(subMeshID, vertices.Count + 2);
            subMeshTriangles.Add(subMeshID, vertices.Count);
            subMeshTriangles.Add(subMeshID, vertices.Count + 2);
            subMeshTriangles.Add(subMeshID, vertices.Count + 3);*/

            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);

            uvs.Add(new Vector2(uv.x, uv.y));
            uvs.Add(new Vector2(uv.x, uv.w));
            uvs.Add(new Vector2(uv.z, uv.w));
            uvs.Add(new Vector2(uv.z, uv.y));

            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        public void Clear()
        {
            subMeshCount = 0;
            subMeshTriangles.Clear();
            vertices.Clear();
            uvs.Clear();
            colors.Clear();
        }

        public void ApplyToMesh()
        {
            lock (mesh)
            {
                Mesh.Clear();
                Mesh.vertices = vertices.ToArray();

                //Mesh.triangles = triangles.ToArray();

                Mesh.subMeshCount = subMeshCount;
                //Debug.Log("M:T " + Mesh.subMeshCount + " " + subMeshCount);

                for (int i = 0; i < subMeshTriangles.Count; i++)
                {
                    //Debug.Log("I " + Mesh.subMeshCount + " " + subMeshTriangles[i].ID);
                    Mesh.SetTriangles(subMeshTriangles[i].triangles.ToArray(), subMeshTriangles[i].ID);
                }

                Mesh.uv = uvs.ToArray();
                Mesh.colors32 = colors.ToArray();
                Mesh.RecalculateNormals();
                Mesh.RecalculateTangents();
            }
        }

        public Mesh GetMesh()
        {
            return Mesh;
        }
    }
}