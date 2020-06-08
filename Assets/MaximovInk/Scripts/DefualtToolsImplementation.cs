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

        // ColliderChecker GetPreviewChecker();

        void SetPreviewObjectPos(BuildingLayer building, Vector3 pos, Vector3 normal);

        void ObjectBehaviourPreview(ObjectBehaviour objectBehaviour, Vector3 pos);

        void ObjectBehaviourOnPlace(ObjectBehaviour objectBehaviour, Vector3 pos);

        void OnPlace(BuildingLayer building, Vector3 worldPos, Vector3 hitNormal);
    }

    public class ObjPlacing : IPlacingObject
    {
        public GameObject PreviewObject;
        public ObjectTile ObjectTile;

        public ObjPlacing(ObjectTile shape)
        {
            this.ObjectTile = shape;
        }

        public void CreatePreview()
        {
            PreviewObject = Object.Instantiate(ObjectTile.GetGameObject())/*.AddComponent<ColliderChecker>()*/;
            PreviewObject.gameObject.SetMaterialRecursively(MaterialDatabase.GetMaterial("preview"));
            PreviewObject.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BuildingChecker"));
        }

        //public ColliderChecker GetPreviewChecker() => PreviewObject;

        public void SetActivePreview(bool active)
        {
            PreviewObject.gameObject.SetActive(active);
            /*if (!active)
                PreviewObject.ResetCounters();*/
        }

        public void DestroyPreview()
        {
            Object.Destroy(PreviewObject.gameObject);
        }

        public void SetPreviewObjectPos(BuildingLayer building, Vector3 pos, Vector3 normal)
        {
            PreviewObject.transform.position = building.GridToWorld(building.WorldToGrid(pos));
            PreviewObject.transform.up = normal;
        }

        public void ObjectBehaviourOnPlace(ObjectBehaviour objectBehaviour, Vector3 pos)
        {
            objectBehaviour.OnObjectPlace(ObjectTile);
        }

        public void ObjectBehaviourPreview(ObjectBehaviour objectBehaviour, Vector3 pos)
        {
            objectBehaviour.OnObjectPreview(PreviewObject.gameObject);
        }

        public void OnPlace(BuildingLayer building, Vector3 worldPos, Vector3 normal)
        {
            building.AddObject(ObjectTile, building.WorldToGrid(worldPos), normal);
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

            PreviewBlock = CreateCube()/*.AddComponent<ColliderChecker>()*/;
            PreviewBlock.transform.localScale = Vector3.one * BuildingLayer.kBlockSize;
            PreviewBlock.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BuildingChecker"));
        }

        //public ColliderChecker GetPreviewChecker() => PreviewBlock;

        public void DestroyPreview()
        {
            Object.Destroy(PreviewBlock.gameObject);
        }

        public void SetActivePreview(bool active)
        {
            PreviewBlock.gameObject.SetActive(active);
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
            objectBehaviour.OnBlockPreview(PreviewBlock.gameObject);
        }

        public void OnPlace(BuildingLayer building, Vector3 worldPos, Vector3 normal)
        {
            building.AddBlock(tile, building.WorldToGrid(worldPos));
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

                if (hit.collider.CompareTag("ObjectTile"))
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        Object.Destroy(hit.collider.gameObject);
                    }
                }

                if (objBehaviour != null)
                {
                    PlacingObject.ObjectBehaviourPreview(objBehaviour, placePoint);

                    if (Input.GetMouseButtonDown(0))
                    {
                        PlacingObject.ObjectBehaviourOnPlace(objBehaviour, placePoint);
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

    public class LayerCombinerTool : ITool
    {
        private BuilderController player;

        private BuildingLayer begin;
        private BuildingLayer end;

        public void OnDeselect()
        {
        }

        public void OnSelect(BuilderController player)
        {
            this.player = player;
        }

        public void Update()
        {
            LayerMask BuildingMask = (1 << LayerMask.NameToLayer("Default"));

            var cam = player.Camera;

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, Mathf.Infinity, BuildingMask))
                {
                    var buildingLayer = hit.collider.GetComponentInParent<BuildingLayer>();
                    if (begin == null)
                    {
                        begin = buildingLayer;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (begin != null)
                {
                    if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, Mathf.Infinity, BuildingMask))
                    {
                        var buildingLayer = hit.collider.GetComponentInParent<BuildingLayer>();
                        if (buildingLayer != null)
                        {
                            end = buildingLayer;
                            buildingLayer.Building.CombineLayers(begin, end);
                        }
                    }
                    begin = null;
                    end = null;
                }
            }
        }
    }
}