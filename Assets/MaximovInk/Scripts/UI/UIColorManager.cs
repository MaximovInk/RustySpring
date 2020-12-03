using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MaximovInk
{
    public class UIColorManager : MonoBehaviour
    {
        public UIScheme scheme;

        public Color tint { get => scheme.tint; set => scheme.tint = value; }

        public static UIColorManager instance;

        public void UpdateChange()
        {
            OnColorChange?.Invoke();
        }

        private void Awake()
        {
            instance = this;
        }

        public event Action OnColorChange;
    }
}