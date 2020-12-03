using System;
using UnityEngine;
using UnityEngine.UI;

namespace MaximovInk.UI
{
    [Flags]
    public enum PanelType
    {
        None = 0,
        LeftTop = 1,
        RightTop = 2,
        LeftBottom = 4,
        RightBottom = 8
    }

    public class RoundedPanel : MaskableGraphic
    {
        public int segments = 5;
        public float borderWidth = 5f;

        public PanelType Type;

        private void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax)
        {
            int startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, Vector2.zero);
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, Vector2.zero);
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, Vector2.zero);
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, Vector2.zero);

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        private Vector2 AngleToDirection(float angleDeg)
        {
            return new Vector2(Mathf.Sin(angleDeg * Mathf.Deg2Rad), Mathf.Cos(angleDeg * Mathf.Deg2Rad));
        }

        private void AddTriangle(VertexHelper vertexHelper, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            int startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(v1, color, Vector2.zero);
            vertexHelper.AddVert(v2, color, Vector2.zero);
            vertexHelper.AddVert(v3, color, Vector2.zero);

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        }

        private void AddCorner(VertexHelper vh, Vector2 min, Vector2 max)
        {
            float stepSize = 90f / segments;

            var last = min;

            var delta = max - min;

            for (int i = 0; i < segments + 1; i++)
            {
                var angle = stepSize * i;

                var dir = AngleToDirection(angle);

                var localDir = new Vector2(min.x + (dir.x * delta.x), min.y + (dir.y * delta.y));

                AddTriangle(vh, last, localDir, min);

                last = localDir;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var min = new Vector2(0, 0) - rectTransform.pivot;
            min.x *= rectTransform.rect.width;
            min.y *= rectTransform.rect.height;

            var size = rectTransform.rect.size;
            var max = min + size;

            if (borderWidth <= 0)
            {
                AddQuad(vh,
                    min,
                    max
                    );

                return;
            }

            var localBorderW = Mathf.Min(borderWidth, rectTransform.rect.width / 2, rectTransform.rect.height / 2);

            var x0 = min.x;
            var x1 = x0 + localBorderW;
            var x2 = max.x - localBorderW;
            var x3 = max.x;

            var y0 = min.y;
            var y1 = y0 + localBorderW;
            var y2 = max.y - localBorderW;
            var y3 = max.y;

            //corner
            if ((Type & PanelType.LeftBottom) != 0)
                AddCorner(vh, new Vector2(x1, y1), new Vector2(x0, y0));
            else
                AddQuad(vh, new Vector2(x1, y1), new Vector2(x0, y0));

            //corner
            if ((Type & PanelType.RightBottom) == PanelType.RightBottom)
                AddCorner(vh, new Vector2(x2, y1), new Vector2(x3, y0));
            else
                AddQuad(vh, new Vector2(x2, y1), new Vector2(x3, y0));

            //corner
            if ((Type & PanelType.LeftTop) != 0)
                AddCorner(vh, new Vector2(x1, y2), new Vector2(x0, y3));
            else
                AddQuad(vh, new Vector2(x1, y2), new Vector2(x0, y3));

            //corner
            if ((Type & PanelType.RightTop) == PanelType.RightTop)
                AddCorner(vh, new Vector2(x2, y2), new Vector2(x3, y3));
            else
                AddQuad(vh, new Vector2(x2, y2), new Vector2(x3, y3));

            AddQuad(vh,
                  new Vector2(x1, y1),
                  new Vector2(x2, y2)
                  );

            AddQuad(vh,
                  new Vector2(x0, y1),
                  new Vector2(x1, y2)
                  );

            AddQuad(vh,
              new Vector2(x2, y1),
              new Vector2(x3, y2)
              );

            AddQuad(vh,
            new Vector2(x1, y2),
            new Vector2(x2, y3)
            );

            AddQuad(vh,
            new Vector2(x1, y0),
            new Vector2(x2, y1)
            );
        }
    }
}