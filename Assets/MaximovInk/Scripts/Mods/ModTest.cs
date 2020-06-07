using UnityEngine;
using MaximovInk;

public class ModTest : MonoBehaviour
{
    private string path = @"C:\Users\maxim\Desktop\lift.obj";
    private string dllPath = "mods/1.dll";

    private void Start()
    {
        ModelLoader.Load(path, Vector3.one, new ObjImporter());
        //ModLoader modLoader = new ModLoader();
        //modLoader.CreateModTemplete("templete");
    }
}