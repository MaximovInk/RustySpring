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
            print(layers.Count + " " + isFreeze);
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].FreezeAll = isFreeze;
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
                SaveToJson();
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                LoadFromJson();
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

            layers.Add(newLayer);

            needUpdateLayerStates = true;

            return newLayer;
        }

        public void SaveToJson()
        {
            var datas = layers.Select(n => n.Data).ToArray();

            print(datas.Length);

            var bin = MessagePackSerializer.Serialize(datas);

            print(MessagePackSerializer.SerializeToJson(bin));

            File.WriteAllText(path, MessagePackSerializer.SerializeToJson(bin));

            using (var fs = File.Open(path, FileMode.OpenOrCreate))
            {
                fs.Write(bin, 0, bin.Length);
            }
        }

        public void LoadFromJson()
        {
            if (!File.Exists(path))
                return;

            using (var fs = File.Open(path, FileMode.Open))
            {
                var datas = MessagePackSerializer.Deserialize<BlockMeshData[]>(fs);

                /*for (int i = 0; i < datas.Length; i++)
                {
                    print("->" + datas[i].blocks.Count);
                    for (int j = 0; j < datas.Length; j++)
                    {
                        var block = datas[i].blocks[j];
                        print("->->" + block.Name + " " + block.Position);
                    }
                    print("->" + datas[i].objects.Count);
                }*/
                DestoryAllLayers();

                //TODO:PARAMETERS
                for (int i = 0; i < datas.Length; i++)
                {
                    var layer = AddNewLayer();
                    var data = datas[i];

                    for (int j = 0; j < data.blocks.Count; j++)
                    {
                        var block = data.blocks[j];
                        layer.AddBlock(block.Position, TileDatabase.GetBlock(block.Name));
                    }

                    for (int j = 0; j < data.objects.Count; j++)
                    {
                        var obj = data.objects[j];
                        layer.AddObject(TileDatabase.GetObject(obj.Name), obj.Position, obj.Normal);
                    }
                }
            }
        }
    }
}