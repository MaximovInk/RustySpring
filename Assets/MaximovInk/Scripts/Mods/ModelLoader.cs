using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

using Material = UnityEngine.Material;
using Mesh = UnityEngine.Mesh;

namespace MaximovInk
{
    public abstract class MeshImporter
    {
        public abstract LoadedModel Load(string path, Vector3 scale);
    }

    public class LoadedModel
    {
        public string name { get; set; }
        public List<LoadedMesh> meshes { get; set; }

        public LoadedModel()
        {
            name = string.Empty;
            meshes = new List<LoadedMesh>();
        }
    }

    public class MTLFile
    {
        public List<Material> materials = new List<Material>();

        public class Material
        {
            public string name { get; set; }        //newmtl name
            public Color Ambient { get; set; }      //Ka r g b
            public Texture2D DiffuseMap { get; set; }   //map_Kd r g b
            public Color Diffuse { get; set; }      //Kd r g b
            public Color Specular { get; set; }     //Ks r g b
            public Color Transmission { get; set; } //Tf r g b
            public Color Emissive { get; set; }     //Ke r g b
            public float Shininess { get; set; }     //Ns x
            public float Illumination { get; set; } //illum x
            public float Disslove { get; set; }     //d x
            public float Sharpness { get; set; }    //sharpness
            public float Opacity { get; set; }      //Ni x
        }
    }

    public class LoadedMesh
    {
        public string Name { get; set; }            //o name
        public List<Vector3> Vertices { get; set; } //v x y z
        public List<int> Indices { get; set; }      //f x/y/z ...
        public List<Vector2> Uv { get; set; }       //vt x y z
        public List<Vector3> Normals { get; set; }  //vn x y z
        public Material Material { get; set; }      //usemtl name
        public bool Smooth { get; set; }            //s off/on

        public LoadedMesh()
        {
            Smooth = false;
            Name = string.Empty;
            Vertices = new List<Vector3>();
            Indices = new List<int>();
            Uv = new List<Vector2>();
            Normals = new List<Vector3>();
            Material = new Material(Shader.Find("HDRP/Lit"));
        }
    }

    public class ObjImporter : MeshImporter
    {
        private string loadPath = string.Empty;

        #region MTL

        private string mtllib = string.Empty;
        private MTLFile currentMTLFile = new MTLFile();
        private MTLFile.Material currentMTL = new MTLFile.Material();

        public static Texture2D LoadTexture(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(1, 1);
                tex.LoadImage(fileData);
            }
            return tex;
        }

        public MTLFile LoadMTL(string path)
        {
            if (!File.Exists(path))
                return null;

            var lines = File.ReadAllLines(path);

            currentMTLFile = new MTLFile();

            for (int i = 0; i < lines.Length; i++)
            {
                ProcessLineMTL(lines[i]);
            }

            return currentMTLFile;
        }

