using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MaximovInk
{
    public class EditorManager : MonoBehaviour
    {
        public static EditorManager instance;

        public Workbench CurrentWorkbench { get; set; }

        public Material LayersModeMaterial;

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
    }
}