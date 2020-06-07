using System;
using UnityEngine;
using UnityEngine.UI;

namespace MaximovInk
{
    public class ModUIElement : MonoBehaviour
    {
        public Text Name;
        public Button ActiveToggle;
        public Text ActiveToggleText;

        public event Action OnToggle;

        public void Init(ModInfo info)
        {
            Name.text = info.Name;

            ActiveToggle.onClick.AddListener(() => OnToggle?.Invoke());
        }
    }
}