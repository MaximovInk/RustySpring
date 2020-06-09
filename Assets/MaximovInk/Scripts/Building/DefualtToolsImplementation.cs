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
            PreviewObject = Object.Instantiate(ObjectTile.GetGameObject());
            PreviewObject.gameObject.SetMaterialRecursively(MaterialsDatabase.GetMaterial("preview"));
            PreviewObject.gameObject.SetLayerRecursively(LayerMask.NameToLayer("IgnoreR&C"));
        }

        public void SetActivePreview(bool active)
        {
            PreviewObject.gameObject.SetActive(active);
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

            PreviewBlock = CreateCube();
            PreviewBlock.transform.localScale = Vector3.one * BuildingLayer.kBlockSize;
            PreviewBlock.gameObject.SetLayerRecursively(LayerMask.NameToLayer("IgnoreR&C"));
        }

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

            var material = MaterialsDatabase.GetMaterial("preview");

            var refMaterial = MaterialsDatabase.GetMaterial(tile.MaterialName);

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
                    if (Input.GetMouseButtonDown(1))
                    {
                        objBehaviour.RemoveObject();
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

    public class ConnectorTool : ITool
    {
        private LineMesh line;
        private float dist;
        private BuilderController player;

        private Point begin;
        private Point end;

        private void CreateLine()
        {
            line = new GameObject().AddComponent<LineMesh>();
            line.Material = MaterialsDatabase.GetMaterial("flow");
            line.gameObject.layer = LayerMask.NameToLayer("Connections");
            line.width = 0.1f;
            line.target = player.Camera.transform;
            line.lines.Add(new Line());
        }

        public void OnDeselect()
        {
        }

        public void OnSelect(BuilderController player)
        {
            this.player = player;
            CreateLine();

            line.gameObject.SetActive(false);
        }

        private void CombinePoints(Point a, Point b)
        {
            if (a is InPoint && b is OutPoint)
            {
                CombinePoints(a as InPoint, b as OutPoint);
            }
            else if (a is OutPoint && b is InPoint)
            {
                CombinePoints(b as InPoint, a as OutPoint);
            }
        }

        private void CombinePoints(InPoint a, OutPoint b)
        {
            if (!string.Equals(a.Type, b.Type, StringComparison.InvariantCultureIgnoreCase))
                return;

            b.ConnectedPoints.Add(a);
            a.ConnectedPoint = b;
            b.ForceUpdate();
        }

        public void Update()
        {
            LayerMask connectionMask = (1 << LayerMask.NameToLayer("Connections"));

            var cam = player.Camera;

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, Mathf.Infinity, connectionMask))
                {
                    var point = hit.collider.GetComponentInParent<Point>();

                    if (point != null)
                    {
                        begin = point;

                        dist = Vector3.Distance(point.transform.position, cam.transform.position);

                        line.gameObject.SetActive(true);
                    }
                }
            }

            if (Input.GetMouseButton(0) && begin != null)
            {
                if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, Mathf.Infinity, connectionMask))
                {
                    var point = hit.collider.GetComponentInParent<Point>();

                    if (point != null && point != begin)
                    {
                        end = point;
                        line.lines[0] = new Line() { start = begin.transform.position, end = point.transform.position };
                    }
                    else
                    {
                        line.lines[0] = new Line() { start = begin.transform.position, end = cam.transform.position + (cam.transform.forward * dist) };
                        end = null;
                    }
                }
                else
                {
                    line.lines[0] = new Line() { start = begin.transform.position, end = cam.transform.position + (cam.transform.forward * dist) };
                    end = null;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (begin != null && end != null)
                    CombinePoints(begin, end);
                begin = null;
                end = null;
                line.gameObject.SetActive(false);
            }
        }
    }
}