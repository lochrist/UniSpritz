using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    enum SpaceType
    {
        Pixel,
        Circle,
        Rectangle,
        Line
    }

    internal struct ShapeDescriptor
    {
        public SpaceType type;
        public int x;
        public int y;
        public Vector2Int bounds;
        public bool filled;
    }

    internal class SpriteBuilder
    {
        List<ShapeDescriptor> m_Shapes;
        public SpriteBuilder()
        {
            m_Shapes = new List<ShapeDescriptor>();
        }

        public ShapeDescriptor AddCircle(int radius, bool filled, Color c, string name = "")
        {
            m_Shapes.Add(new ShapeDescriptor()
            {
                type = SpaceType.Circle,
                x = radius,
                filled = filled,
                bounds = new Vector2Int(radius * 2, radius * 2)
            });
            return m_Shapes[m_Shapes.Count - 1];
        }

        public ShapeDescriptor AddRectangle(int w, int h, bool filled, string name = "")
        {
            m_Shapes.Add(new ShapeDescriptor()
            {
                type = SpaceType.Rectangle,
                x = w,
                y = h,
                filled = filled,
                bounds = new Vector2Int(w, h)
            });
            return m_Shapes[m_Shapes.Count - 1];
        }

        public ShapeDescriptor AddPixel(string name = "")
        {
            m_Shapes.Add(new ShapeDescriptor()
            {
                type = SpaceType.Pixel,
                bounds = new Vector2Int(1, 1)
            });
            return m_Shapes[m_Shapes.Count - 1];
        }

        public ShapeDescriptor AddLine(int x, int y, string name = "")
        {
            m_Shapes.Add(new ShapeDescriptor()
            {
                type = SpaceType.Line,
                x = x,
                y = y,
                bounds = new Vector2Int(x, y)
            });
            return m_Shapes[m_Shapes.Count - 1];
        }
    }
}