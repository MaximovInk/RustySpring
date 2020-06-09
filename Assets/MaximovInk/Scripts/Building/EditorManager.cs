using System;
using UnityEngine;

namespace MaximovInk
{
    public class EditorManager : MonoBehaviour
    {
        public static EditorManager instance;

        public Workbench CurrentWorkbench { get; set; }

        public Material LayersModeMaterial;

        public event Action<bool> OnActiveChanged;

        private void Awake()
        {
            instance = this;
        }

        public void SetLayersMode(bool enabled)
        {
            if (CurrentWorkbench == null)
                return;

            CurrentWorkbench.Building.SetCustomMaterial(enabled ? LayersModeMaterial : null);
        }

        private bool toggleLayers = false;

        public void ToggleLayersMode()
        {
            toggleLayers = !toggleLayers;
            SetLayersMode(toggleLayers);
        }

        public void SetActive(bool val)
        {
            OnActiveChanged?.Invoke(val);
        }
    }
}