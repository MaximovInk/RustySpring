using UnityEngine;

namespace MaximovInk
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(BoxCollider))]
    public class Point : MonoBehaviour
    {
        public string Name;

        public string Type;

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();

            if (meshRenderer == null)
            {
                Destroy(gameObject);
                return;
            }

            EditorManager.instance.OnActiveChanged += (val) => meshRenderer.enabled = val;
        }

        public virtual void InitGraphic()
        {
            GetComponent<MeshFilter>().mesh = CreateQuad();
            GetComponent<MeshRenderer>().material = MaterialsDatabase.GetMaterial("cutout");
        }

        public virtual void SetState(bool activated)
        {
        }

        private void LateUpdate()
        {
            transform.forward = -Camera.main.transform.forward;
        }

        //TODO: MAYBE Mesh runtime utility class?
        private Mesh CreateQuad()
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
            new Vector3(-0.5f, -0.5f, 0),
            new Vector3(0.5f, -0.5f, 0),
            new Vector3(-0.5f, 0.5f, 0),
            new Vector3(0.5f, 0.5f, 0)
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
            };
            mesh.triangles = tris;

            Vector2[] uv = new Vector2[4]
            {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
            };
            mesh.uv = uv;

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}