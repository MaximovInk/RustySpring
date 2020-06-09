using UnityEngine;

namespace MaximovInk
{
    public class SaddleBehaviour : ObjectBehaviour, IInteractable
    {
        private OutPoint XAxis;
        private OutPoint YAxis;

        public void Interact()
        {
            var player = GameManager.instance.LocalPlayer;
            player.Freeze = true;
            player.transform.position = transform.position + new Vector3(0, 2, 0);
        }

        public override void OnInstantiate(BuildingLayer buildingLayer, ObjectTileData data)
        {
            base.OnInstantiate(buildingLayer, data);

            XAxis = AddOutput("X axis", "num", new Vector3Int(0, 0, 0));
            YAxis = AddOutput("Y axis", "num", new Vector3Int(0, 1, 0));
        }

        private void Update()
        {
            if (buildingLayer?.Building?.Freeze == true)
                return;

            XAxis?.ValueChanged(Input.GetAxis("Horizontal"));
            YAxis?.ValueChanged(Input.GetAxis("Vertical"));
        }
    }
}