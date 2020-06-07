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

    /*
    public class HammerTool : ITool
    {
        public HammerTool(float force)
        {
            this.force = force;
        }

        private Player player;

        private readonly float force;

        public void OnDeselect()
        {
        }

        public void OnSelect(Player player)
        {
            this.player = player;
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var cam = player.Camera;
                var buildDistance = player.BuildDistance;
                LayerMask BuildingMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Ground"));

                if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, buildDistance, BuildingMask))
                {
                    var rb = hit.collider.GetComponentInParent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForceAtPosition(((player.transform.forward * 0.8f) + player.transform.up) * force, hit.point);
                    }
                }
            }
        }
    }
    */

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
            /*Vector3[] vertices = {
            new Vector3 (-0.5f, -0.5f, -0.5f),
            new Vector3 (0.5f, -0.5f, -0.5f),
            new Vector3 (0.5f, 0.5f, -0.5f),
            new Vector3 (-0.5f, 0.5f, -0.5f),
            new Vector3 (-0.5f, 0.5f, 0.5f),
            new Vector3 (0.5f, 0.5f, 0.5f),
            new Vector3 (0.5f, -0.5f, 0.5f),
            new Vector3 (-0.5f, -0.5f, 0.5f),
        };

            int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

            var go = new GameObject();

            Mesh mesh = new Mesh();
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.Optimize();
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;*/

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
            LayerMask BuildingMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Ground"));

            var cam = player.Camera;

            if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, BuildingMask))
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
                            building.RemoveObject(building.WorldToGrid(removePoint));
                            building.RemoveBlock(building.WorldToGrid(removePoint));
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

    /*
    public class LiftTool : ITool
    {
        public BlockMesh buildingCopy;
        public BlockMesh buildingOriginal;
        public bool IsMovingLift;
        public BlockMesh LiftBlockMesh;
        public Transform LiftBottom;
        public Transform LiftCenter;
        public Collider[] liftColliders;
        public float LiftCurrentHeight;
        public GameObject LiftInstance;
        private static GameObject lastLift;
        private static float lastLiftH;
        public float LiftMaxHeight = 10f;
        public Transform LiftTop;
        private BuilderController player;

        public void OnDeselect()
        {
            if (IsMovingLift)
                Object.Destroy(LiftInstance);

            if (IsMovingLift && buildingOriginal == null && buildingCopy != null)
            {
                buildingCopy.gameObject.SetLayerRecursively((1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Ground")));
                buildingCopy.IsKinematic = false;
                buildingCopy.transform.SetParent(null);
                buildingCopy.SetPreview(false);
                buildingCopy = null;
            }

            lastLift = LiftInstance;
            lastLiftH = LiftCurrentHeight;
        }

        public void OnSelect(BuilderController player)
        {
            if (lastLift != null)
            {
                LiftInstance = lastLift;
                LiftCurrentHeight = lastLiftH;

                buildingCopy = LiftInstance.GetComponentInChildren<BlockMesh>();
                InitLift();
            }

            this.player = player;
        }

        private void InitLift()
        {
            LiftTop = LiftInstance.transform.Find("top");
            LiftCenter = LiftInstance.transform.Find("center");
            LiftBottom = LiftInstance.transform.Find("bottom");
            liftColliders = LiftInstance.GetComponentsInChildren<Collider>();
        }

        public void Update()
        {
            LayerMask BuildingMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Ground"));

            var cam = player.Camera;
            var buildDistance = player.BuildDistance;

            if (LiftInstance == null)
            {
                var lift = Resources.Load<GameObject>("Prefabs/lift");
                LiftInstance = Object.Instantiate(lift);
                LiftInstance.SetLayerRecursively(LayerMask.NameToLayer("IgnoreR&C"));
                InitLift();
                LiftCurrentHeight = 0f;
                IsMovingLift = true;
            }
            if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, buildDistance, BuildingMask))
            {
                if (IsMovingLift)
                    LiftInstance.transform.position = hit.point + (Vector3.up * 0.46f);

                for (int i = 0; i < liftColliders.Length; i++)
                {
                    liftColliders[i].isTrigger = IsMovingLift;
                }

                var building = hit.collider.GetComponentInParent<BlockMesh>();

                if (building?.CanMoveByLift == false)
                {
                    building = null;
                }

                if (IsMovingLift)
                {
                    if (building != null)
                    {
                        if (buildingCopy == null)
                        {
                            buildingOriginal = building;

                            buildingCopy = Object.Instantiate(building);
                            //buildingCopy.gameObject.SetLayerRecursively(LayerMask.NameToLayer("IgnoreR&C"));
                            buildingCopy.SetLayer(LayerMask.NameToLayer("IgnoreR&C"));
                            buildingCopy.CenterAllBlocksAndObject();
                            buildingCopy.SetPreview(true);

                            buildingCopy.IsKinematic = true;
                            buildingCopy.Velocity = Vector3.zero;
                            buildingCopy.AngularVelocity = Vector3.zero;

                            buildingCopy.transform.SetParent(LiftInstance.transform);
                            buildingCopy.transform.localPosition = new Vector3(0, LiftCurrentHeight + BlockMesh.HalfBlockSize, 0);
                            buildingCopy.transform.localRotation = Quaternion.identity;
                        }
                    }
                    else if (buildingOriginal != null && buildingCopy != null)
                    {
                        Object.Destroy(buildingCopy.gameObject);
                        buildingOriginal = null;
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (buildingOriginal != null && buildingCopy != null)
                    {
                        Object.Destroy(buildingOriginal.gameObject);
                    }
                    if (buildingOriginal == null && buildingCopy != null)
                    {
                        buildingCopy.SetPreview(false);
                        buildingCopy.SetIsDirty();
                        LiftInstance.SetLayerRecursively(LayerMask.NameToLayer("Default"));
                        buildingCopy.SetLayer(LayerMask.NameToLayer("Default"));
                        IsMovingLift = false;
                    }

                    if (buildingOriginal == null && buildingCopy == null)
                    {
                        LiftInstance.SetLayerRecursively(LayerMask.NameToLayer("Default"));
                        IsMovingLift = false;
                    }
                }

                if (Input.GetMouseButtonDown(1) && LiftInstance != null && hit.collider.transform.root.gameObject == LiftInstance)
                {
                    if (buildingCopy != null)
                    {
                        //buildingCopy.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
                        buildingCopy.SetLayer(LayerMask.NameToLayer("Default"));
                        buildingCopy.IsKinematic = false;
                        buildingCopy.transform.SetParent(null);
                        buildingCopy.SetPreview(false);
                        buildingCopy = null;
                    }

                    Object.Destroy(LiftInstance);
                }
            }

            if (!IsMovingLift)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    LiftCurrentHeight += Time.deltaTime;
                    LiftCurrentHeight = Mathf.Min(LiftMaxHeight, LiftCurrentHeight);

                    UpdateLift();

                    if (buildingCopy != null && buildingOriginal == null)
                        buildingCopy.transform.localPosition = new Vector3(0, LiftCurrentHeight + BlockMesh.HalfBlockSize, 0);
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    LiftCurrentHeight -= Time.deltaTime;
                    LiftCurrentHeight = Mathf.Max(0, LiftCurrentHeight);

                    UpdateLift();

                    if (buildingCopy != null && buildingOriginal == null)
                        buildingCopy.transform.localPosition = new Vector3(0, LiftCurrentHeight + BlockMesh.HalfBlockSize, 0); ;
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    var angle = 50f * Time.deltaTime;

                    LiftTop.transform.eulerAngles += new Vector3(0, angle, 0);

                    if (buildingCopy != null && buildingOriginal == null)
                    {
                        buildingCopy.transform.eulerAngles += new Vector3(0, angle, 0);
                        buildingCopy.transform.localPosition = new Vector3(0, LiftCurrentHeight + BlockMesh.HalfBlockSize, 0);
                    }
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    var angle = 50f * Time.deltaTime;

                    LiftTop.transform.eulerAngles -= new Vector3(0, angle, 0);

                    if (buildingCopy != null && buildingOriginal == null)
                    {
                        buildingCopy.transform.eulerAngles -= new Vector3(0, angle, 0);
                        buildingCopy.transform.localPosition = new Vector3(0, LiftCurrentHeight + BlockMesh.HalfBlockSize, 0); ;
                    }
                }
            }
        }

        private void UpdateLift()
        {
            LiftTop.localPosition = new Vector3(0, LiftCurrentHeight, 0);

            LiftCenter.localPosition = (LiftTop.localPosition + LiftBottom.localPosition) / 2f;

            var centerScale = LiftTop.localPosition.y - LiftBottom.localPosition.y;

            LiftCenter.localScale = new Vector3(LiftCenter.localScale.x, centerScale * 2.2f, LiftCenter.localScale.z);
        }
    }*/
}