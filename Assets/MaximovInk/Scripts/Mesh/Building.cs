using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using MessagePack;

namespace MaximovInk
{
    public class Building : MonoBehaviour
    {
        public List<BuildingLayer> layers = new List<BuildingLayer>();

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

        private void UpdateLayerStates()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].FreezeAll = isFreeze;
                // layers[i].transform.localPosition = Vector3.zero;
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
                        layer.AddBlock(block.Position, TileDatabase.GetBlock(block.Name), block.parameters);
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