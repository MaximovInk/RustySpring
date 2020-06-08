using UnityEngine;

namespace MaximovInk
{
    public class Workbench : MonoBehaviour
    {
        public BuilderController BuilderController;

        public Building Building;

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
                Init();
            }
        }

        private void Init()
        {
            if (Building != null)
                return;

            Building = new GameObject().AddComponent<Building>();
            Building.transform.SetParent(transform);
            Building.transform.localPosition = new Vector3(0, 1, 0);
            Building.transform.localRotation = Quaternion.identity;
            var layer0 = Building.AddNewLayer();
            layer0.AddBlock(Vector3Int.zero, TileDatabase.GetBlock("wood"));
            Building.Freeze = true;
        }
    }
}