using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MaximovInk
{
    [System.Serializable]
    public struct StyleC
    {
        public string Key;
        public Color Color;
    }

    [CreateAssetMenu(fileName = "new scheme", menuName = "UIScheme")]
    public class UIScheme : ScriptableObject
    {
        public List<StyleC> colors = new List<StyleC>();

        public Color tint = Color.white;
    }
}