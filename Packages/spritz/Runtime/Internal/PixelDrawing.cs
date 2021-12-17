using System;
using UnityEngine;

namespace UniMini
{
    internal static class PixelDrawing
    {
        public static void DrawCircle(this Layer layer, int x, int y, int radius, Color color)
        {
            Circle(layer, x, y, radius, color, false);
        }

        public static void DrawFilledCircle(this Layer layer, int x, int y, int radius, Color color)
        {
            Circle(layer, x, y, radius, color, true);
        }

        public static void DrawRectangle(this Layer layer, RectInt rectangle, Color color)
        {
            var x = rectangle.x;
            var y = rectangle.y;
            var height = rectangle.height - 1;
            var width = rectangle.width - 1;

            layer.DrawLine(x, y, x, y + height, color);
            layer.DrawLine(x, y + height, x + width, y + height, color);
            layer.DrawLine(x + width, y + height, x + width, y, color);
            layer.DrawLine(x + width, y, x, y, color);
        }

        public static void DrawFilledRectangle(this Layer layer, RectInt rectangle, Color color)
        {
            var colorsArray = new Color[rectangle.width * rectangle.height];
            for (int i = 0; i < colorsArray.Length; i++)
            {
                colorsArray[i] = color;
            }

            layer.DrawPixels(rectangle.x, rectangle.y, rectangle.width, rectangle.height, colorsArray);
        }

        public static void DrawLine(this Layer layer, Vector2Int start, Vector2Int end, Color color)
        {
            Line(layer, start.x, start.y, end.x, end.y, color);
        }

        public static void DrawLine(this Layer layer, int x0, int y0, int x1, int y1, Color color)
        {
            Line(layer, x0, y0, x1, y1, color);
        }

        private static void Circle(Layer layer, int x, int y, int radius, Color color, bool filled = false)
        {
            int cx = radius;
            int cy = 0;
            int radiusError = 1 - cx;

            while (cx >= cy)
            {
                if (!filled)
                {
                    PlotCircle(layer, cx, x, cy, y, color);
                }
                else
                {
                    ScanLineCircle(layer, cx, x, cy, y, color);
                }

                cy++;

                if (radiusError < 0)
                {
                    radiusError += 2 * cy + 1;
                }
                else
                {
                    cx--;
                    radiusError += 2 * (cy - cx + 1);
                }
            }
        }

        private static void PlotCircle(Layer layer, int cx, int x, int cy, int y, Color color)
        {
            layer.DrawPixel(cx + x, cy + y, color);
            layer.DrawPixel(cy + x, cx + y, color);
            layer.DrawPixel(-cx + x, cy + y, color);
            layer.DrawPixel(-cy + x, cx + y, color);
            layer.DrawPixel(-cx + x, -cy + y, color);
            layer.DrawPixel(-cy + x, -cx + y, color);
            layer.DrawPixel(cx + x, -cy + y, color);
            layer.DrawPixel(cy + x, -cx + y, color);
        }

        private static void ScanLineCircle(Layer layer, int cx, int x, int cy, int y, Color color)
        {
            layer.DrawLine(cx + x, cy + y, -cx + x, cy + y, color);
            layer.DrawLine(cy + x, cx + y, -cy + x, cx + y, color);
            layer.DrawLine(-cx + x, -cy + y, cx + x, -cy + y, color);
            layer.DrawLine(-cy + x, -cx + y, cy + x, -cx + y, color);
        }

        private static void Line(Layer layer, int x0, int y0, int x1, int y1, Color color)
        {
            // https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
            var dx = Mathf.Abs(x1 - x0);
            var sx = x0 < x1 ? 1 : -1;
            var dy = -Math.Abs(y1-y0);
            var sy = y0 < y1 ? 1 : -1;
            var err = dx + dy;
            while(true)
            {
                layer.DrawPixel(x0, y0, color);
                if (x0 == x1 && y0 == y1)
                    break;
                var e2 = 2 * err;
                if (e2>=dy)
                {
                    err += dy;
                    x0 += sx;
                }
                if (e2<=dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
    }
}
