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

        public static void DrawFilledRectangle(this Layer layer, RectInt rectangle, Color32 color)
        {
            layer.DrawPixels(rectangle.x, rectangle.y, rectangle.width, rectangle.height, color);
        }

        public static void DrawLine(this Layer layer, Vector2Int start, Vector2Int end, Color32 color)
        {
            Line(layer, start.x, start.y, end.x, end.y, color);
        }

        public static void DrawLine(this Layer layer, int x0, int y0, int x1, int y1, Color32 color)
        {
            Line(layer, x0, y0, x1, y1, color);
        }

        public static void DrawLineAS(this Layer layer, int x0, int y0, int x1, int y1, Color32 color, int wd)
        {
            // From http://members.chello.at/~easyfilter/bresenham.html
            // TODO: original code handles anti aliasing. Could potentially be simplified.

            var dx = Mathf.Abs(x1 - x0);
            var sx = x0 < x1 ? 1 : -1;
            int dy = Mathf.Abs(y1 - y0);
            var sy = y0 < y1 ? 1 : -1;
            int err = dx - dy, e2, x2, y2;                          /* error value e_xy */
            float ed = dx + dy == 0 ? 1 : Mathf.Sqrt((float)dx * dx + (float)dy * dy);
            for (wd = (wd + 1) / 2; ;)
            {                                   /* pixel loop */
                layer.DrawPixel(x0, y0, color);
                e2 = err; x2 = x0;
                if (2 * e2 >= -dx)
                {                                           /* x step */
                    for (e2 += dy, y2 = y0; e2 < ed * wd && (y1 != y2 || dx > dy); e2 += dx)
                        layer.DrawPixel(x0, y2 += sy, color);
                    if (x0 == x1)
                        break;
                    e2 = err;
                    err -= dy;
                    x0 += sx;
                }
                if (2 * e2 <= dy)
                {                                            /* y step */
                    for (e2 = dx - e2; e2 < ed * wd && (x1 != x2 || dx < dy); e2 += dy)
                        layer.DrawPixel(x2 += sx, y0, color);
                    if (y0 == y1)
                        break;
                    err += dx;
                    y0 += sy;
                }
            }
        }

        public static void DrawLine(this Layer layer, int x1, int y1, int x2, int y2, Color32 color, int wd)
        {
            // From https://saideepdicholkar.blogspot.com/2017/04/bresenhams-line-algorithm-thick-line.html
            if (wd == 1)
            {
                layer.DrawLine(x1, y1, x2, y2, color);
                return;
            }
                

            if ((y2 - y1) / (x2 - x1) < 1)
            {
                var wy = (wd - 1) * Mathf.Sqrt(Mathf.Pow((x2 - x1), 2) + Mathf.Pow((y2 - y1), 2)) / (2 * Mathf.Abs(x2 - x1));
                for (var i = 0; i < wy; i++)
                {
                    layer.DrawLine(x1, y1 - i, x2, y2 - i, color);
                    layer.DrawLine(x1, y1 + i, x2, y2 + i, color);
                }
            }
            else
            {
                var wx = (wd - 1) * Mathf.Sqrt(Mathf.Pow((x2 - x1), 2) + Mathf.Pow((y2 - y1), 2)) / (2 * Mathf.Abs(y2 - y1));
                for (var i = 0; i < wx; i++)
                {
                    layer.DrawLine(x1 - i, y1, x2 - i, y2, color);
                    layer.DrawLine(x1 + i, y1, x2 + i, y2, color);
                }
            }
        }

        private static void Circle(Layer layer, int x, int y, int radius, Color32 color, bool filled = false)
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

        private static void PlotCircle(Layer layer, int cx, int x, int cy, int y, Color32 color)
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

        private static void ScanLineCircle(Layer layer, int cx, int x, int cy, int y, Color32 color)
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
