using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MaximovInk
{
    public interface ITool
    {
        void OnDeselect();

        void OnSelect(BuilderController player);

        void Update();
    }

    public interface IPlacingObject
    {
        void CreatePreview();

        void DestroyPreview();

        void SetActivePreview(bool active);

        void SetPreviewObjectPos(BuildingLayer building, Vector3 pos, Vector3 normal);

        void ObjectBehaviourPreview(ObjectBehaviour objectBehaviour, Vector3 pos);

        void ObjectBehaviourOnPlace(ObjectBehaviour objectBehaviour, Vector3 pos);

        void OnPlace(BuildingLayer building, Vector3 worldPos, Vector3 hitNormal);
    }

    public class ObjPlacing : IPlacingObject
    {
        public GameObject PreviewObject;
        public ObjectTile Shape;

        public ObjPlacing(ObjectTile shape)
        {
            this.Shape = shape;
        }

        public void CreatePreview()
        {
            PreviewObject = Object.Instantiate(Shape.GetGameObject());
            PreviewObject.SetMaterialRecursively(MaterialDatabase.GetMaterial("preview"));
            PreviewObject.SetLayerRecursively(LayerMask.NameToLayer("IgnoreR&C"));
        }

        public void SetActivePreview(bool active)
        {
            PreviewObject.SetActive(active);
        }

        public void DestroyPreview()
        {
            Object.Destroy(PreviewObject);
        }

        public void SetPreviewObjectPos(BuildingLayer building, Vector3 pos, Vector3 normal)
        {
            PreviewObject.transform.position = building.GridToWorld(building.WorldToGrid(pos));
            PreviewObject.transform.up = normal;
        }

        public void ObjectBehaviourOnPlace(ObjectBehaviour objectBehaviour, Vector3 pos)
        {
            objectBehaviour.OnObjectPlace(Shape);
        }

        public void ObjectBehaviourPreview(ObjectBehaviour objectBehaviour, Vector3 pos)
        {
            objectBehaviour.OnObjectPreview(PreviewObject);
        }

        public void OnPlace(BuildingLayer building, Vector3 worldPos, Vector3 normal)
        {
            building.AddObject(Shape, building.WorldToGrid(worldPos), normal);
        }
    }

    public class BlockPlacing : IPlacingObject
    {
        public GameObject PreviewBlock;

        private BlockTile tile;

        public BlockPlacing(BlockTile tile)
        {
            this.tile = tile;
        }

        public void CreatePreview()
        {
            if (PreviewBlock != null)
                return;

            PreviewBlock = CreateCube();
            PreviewBlock.transform.localScale = Vector3.one * BuildingLayer.kBlockSize;
            PreviewBlock.SetLayerRecursively(LayerMask.NameToLayer("IgnoreR&C"));
        }

        public void DestroyPreview()
        {
            Object.Destroy(PreviewBlock);
        }

        public void SetActivePreview(bool active)
        {
            PreviewBlock.SetActive(active);
        }

        public void SetPreviewObjectPos(BuildingLayer building, Vector3 pos, Vector3 normal)
        {
            PreviewBlock.transform.position = building.GridToWorld(building.WorldToGrid(pos));
            PreviewBlock.transform.rotation = building.transform.rotation;
        }

        public void ObjectBehaviourOnPlace(ObjectBehaviour objectBehaviour, Vector3 pos)
        {
            objectBehaviour.OnBlockPlace(tile);
        }

        public void ObjectBehaviourPreview(ObjectBehaviour objectBehaviour, Vector3 pos)
        {
            objectBehaviour.OnBlockPreview(PreviewBlock);
        }

        public void OnPlace(BuildingLayer building, Vector3 worldPos, Vector3 normal)
        {
            building.AddBlock(building.WorldToGrid(worldPos), tile);
        }

        private GameObject CreateCube()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Object.Destroy(go.GetComponent<BoxCollider>());

            var material = MaterialDatabase.GetMaterial("preview");

            var refMaterial = MaterialDatabase.GetMaterial(tile.MaterialName);

            material.SetTexture("_BaseMap", refMaterial.GetTexture("_BaseMap"));

            go.GetComponent<MeshRenderer>().sharedMaterial = material;

            return go;
        }
    }

    public class PlacingTool : ITool
    {
        public IPlacingObject PlacingObject;
        private BuilderController player;

        public PlacingTool(IPlacingObject placingObject)
        {
            PlacingObject = placingObject;
        }

        public void OnDeselect()
        {
            PlacingObject.DestroyPreview();
        }

        public void OnSelect(BuilderController player)
        {
            PlacingObject.CreatePreview();
            this.player = player;
        }

        public void Update()
        {
            LayerMask BuildingMask = (1 << LayerMask.NameToLayer("Default"));

            var cam = player.Camera;

            if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, Mathf.Infinity, BuildingMask))
            {
                PlacingObject.SetActivePreview(true);

                var objBehaviour = hit.collider.GetComponentInParent<ObjectBehaviour>();
                var building = hit.collider.GetComponentInParent<BuildingLayer>();

                var placePoint = hit.point + (hit.normal * BuildingLayer.HalfBlockSize);

                if (objBehaviour != null)
                {
                    PlacingObject.ObjectBehaviourPreview(objBehaviour, placePoint);

                    if (Input.GetMouseButtonDown(0))
                    {
                        PlacingObject.ObjectBehaviourOnPlace(objBehaviour, placePoint);
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        objBehaviour.RemoveObject();
                    }
                }
                else
                {
                    if (building != null)
                    {
                        PlacingObject.SetPreviewObjectPos(building, placePoint, hit.normal);

                        if (Input.GetMouseButtonDown(0))
                        {
                            PlacingObject.OnPlace(building, placePoint, hit.normal);
                        }
                        if (Input.GetMouseButtonDown(1))
                        {
                            ///TODO: REMade
                            var removePoint = hit.point - (hit.normal * BuildingLayer.HalfBlockSize);
                            building.RemoveObjectAt(building.WorldToGrid(removePoint));
                            building.RemoveBlockAt(building.WorldToGrid(removePoint));
                        }
                    }
                }
            }
            else
            {
                PlacingObject.SetActivePreview(false);
            }
        }
    }
}