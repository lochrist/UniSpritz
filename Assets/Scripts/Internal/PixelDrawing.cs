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

        public static void DrawRectangle(this Layer layer, RectInt rectangle, Color color)
        {
            var x = rectangle.x;
            var y = rectangle.y;
            var height = rectangle.height;
            var width = rectangle.width;

            // top left to bottom left
            layer.DrawLine(x, y, x, y + height, color);

            // bottom left to bottom right
            layer.DrawLine(x, y + height, x + width, y + height, color);

            // bottom right to top right
            layer.DrawLine(x + width, y + height, x + width, y, color);

            // top right to top left
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

        private static void Line(Layer layer, int x0, int y0, int x1, int y1, Color color)
        {
            bool isSteep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (isSteep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            int deltax = x1 - x0;
            int deltay = Math.Abs(y1 - y0);

            int error = deltax / 2;
            int ystep;
            int y = y0;

            if (y0 < y1)
                ystep = 1;
            else
                ystep = -1;

            for (int x = x0; x < x1; x++)
            {
                if (isSteep)
                    layer.DrawPixel(y, x, color);
                else
                    layer.DrawPixel(x, y, color);

                error = error - deltay;
                if (error < 0)
                {
                    y = y + ystep;
                    error = error + deltax;
                }
            }
        }

        private static void Swap(ref int x, ref int y)
        {
            int temp = x;
            x = y;
            y = temp;
        }
    }


}
