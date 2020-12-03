using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MaximovInk
{
    public class UIEventColor : MonoBehaviour
    {
        public string key = "default";

        private Graphic graphic;
        private TextMeshProUGUI textMesh;

        private void Start()
        {
            graphic = GetComponent<Graphic>();
            textMesh = GetComponent<TextMeshProUGUI>();
            if (textMesh == null && graphic == null)
            {
                Destroy(gameObject);
                return;
            }

            var defColor = graphic != null ? graphic.color : textMesh.color;

            if (!UIColorManager.instance.scheme.colors.Any(n => n.Key == key))
            {
                UIColorManager.instance.scheme.colors.Add(new StyleC() { Key = key, Color = defColor });
            }

            UIColorManager.instance.OnColorChange += () =>
            {
                if (textMesh != null)
                    textMesh.color = UIColorManager.instance.scheme.colors.Find(n => n.Key == key).Color * UIColorManager.instance.tint;
                if (graphic != null)
                    graphic.color = UIColorManager.instance.scheme.colors.Find(n => n.Key == key).Color * UIColorManager.instance.tint;
            };

            UIColorManager.instance.UpdateChange();
        }
    }
}