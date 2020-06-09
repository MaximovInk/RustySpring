using UnityEngine;

namespace MaximovInk
{
    public class Workbench : MonoBehaviour
    {
        public BuilderController BuilderController;

        public Building Building;

        private LineMesh lineMesh;

        private float updateRate = 0.1f;
        private float updateTimer = 0;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Freeze = true;
                player.Camera.enabled = false;
                BuilderController.SetActive(true);
                GameManager.instance.GameUI.SetActive(false);
                GameManager.instance.EditorUI.SetActive(true);
                EditorManager.instance.CurrentWorkbench = this;
                Init();
            }
        }

        public void OnQuit()
        {
            lineMesh.gameObject.SetActive(false);
        }

        private void Init()
        {
            if (Building != null)
                return;

            EditorManager.instance.SetActive(true);
            Building = new GameObject().AddComponent<Building>();
            Building.transform.SetParent(transform);
            Building.transform.localPosition = new Vector3(0, 1, 0);
            Building.transform.localRotation = Quaternion.identity;
            var layer0 = Building.AddNewLayer();
            layer0.AddBlock(TileDatabase.GetBlock("wood"), Vector3Int.zero);
            Building.Freeze = true;

            if (lineMesh != null)
            {
                lineMesh.gameObject.SetActive(true);
                return;
            }

            lineMesh = new GameObject().AddComponent<LineMesh>();

            lineMesh.transform.SetParent(transform);

            lineMesh.Material = MaterialsDatabase.GetMaterial("flow");
            lineMesh.gameObject.layer = LayerMask.NameToLayer("Connections");
            lineMesh.width = 0.1f;
            lineMesh.target = EditorManager.instance.CurrentWorkbench?.BuilderController?.Camera?.transform;
        }

        private void LateUpdate()
        {
            if (lineMesh?.gameObject.activeSelf != true)
                return;

            updateTimer += Time.deltaTime;
            if (updateTimer > updateRate)
            {
                updateTimer = 0f;

                var inPoints = GetComponentsInChildren<InPoint>();

                lineMesh.lines.Clear();

                for (int i = 0; i < inPoints.Length; i++)
                {
                    if (inPoints[i].ConnectedPoint == null)
                        continue;
                    lineMesh.lines.Add(new Line() { start = inPoints[i].ConnectedPoint.transform.position, end = inPoints[i].transform.position });
                }
            }
        }
    }
}