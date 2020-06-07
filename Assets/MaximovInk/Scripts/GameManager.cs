using System;
using UnityEngine;

namespace MaximovInk
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public ModLoader ModLoader;

        public Transform ModsUIParent;
        public ModUIElement ModUIPrefab;

        public Player LocalPlayer;

        public event Action OnUpdate;

        public event Action OnFixedUpdate;

        public event Action OnLateUpdate;

        public GameObject GameUI;
        public GameObject EditorUI;

        private void Awake()
        {
            instance = this;
            LocalPlayer = FindObjectOfType<Player>();

            ModLoader = new ModLoader();

            ModLoader.onModsLoaded += OnModsLoaded;

            ModLoader.LoadMods();
        }

        private void OnModsLoaded()
        {
            foreach (var mod in ModLoader.mods)
            {
                var modUI = Instantiate(ModUIPrefab, ModsUIParent);
                modUI.Init(mod.Key);
                modUI.OnToggle += () =>
                {
                    mod.Value.IsEnabled = !mod.Value.IsEnabled;
                    if (mod.Value.IsEnabled)
                    {
                        mod.Value.OnEnable();
                    }
                    else
                    {
                        mod.Value.OnDisable();
                    }

                    modUI.ActiveToggleText.text = mod.Value.IsEnabled ? "Enabled" : "Disabled";
                };
            }
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }
    }
}