using UnityEngine;

namespace MaximovInk
{
    public class Workbench : MonoBehaviour
    {
        public BuilderController BuilderController;

        private Building building;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Freeze = true;
                player.Camera.enabled = false;
                BuilderController.SetActive(true);
                Init();
            }
        }

        private void Init()
        {
            if (building != null)
                return;

            building = new GameObject().AddComponent<Building>();
            building.transform.SetParent(transform);
            building.transform.localPosition = new Vector3(0, 1, 0);
            building.transform.localRotation = Quaternion.identity;
            var layer0 = building.AddNewLayer();
            layer0.AddBlock(Vector3Int.zero, TileDatabase.GetBlock("wood"));
            building.Freeze = true;
        }
    }
}