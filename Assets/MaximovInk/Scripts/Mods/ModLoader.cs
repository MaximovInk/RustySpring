using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.IO;

namespace MaximovInk
{
    [System.Serializable]
    public class ModInfo
    {
        public string Name;
        public string Description;
        public string DllPath;
        public string IconPath;
        public string Author;
        public string Version;
    }

    public class ModLoader
    {
        public Dictionary<ModInfo, IMod> mods = new Dictionary<ModInfo, IMod>();

        private string ModsPath => Application.dataPath + "/../mods/";

        public event Action onModsLoaded;

        public void UnloadCurrentMods()
        {
            if (mods.Count > 0)
            {
                foreach (var mod in mods)
                {
                    mod.Value.OnDisable();
                    mod.Value.OnUnload();
                }
                mods.Clear();
            }
        }

        public void LoadMods()
        {
            if (!Directory.Exists(ModsPath))
                Directory.CreateDirectory(ModsPath);

            var directoryInfo = Directory.GetDirectories(ModsPath);

            for (int i = 0; i < directoryInfo.Length; i++)
            {
                var modInfoPath = directoryInfo[i] + "/modInfo.json";
                if (!File.Exists(modInfoPath))
                    continue;

                var modInfo = JsonUtility.FromJson<ModInfo>(File.ReadAllText(modInfoPath));

                if (modInfo == null)
                    continue;

                if (modInfo.DllPath == string.Empty)
                    continue;

                var mod = LoadMod(directoryInfo[i] + "/" + modInfo.DllPath);

                if (mod != null)
                    mods.Add(modInfo, mod);
            }

            onModsLoaded?.Invoke();
        }

        public void CreateModTemplete(string name)
        {
            var jsonData = JsonUtility.ToJson(new ModInfo() { Name = name, Author = "Unknown", Description = "Templete for mod", IconPath = string.Empty, Version = "0.0.1" });

            var directory = Directory.CreateDirectory(ModsPath + "/" + name + "/");
            using (var stream = File.CreateText(directory.FullName + "/modInfo.json"))
            {
                stream.Write(jsonData);
            }
        }

        private IMod LoadMod(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("Error loading, file not exists. Path :" + path);
                return null;
            }

            Debug.Log("loading from " + path);
            Assembly assembly = Assembly.LoadFrom(path);
            Debug.Log("loading");

            var type = Array.
                        Find(assembly.GetTypes(), t => t.GetInterfaces().
                        Any(i => i.FullName == typeof(IMod).FullName));

            if (type != null)
            {
                return assembly.CreateInstance(type.FullName) as IMod;
            }

            return null;
        }
    }
}