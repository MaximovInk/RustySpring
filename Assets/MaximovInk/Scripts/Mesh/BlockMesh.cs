using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace MaximovInk
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody))]
    public class BlockMesh : MonoBehaviour
    {/*
        [HideInInspector]
        public BlockMeshData Data = new BlockMeshData();

        private readonly List<BoxCollider> colliders = new List<BoxCollider>();
        private readonly List<Tuple<Vector3, Vector3>> boxes = new List<Tuple<Vector3, Vector3>>();
        private readonly List<string> materialSubMeshes = new List<string>();
        private readonly List<BlockMeshData> dataToSplit = new List<BlockMeshData>();

        private MeshData meshData;

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;

        public bool CanEditStructure { get; set; } = true;
        public bool CanMoveByLift { get; set; } = true;
        public bool CanEditLogic { get; set; } = true;
        public bool DestroyIfEmpty { get; set; } = true;
        public bool IsStatic { get; set; }

        public bool IsKinematic
        {
            get => Rigidbody.isKinematic; set
            {
                Rigidbody.isKinematic = value;
                OnSetKinematic?.Invoke(value);
            }
        }

        public Vector3 Velocity
        {
            get => Rigidbody.velocity;
            set
            {
                Rigidbody.velocity = value;
                OnSetVelocity?.Invoke(value);
            }
        }

        public Vector3 AngularVelocity
        {
            get => Rigidbody.angularVelocity;
            set
            {
                Rigidbody.angularVelocity = value;
                OnSetAVelocity?.Invoke(value);
            }
        }

        public bool FreezeAll { set => Rigidbody.constraints = value ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None; }

        private Rigidbody Rigidbody { get; set; }

        public const float kBlockSize = 0.25f;

        public event Action<bool> OnSetPreview;

        public event Action<bool> OnSetKinematic;

        public event Action<Vector3> OnSetVelocity;

        public event Action<Vector3> OnSetAVelocity;

        public event Action<LayerMask> OnLayerChange;

        private Material previewMaterial;

        private bool isDirty;
        private bool applyMesh;
        private bool applyColliders;
        private bool needSplit;

        private bool isPreview;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            Rigidbody = GetComponent<Rigidbody>();

            previewMaterial = MaterialDatabase.GetMaterial("preview");
        }

        private void Start()
        {
            InitMesh();

            isDirty = true;

            FreezeAll = true;
        }

        private readonly object locker = new object();

        private void Update()
        {
            if (needSplit)
            {
                lock (locker)
                {
                    needSplit = false;
                    // Debug.Log("split layers");
                    SplitLayers();
                    isDirty = true;
                }
            }

            if (isDirty)
            {
                isDirty = false;

                if (IsEmpty() && DestroyIfEmpty)
                {
                    Destroy(gameObject);
                    return;
                }

                //Debug.Log("calc sub mesh ids");
                CalculateSubMeshIDs();

                //Debug.Log("start generate");
                new Thread(Generate).Start();
            }

            if (applyMesh)
            {
                applyMesh = false;

                //Debug.Log("apply mesh");
                ApplyMesh();
            }

            if (applyColliders)
            {
                applyColliders = false;

                //Debug.Log("apply colliders");
                ApplyColliders();
            }
        }

        private void Generate()
        {
            lock (locker)
            {
                // Debug.Log("pack split data");
                if (PackSplitData())
                    return;

                // Debug.Log("clear mesh");
                ClearMesh();

                // Debug.Log("generate block mesh");
                GenerateBlockMesh();

                // Debug.Log("generate collider");
                GenerateColliderData();
            }
        }

        public void ClearAll()
        {
            for (int i = 0; i < Data.objects.Count; i++)
            {
                Destroy(Data.objects[i].Instance);
            }
            Data.objects.Clear();
            Data.blocks.Clear();
            isDirty = true;
        }

        public void ConnectJointToThis(Joint joint)
        {
            joint.connectedBody = Rigidbody;
        }

        public void SetPreview(bool isPreview)
        {
            this.isPreview = isPreview;
            OnSetPreview?.Invoke(isPreview);
        }

        public void SetLayer(LayerMask layer)
        {
            print(LayerMask.LayerToName(layer));
            gameObject.SetLayerRecursively(layer.value);

            OnLayerChange?.Invoke(layer);
        }

        public void SetIsDirty()
        {
            isDirty = true;
        }

        public void InitMesh()
        {
            meshData = new MeshData();
            meshFilter.mesh = meshData.GetMesh();
        }

        public void AddObject(string ObjectTile, Vector3Int Position, Vector3 normal)
        {
            lock (locker)
            {
                for (int i = 0; i < shape.Positions.Length; i++)
                {
                    if (shape.Positions.Any(n => n == (Position + shape.Positions[i])))
                    {
                        return;
                    }
                }

                var prefab = shape.GetGameObject();
                var instance = Instantiate(prefab, transform);
                instance.transform.localPosition = GridToLocal(Position);
                instance.transform.up = normal;

                Data.objects.Add(new ObjectTileData() { Position = Position, TileName = ObjectTile, Normal = normal });

                isDirty = true;
            }
        }

        public void RemoveObject(Vector3Int atPosition)
        {
            lock (locker)
            {
                for (int i = 0; i < Data.objects.Count; i++)
                {
                    for (int j = 0; j < Data.objects[i].Tile.Positions.Length; j++)
                    {
                        if (Data.objects[i].Tile.Positions[j] + Data.objects[i].Position == atPosition)
                        {
                            Destroy(Data.objects[i].Instance);
                            Data.objects.RemoveAt(i);
                            return;
                        }
                    }
                }
                isDirty = true;
            }
        }

        public void RemoveBlock(Vector3Int position)
        {
            lock (locker)
            {
                var block = Data.blocks.Find(n => n.Position == position);

                if (block == null)
                    return;

                Data.blocks.Remove(block);
                isDirty = true;
            }
        }

        public void AddBlock(Vector3Int position, BlockTile tile)
        {
            lock (locker)
            {
                if (Data.blocks.Any(n => n.Position == position))
                    return;

                Data.blocks.Add(new TileData() { Position = position, Tile = tile });
                isDirty = true;
            }
        }

        public TileData GetBlockAt(Vector3Int position)
        {
            return Data.blocks.FirstOrDefault(n => n.Position == position);
        }

        public static float HalfBlockSize => kBlockSize / 2f;

        public Vector3Int WorldToGrid(Vector3 worldPos)
        {
            var localPos = transform.InverseTransformPoint(worldPos);

            return LocalToGrid(localPos);
        }

        public Vector3Int LocalToGrid(Vector3 localPos)
        {
            return new Vector3Int(Mathf.FloorToInt(localPos.x / kBlockSize), Mathf.FloorToInt(localPos.y / kBlockSize), Mathf.FloorToInt(localPos.z / kBlockSize));
        }

        public Vector3 GridToLocal(Vector3Int gridPos)
        {
            return ((Vector3)gridPos * kBlockSize) + (Vector3.one * HalfBlockSize);
        }

        public Vector3 GridToWorld(Vector3Int gridPos)
        {
            return transform.TransformPoint(GridToLocal(gridPos));
        }

        public void CenterAllBlocksAndObject()
        {
            if (Data.blocks.Count == 0)
                return;

            if (Data.blocks.Any(n => n.Position == Vector3Int.zero))
                return;

            var nearestBlock = Data.blocks.OrderBy(n => n.Position.magnitude).First();

            var delta = nearestBlock.Position;

            for (int i = 0; i < Data.blocks.Count; i++)
            {
                Data.blocks[i].Position -= delta;
            }

            for (int i = 0; i < Data.objects.Count; i++)
            {
                Data.objects[i].Position -= delta;
            }

            isDirty = true;
        }

        private bool IsEmpty()
        {
            lock (locker)
                return Data.IsEmpty();
        }

        private void CalculateSubMeshIDs()
        {
            materialSubMeshes.Clear();

            var materials = new List<Material>();

            //materials.Clear();

            for (int i = 0; i < Data.blocks.Count; i++)
            {
                var matName = Data.blocks[i].Tile.MaterialName;

                if (!materialSubMeshes.Contains(matName))
                {
                    materialSubMeshes.Add(matName);
                    if (!isPreview)
                        materials.Add(MaterialDatabase.GetMaterial(matName));
                    else
                        materials.Add(previewMaterial);
                }
            }

            meshRenderer.materials = materials.ToArray();
        }

        private void ClearMesh()
        {
            lock (locker)
            {
                meshData.Clear();
            }
        }

        private void ApplyMesh()
        {
            lock (locker)
            {
                meshData.ApplyToMesh();
            }
        }

        private void ApplyColliders()
        {
            lock (locker)
            {
                FreezeAll = true;

                for (int i = 0; i < colliders.Count; i++)
                {
                    Destroy(colliders[i]);
                }

                colliders.Clear();

                for (int i = 0; i < boxes.Count; i++)
                {
                    var bCollider = gameObject.AddComponent<BoxCollider>();

                    var min = boxes[i].Item1 * kBlockSize;
                    var max = boxes[i].Item2 * kBlockSize;

                    var size = max - min;
                    var center = min + (size / 2f);

                    bCollider.size = size;
                    bCollider.center = center;
                    bCollider.hideFlags = HideFlags.HideInInspector;

                    colliders.Add(bCollider);
                }

                FreezeAll = IsStatic;
            }
        }

        private void SplitLayers()
        {
            for (int j = 0; j < dataToSplit.Count; j++)
            {
                var data = dataToSplit[j];

                var blockMesh = new GameObject("New mesh").AddComponent<BlockMesh>();
                blockMesh.Data = data;

                blockMesh.transform.position = transform.position;
                blockMesh.transform.rotation = transform.rotation;

                //blockMesh.CenterAllBlocksAndObject();
            }
        }

        private bool PackSplitData()
        {
            connected.Clear();
            dataToSplit.Clear();

            while (Data.blocks.Count > 0)
            {
                CheckConnectedBlock(Data.blocks[0].Position);

                if (connected.Count == Data.blocks.Count)
                {
                    break;
                }
                else
                {
                    // Debug.Log(connected.Count + " why " + Data.blocks.Count);
                    var newMesh = new BlockMeshData();

                    newMesh.blocks = new List<TileData>(connected);

                    dataToSplit.Add(newMesh);

                    Data.blocks = Data.blocks.Except(connected).ToList();

                    connected.Clear();
                }
            }

            needSplit = dataToSplit.Count > 0;

            return needSplit;
        }

        private readonly List<TileData> connected = new List<TileData>();

        private void CheckConnectedBlock(Vector3Int position)
        {
            if (connected.Any(z => z.Position == position))
                return;

            var block = Data.blocks.Find(n => n.Position == position);
            if (block != null)
            {
                connected.Add(block);

                CheckConnectedBlock(position + new Vector3Int(1, 0, 0));
                CheckConnectedBlock(position + new Vector3Int(-1, 0, 0));
                CheckConnectedBlock(position + new Vector3Int(0, 1, 0));
                CheckConnectedBlock(position + new Vector3Int(0, -1, 0));
                CheckConnectedBlock(position + new Vector3Int(0, 0, 1));
                CheckConnectedBlock(position + new Vector3Int(0, 0, -1));
            }
        }

        private void GenerateBlockMesh()
        {
            applyMesh = false;
            var blockSizeV = Vector3.one * kBlockSize;

            for (int i = 0; i < Data.blocks.Count; i++)
            {
                Vector3Int position = Data.blocks[i].Position;

                Vector3 vMin = (Vector3)Data.blocks[i].Position * kBlockSize;
                Vector3 vMax = vMin + blockSizeV;

                int id = materialSubMeshes.IndexOf(Data.blocks[i].Tile.MaterialName);

                // var value = (Mathf.Abs((position.x / 2) + (position.y / 2) + (position.z / 2)) % 4) / 4f;

                var tilingValue = 8f;

                var valueX = position.x % (int)tilingValue / tilingValue;
                var valueY = position.y % (int)tilingValue / tilingValue;
                var valueZ = position.z % (int)tilingValue / tilingValue;

                Vector4 uv = Data.blocks[i].Tile.UV;

                var step = new Vector2(uv.z - uv.x, uv.w - uv.y);

                var blockOffset = 1 / tilingValue;

                var uv1 = new Vector4(
                    uv.x + (valueX * step.x),
                    uv.y + (valueZ * step.y),
                    uv.x + ((valueX + blockOffset) * step.x),
                    uv.y + ((valueZ + blockOffset) * step.y));
                var uv2 = new Vector4(
                    uv.x + (valueX * step.x),
                    uv.y + (valueY * step.y),
                    uv.x + ((valueX + blockOffset) * step.x),
                    uv.y + ((valueY + blockOffset) * step.y));
                var uv3 = new Vector4(
                    uv.x + (valueZ * step.x),
                    uv.y + (valueY * step.y),
                    uv.x + ((valueZ + blockOffset) * step.x),
                    uv.y + ((valueY + blockOffset) * step.y));

                var uv4 = new Vector4(
                    uv1.x,
                    uv1.w,
                    uv1.z,
                    uv1.y);

                var uv5 = new Vector4(uv2.z, uv2.y, uv2.x, uv2.w);

                var uv6 = new Vector4(uv3.z, uv3.y, uv3.x, uv3.w);

                //int id = (Mathf.Abs(position.x / 2 + position.y / 2 + position.z / 2)) % 2 == 0 ? 1 : materialSubMeshes.IndexOf(Data.blocks[i].Tile.MaterialName);
                if (GetBlockAt(new Vector3Int(position.x, position.y - 1, position.z)) == null)
                {
                    meshData.AddQuad(
                        new Vector3(vMin.x, vMin.y, vMax.z),
                        new Vector3(vMin.x, vMin.y, vMin.z),
                        new Vector3(vMax.x, vMin.y, vMin.z),
                        new Vector3(vMax.x, vMin.y, vMax.z),
                        uv4,
                        Color.white,
                        id
                    );
                }
                if (GetBlockAt(new Vector3Int(position.x, position.y + 1, position.z)) == null)
                {
                    meshData.AddQuad(
                        new Vector3(vMin.x, vMax.y, vMin.z),
                        new Vector3(vMin.x, vMax.y, vMax.z),
                        new Vector3(vMax.x, vMax.y, vMax.z),
                        new Vector3(vMax.x, vMax.y, vMin.z),
                        uv1,
                        Color.white,
                        id
                    );
                }

                if (GetBlockAt(new Vector3Int(position.x, position.y, position.z - 1)) == null)
                {
                    meshData.AddQuad(
                        new Vector3(vMin.x, vMin.y, vMin.z),
                        new Vector3(vMin.x, vMax.y, vMin.z),
                        new Vector3(vMax.x, vMax.y, vMin.z),
                        new Vector3(vMax.x, vMin.y, vMin.z),
                        uv2,
                        Color.white,
                        id
                    );
                }
                if (GetBlockAt(new Vector3Int(position.x, position.y, position.z + 1)) == null)
                {
                    meshData.AddQuad(
                        new Vector3(vMax.x, vMin.y, vMax.z),
                        new Vector3(vMax.x, vMax.y, vMax.z),
                        new Vector3(vMin.x, vMax.y, vMax.z),
                        new Vector3(vMin.x, vMin.y, vMax.z),
                        uv5,
                        Color.white,
                        id
                    );
                }

                if (GetBlockAt(new Vector3Int(position.x - 1, position.y, position.z)) == null)
                {
                    meshData.AddQuad(
                        new Vector3(vMin.x, vMin.y, vMax.z),
                        new Vector3(vMin.x, vMax.y, vMax.z),
                        new Vector3(vMin.x, vMax.y, vMin.z),
                        new Vector3(vMin.x, vMin.y, vMin.z),
                        uv6,
                        Color.white,
                        id
                    );
                }
                if (GetBlockAt(new Vector3Int(position.x + 1, position.y, position.z)) == null)
                {
                    meshData.AddQuad(
                        new Vector3(vMax.x, vMin.y, vMin.z),
                        new Vector3(vMax.x, vMax.y, vMin.z),
                        new Vector3(vMax.x, vMax.y, vMax.z),
                        new Vector3(vMax.x, vMin.y, vMax.z),
                        uv3,
                        Color.white,
                        id
                    );
                }
            }

            applyMesh = true;
        }

        private void GenerateColliderData()
        {
            applyColliders = false;
            boxes.Clear();
            List<Vector3Int> calculatedIds = new List<Vector3Int>();

            while (true)
            {
                if (calculatedIds.Count >= Data.blocks.Count)
                    break;

                var block = Data.blocks.FirstOrDefault(
                    n => calculatedIds.All(element =>
                    {
                        return element != n.Position;
                    }
                    ));

                if (block == null)
                {
                    break;
                }

                var startPos = block.Position;
                var endPos = block.Position;
                var temp = new List<Vector3Int>();

                ///X STRETCH
                while (true)
                {
                    var iblock = GetBlockAt(new Vector3Int(endPos.x + 1, endPos.y, endPos.z));

                    if (iblock == null || calculatedIds.Contains(iblock.Position))
                    {
                        break;
                    }
                    else
                    {
                        endPos.x++;
                    }
                }

                while (true)
                {
                    var iblock = GetBlockAt(new Vector3Int(startPos.x - 1, startPos.y, startPos.z));

                    if (iblock == null || calculatedIds.Contains(iblock.Position))
                    {
                        break;
                    }
                    else
                    {
                        startPos.x--;
                    }
                }

                ///Y STRETCH

                while (true)
                {
                    for (int ix = startPos.x; ix <= endPos.x; ix++)
                    {
                        var iblock = GetBlockAt(new Vector3Int(ix, endPos.y + 1, endPos.z));

                        if (iblock == null || calculatedIds.Contains(iblock.Position))
                        {
                            goto continue1;
                        }
                    }

                    endPos.y++;
                }
            continue1:

                while (true)
                {
                    for (int ix = startPos.x; ix <= endPos.x; ix++)
                    {
                        var iblock = GetBlockAt(new Vector3Int(ix, startPos.y - 1, startPos.z));

                        if (iblock == null || calculatedIds.Contains(iblock.Position))
                        {
                            goto continue2;
                        }
                    }

                    startPos.y--;
                }
            continue2:

                ///Z STRETCH

                while (true)
                {
                    for (int ix = startPos.x; ix <= endPos.x; ix++)
                    {
                        for (int iy = startPos.y; iy <= endPos.y; iy++)
                        {
                            var iblock = GetBlockAt(new Vector3Int(ix, iy, endPos.z + 1));

                            if (iblock == null || calculatedIds.Contains(iblock.Position))
                            {
                                goto continue3;
                            }
                        }
                    }

                    endPos.z++;
                }
            continue3:

                while (true)
                {
                    for (int ix = startPos.x; ix <= endPos.x; ix++)
                    {
                        for (int iy = startPos.y; iy <= endPos.y; iy++)
                        {
                            var iblock = GetBlockAt(new Vector3Int(ix, iy, startPos.z - 1));

                            if (iblock == null || calculatedIds.Contains(iblock.Position))
                            {
                                goto continue4;
                            }
                        }
                    }

                    startPos.z--;
                }
            continue4:

                for (int ix = startPos.x; ix <= endPos.x; ix++)
                {
                    for (int iy = startPos.y; iy <= endPos.y; iy++)
                    {
                        for (int iz = startPos.z; iz <= endPos.z; iz++)
                        {
                            var iblock = GetBlockAt(new Vector3Int(ix, iy, iz));
                            if (iblock != null)
                                calculatedIds.Add(iblock.Position);
                        }
                    }
                }

                boxes.Add(new Tuple<Vector3, Vector3>(startPos, endPos + Vector3Int.one));
            }

            applyColliders = true;
        }*/
    }
}