        private void ProcessLineMTL(string line)
        {
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "#":
                        return;

                    case "newmtl":
                        currentMTL = new MTLFile.Material() { name = parts[1] };

                        currentMTLFile.materials.Add(currentMTL);

                        break;

                    case "Ka":
                        currentMTL.Ambient = new Color(cf(parts[1]), cf(parts[2]), cf(parts[3]), 1f);
                        break;

                    case "Kd":

                        currentMTL.Diffuse = new Color(cf(parts[1]), cf(parts[2]), cf(parts[3]), 1f);
                        break;

                    case "map_Kd":
                        var texture = LoadTexture(Path.Combine(loadPath, parts[1]));
                        currentMTL.DiffuseMap = texture;
                        break;

                    case "Ks":
                        currentMTL.Specular = new Color(cf(parts[1]), cf(parts[2]), cf(parts[3]), 1f);
                        break;

                    case "Ns":
                        currentMTL.Shininess = cf(parts[1]);
                        break;

                    case "Tf":
                        currentMTL.Transmission = new Color(cf(parts[1]), cf(parts[2]), cf(parts[3]), 1f);
                        break;

                    case "illum":
                        currentMTL.Illumination = cf(parts[1]);
                        break;

                    case "d":
                        currentMTL.Disslove = cf(parts[1]);
                        break;

                    case "Ke":
                        currentMTL.Emissive = new Color(cf(parts[1]), cf(parts[2]), cf(parts[3]), 1f);
                        break;

                    case "sharpness":
                        currentMTL.Sharpness = cf(parts[1]);
                        break;

                    case "Ni":
                        currentMTL.Opacity = cf(parts[1]);
                        break;
                }
            }
        }

        #endregion MTL

        public override LoadedModel Load(string path, Vector3 scale)
        {
            if (!File.Exists(path))
                return null;

            currentModel = new LoadedModel();

            loadPath = new FileInfo(path).Directory.FullName;

            var lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++)
            {
                ProcessLineObj(lines[i]);
            }

            return currentModel;
        }

        private float cf(string v)
        {
            return float.Parse(v, CultureInfo.InvariantCulture);
        }

        private int ci(string v)
        {
            return int.Parse(v, CultureInfo.InvariantCulture);
        }

        #region OBJ

        private LoadedModel currentModel;
        private LoadedMesh currentMesh;
        private MTLFile modelMTLlib;

        private readonly List<Vector3> tempVertices = new List<Vector3>();
        private readonly List<Vector2> tempUv = new List<Vector2>();
        private readonly List<Vector3> tempNormals = new List<Vector3>();

        private void ProcessLineObj(string line)
        {
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "#":
                        return;

                    case "s":
                        currentMesh.Smooth = string.Equals(parts[1].Trim(), "on", StringComparison.OrdinalIgnoreCase);
                        break;

                    case "mtllib":
                        mtllib = parts[1];
                        modelMTLlib = LoadMTL(Path.Combine(loadPath, mtllib));
                        break;

                    case "o":
                        currentMesh = new LoadedMesh() { Name = parts[1] };
                        currentModel.meshes.Add(currentMesh);
                        break;

                    case "v":
                        tempVertices.Add(new Vector3(cf(parts[1]), cf(parts[2]), cf(parts[3])));
                        break;

                    case "vt":
                        tempUv.Add(new Vector2(cf(parts[1]), cf(parts[2])));
                        break;

                    case "vn":
                        tempNormals.Add(new Vector3(cf(parts[1]), cf(parts[2]), cf(parts[3])));
                        break;

                    case "usemtl":
                        var mat = modelMTLlib.materials.Find(n => string.Equals(n.name, parts[1], StringComparison.OrdinalIgnoreCase));

                        var uMat = new Material(Shader.Find("HDRP/Lit"));
                        uMat.SetColor("_BaseColor", new Color(mat.Diffuse.r, mat.Diffuse.g, mat.Diffuse.b, mat.Disslove));
                        uMat.SetTexture("_BaseColorMap", mat.DiffuseMap);
                        uMat.SetColor("_SpecularColor", mat.Specular);
                        uMat.SetColor("_EmissiveColor", mat.Emissive);
                        uMat.SetColor("_TransmittanceColor", mat.Transmission);

                        currentMesh.Material = uMat;

                        break;

                    case "f":
                        var elemCount = currentMesh.Vertices.Count;

                        if (parts.Length == 5)
                        {
                            //Quad
                            for (var i = 1; i <= 4; i++)
                            {
                                var data = parts[i].Split('/');
                                currentMesh.Vertices.Add(tempVertices[ci(data[0]) - 1]);
                                currentMesh.Uv.Add(tempUv[ci(data[1]) - 1]);
                                currentMesh.Normals.Add(tempNormals[ci(data[2]) - 1]);
                            }

                            currentMesh.Indices.Add(elemCount + 0);
                            currentMesh.Indices.Add(elemCount + 1);
                            currentMesh.Indices.Add(elemCount + 2);
                            currentMesh.Indices.Add(elemCount + 0);
                            currentMesh.Indices.Add(elemCount + 2);
                            currentMesh.Indices.Add(elemCount + 3);
                        }
                        else if (parts.Length == 4)
                        {
                            //Traingle
                            for (var i = 1; i <= 3; i++)
                            {
                                var data = parts[i].Split('/');

                                currentMesh.Vertices.Add(tempVertices[ci(data[0]) - 1]);
                                currentMesh.Uv.Add(tempUv[ci(data[1]) - 1]);
                                currentMesh.Normals.Add(tempNormals[ci(data[2]) - 1]);
                            }

                            currentMesh.Indices.Add(elemCount + 0);
                            currentMesh.Indices.Add(elemCount + 1);
                            currentMesh.Indices.Add(elemCount + 2);
                        }

                        break;
                }
            }
        }

        #endregion OBJ
    }

    public static class ModelLoader
    {
        public static GameObject Load(string meshPath, Vector3 scale, MeshImporter meshImporter)
        {
            var model = meshImporter.Load(meshPath, scale);

            if (model == null)
                return null;

            var rootGo = new GameObject(model.name + "_root");

            for (int i = 0; i < model.meshes.Count; i++)
            {
                var mesh = model.meshes[i];

                var go = new GameObject(mesh.Name);
                go.transform.SetParent(rootGo.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                var meshFilter = go.AddComponent<MeshFilter>();
                var meshRenderer = go.AddComponent<MeshRenderer>();

                var uMesh = new Mesh();

                uMesh.vertices = mesh.Vertices.ToArray();
                uMesh.triangles = mesh.Indices.ToArray();
                uMesh.uv = mesh.Uv.ToArray();
                uMesh.normals = mesh.Normals.ToArray();

                meshFilter.mesh = uMesh;
                meshRenderer.material = mesh.Material;
            }

            return rootGo;
        }
    }
}