using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    public static class RectUtil
    {
        public static RectInt CutLeft(this ref RectInt r, int v)
        {
            var minX = r.xMin;
            r.xMin = Mathf.Min(r.xMax, r.xMin + v);
            return new RectInt(minX, r.yMin, r.xMin - minX, r.height);
        }

        public static RectInt CutRight(this ref RectInt r, int v)
        {
            var maxX = r.xMax;
            r.xMax = Mathf.Max(r.xMin, r.xMax - v);
            return new RectInt(r.xMax, r.yMin, maxX - r.xMax, r.height);
        }

        public static RectInt CutTop(this ref RectInt r, int v)
        {
            // Spritz top is zero. Bottom is H

            var minY = r.yMin;
            r.yMin = Mathf.Min(r.yMin + v, r.yMax);
            return new RectInt(r.xMin, minY, r.width, r.yMin - minY);

        }

        public static RectInt CutBottom(this ref RectInt r, int v)
        {
            // Spritz top is zero. Bottom is H

            var maxY = r.yMax;
            r.yMax = Mathf.Max(r.yMin, r.yMax - v);
            return new RectInt(r.xMin, r.yMax, r.width, maxY - r.yMax);
        }

        public static RectInt GetLeft(this RectInt r, int v)
        {
            v = Mathf.Min(r.width, v);
            return new RectInt(r.xMin, r.yMin, v, r.height);
        }

        public static RectInt GetRight(this RectInt r, int v)
        {
            v = Mathf.Min(r.width, v);
            return new RectInt(r.xMax - v, r.yMin, v, r.height);
        }

        public static RectInt GetTop(this RectInt r, int v)
        {
            // Spritz top is zero. Bottom is H

            v = Mathf.Min(r.height, v);
            return new RectInt(r.xMin, r.yMin, r.width, v);
        }

        public static RectInt GetBottom(this RectInt r, int v)
        {
            // Spritz top is zero. Bottom is H

            v = Mathf.Min(r.height, v);
            return new RectInt(r.xMin, r.yMax - v, r.width, v);
        }

        public static RectInt AddLeft(this RectInt r, int v)
        {
            return new RectInt(r.xMin - v, r.yMin, r.width + v, r.height);
        }

        public static RectInt AddRight(this RectInt r, int v)
        {
            return new RectInt(r.xMin, r.yMin, r.width + v, r.height);
        }

        public static RectInt AddTop(this RectInt r, int v)
        {
            // Spritz top is zero. Bottom is H
            return new RectInt(r.xMin, r.yMin - v, r.width, r.height + v);
        }

        public static RectInt AddBottom(this RectInt r, int v)
        {
            // Spritz top is zero. Bottom is H
            return new RectInt(r.xMin, r.yMin, r.width, r.height + v);
        }

        public static RectInt Extend(this RectInt r, int v)
        {
            return new RectInt(r.xMin - v, r.yMin - v, r.width + 2*v, r.height + 2*v);
        }

        public static RectInt Contract(this RectInt r, int v)
        {
            var xMin = Mathf.Min(r.xMin + v, r.xMax);
            var xMax = Mathf.Max(r.xMin, r.xMax - v);
            var yMin = Mathf.Min(r.yMin + v, r.yMax);
            var yMax = Mathf.Max(r.yMin, r.yMax - v);
            return new RectInt(xMin, yMin, Mathf.Max(xMax - xMin, 0), Mathf.Max(yMax - yMin, 0));
        }

        /*
            v1: all sides
            v1 v2 : vertical | horizontal
            v1 v2 v3 : top | horizontal | bottom
            v1 v2 v3 v4: top | right | bottom | left
         */
        public static RectInt AddMargin(this RectInt r, int v)
        {
            return r.Extend(v);
        }

        public static RectInt AddMargin(this RectInt r, int v, int h)
        {
            return new RectInt(r.xMin - h, r.yMin - v, r.width + 2 * h, r.height + 2 * v);
        }

        public static RectInt AddMargin(this RectInt r, int t, int h, int b)
        {
            // Spritz top is zero. Bottom is H

            return new RectInt(r.xMin - h, r.yMin - t, r.width + 2 * h, r.height + t + b);
        }

        public static RectInt AddMargin(this RectInt r, int t, int right, int b, int l)
        {
            // Spritz top is zero. Bottom is H

            return new RectInt(r.xMin - l, r.yMin - t, r.width + right + l, r.height + t + b);
        }

        /*
            v1: all sides
            v1 v2 : vertical | horizontal
            v1 v2 v3 : top | horizontal | bottom
            v1 v2 v3 v4: top | right | bottom | left
         */
        public static RectInt AddPadding(this RectInt r, int v)
        {
            return r.Contract(v);
        }

        public static RectInt AddPadding(this RectInt r, int v, int h)
        {
            var xMin = Mathf.Min(r.xMin + h, r.xMax);
            var xMax = Mathf.Max(r.xMin, r.xMax - h);
            var yMin = Mathf.Min(r.yMin + v, r.yMax);
            var yMax = Mathf.Max(r.yMin, r.yMax - v);
            return new RectInt(xMin, yMin, Mathf.Max(xMax - xMin, 0), Mathf.Max(yMax - yMin, 0));
        }

        public static RectInt AddPadding(this RectInt r, int t, int h, int b)
        {
            // Spritz top is zero. Bottom is H

            var xMin = Mathf.Min(r.xMin + h, r.xMax);
            var xMax = Mathf.Max(r.xMin, r.xMax - h);
            var yMin = Mathf.Min(r.yMin + t, r.yMax);
            var yMax = Mathf.Max(r.yMin, r.yMax - b);
            return new RectInt(xMin, yMin, Mathf.Max(xMax - xMin, 0), Mathf.Max(yMax - yMin, 0));
        }

        public static RectInt AddPadding(this RectInt r, int t, int right, int b, int l)
        {
            // Spritz top is zero. Bottom is H

            var xMin = Mathf.Min(r.xMin + l, r.xMax);
            var xMax = Mathf.Max(r.xMin, r.xMax - right);
            var yMin = Mathf.Min(r.yMin + t, r.yMax);
            var yMax = Mathf.Max(r.yMin, r.yMax - b);
            return new RectInt(xMin, yMin, Mathf.Max(xMax - xMin, 0), Mathf.Max(yMax - yMin, 0));
        }

        public static RectInt CenterRect(this RectInt r, Vector2Int size)
        {
            var midX = (int)r.center.x;
            var midY = (int)r.center.y;
            var xMin = midX - size.x / 2;
            var yMin = midY - size.y / 2;
            return new RectInt(xMin, yMin, size.x, size.y);
        }
    }
}
