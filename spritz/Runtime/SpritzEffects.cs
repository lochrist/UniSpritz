using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    public class Effect
    {
        public Color32[] buffer;
        public RectInt geometry;
        public Action<Effect> updateHandler;
        public Action<Effect> drawHandler;
        public AnimTick ticker;
        public Color32 resetColor;

        public Effect(float fps, float duration, RectInt geom, int pixelBufferSize, Color32 resetColor)
        {
            ticker = new AnimTick(fps, duration);
            buffer = new Color32[pixelBufferSize];
            geometry = geom;
            this.resetColor = resetColor;
            Reset();
        }

        public void Update()
        {
            if (ticker.isValid && ticker.isRunning)
            {
                ticker.Update();
                updateHandler?.Invoke(this);
            }
        }

        public void Draw()
        {
            if (drawHandler != null)
            {
                drawHandler.Invoke(this);
            }
            else
            {
                Spritz.DrawPixels(geometry.x, geometry.y, geometry.width, geometry.height, buffer);
            }
        }

        public void Reset()
        {
            for(var i = 0; i < buffer.Length; ++i)
            {
                buffer[i] = resetColor;
            }
            ticker.Reset();
        }
    }
}
