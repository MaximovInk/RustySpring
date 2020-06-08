using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using MessagePack;

namespace MaximovInk
{
    public class Building : MonoBehaviour
    {
        private List<BuildingLayer> layers = new List<BuildingLayer>();

        private static string path => Application.dataPath + "/build0.blueprint";

        public bool Freeze
        {
            set
            {
                isFreeze = value;
                UpdateLayerStates();
            }
            get => isFreeze;
        }

        private bool isFreeze;
        private bool needUpdateLayerStates;

        public event System.Action<BuildingLayer> OnLayerRemove;

        public event System.Action<BuildingLayer> OnLayerAdd;

        private Material customMaterial;

        public BuildingLayer this[int i] { get { return layers[i]; } }

        public void CombineLayers(BuildingLayer from, BuildingLayer to)
        {
            if (to == from)
                return;

            if (!from.CanCombineWith(to) || !to.CanCombineWith(from))
                return;

            for (int i = 0; i < from.Data.blocks.Count; i++)
            {
                var block = from.Data.blocks[i];
                to.AddBlock(TileDatabase.GetBlock(block.Name), block.Position, block.parameters);
            }

            for (int i = 0; i < from.Data.objects.Count; i++)
            {
                var obj = from.Data.objects[i];
                to.AddObject(TileDatabase.GetObject(obj.Name), obj.Position, obj.Normal, obj.parameters);
            }

            RemoveLayer(from);
        }

        public int GetIndexOf(BuildingLayer layer)
        {
            return layers.IndexOf(layer);
        }

        public void RemoveLayer(BuildingLayer layer)
        {
            Destroy(layer.gameObject);
            layers.Remove(layer);
            OnLayerRemove?.Invoke(layer);
        }

        public void SetCustomMaterial(Material material)
        {
            customMaterial = material;
            needUpdateLayerStates = true;
        }

        private void UpdateLayerStates()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].FreezeAll = isFreeze;

                var mat = customMaterial != null ? new Material(customMaterial) : null;

                mat?.SetColor("_BaseColor", Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f));

                layers[i].CustomMaterial = mat;
            }
        }

        private void Update()
        {
            if (needUpdateLayerStates)
            {
                UpdateLayerStates();
                needUpdateLayerStates = false;
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                SaveData();
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                LoadData();
            }
        }

        public void DestoryAllLayers()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                Destroy(layers[i].gameObject);
            }
            layers.Clear();
        }

        public BuildingLayer AddNewLayer()
        {
            var newLayer = new GameObject().AddComponent<BuildingLayer>();
            newLayer.transform.SetParent(transform);
            newLayer.transform.localPosition = Vector3.zero;
            newLayer.transform.localRotation = Quaternion.identity;
            newLayer.Building = this;

            layers.Add(newLayer);

            OnLayerAdd?.Invoke(newLayer);

            needUpdateLayerStates = true;

            return newLayer;
        }

        public void SaveData()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].OnSerialize();
            }

            var datas = layers.Select(n => n.Data).ToArray();

            print(datas.Length);

            var bin = MessagePackSerializer.Serialize(datas);

            print(MessagePackSerializer.SerializeToJson(datas));

            File.WriteAllText(path, MessagePackSerializer.SerializeToJson(bin));

            using (var fs = File.Open(path, FileMode.OpenOrCreate))
            {
                fs.Write(bin, 0, bin.Length);
            }
        }

        public void LoadData()
        {
            if (!File.Exists(path))
                return;

            using (var fs = File.Open(path, FileMode.Open))
            {
                var datas = MessagePackSerializer.Deserialize<BlockMeshData[]>(fs);

                DestoryAllLayers();

                //TODO:PARAMETERS
                for (int i = 0; i < datas.Length; i++)
                {
                    var layer = AddNewLayer();

                    var data = datas[i];

                    for (int j = 0; j < data.blocks.Count; j++)
                    {
                        var block = data.blocks[j];
                        layer.AddBlock(TileDatabase.GetBlock(block.Name), block.Position, block.parameters);
                    }

                    for (int j = 0; j < data.objects.Count; j++)
                    {
                        var obj = data.objects[j];
                        layer.AddObject(TileDatabase.GetObject(obj.Name), obj.Position, obj.Normal, obj.parameters);
                    }
                }

                for (int i = 0; i < layers.Count; i++)
                {
                    layers[i].OnDeserialize();
                }
            }
        }
    }
}