using UnityEngine;
using System;

namespace UniMini
{
    public static class EffectsFactory
    {
        public static Effect CreateDissolve(int w, int h, Color32 color, float duration)
        {
            var dissolveOrder = new int[w * h];
            for (var i = 0; i < dissolveOrder.Length; ++i)
                dissolveOrder[i] = i;
            ExampleUtils.Shuffle(dissolveOrder);

            Action<Effect> update = effect =>
            {
                for (var i = effect.ticker.lastFrameIndex < 0 ? 0 : effect.ticker.lastFrameIndex; i < effect.ticker.frameIndex; ++i)
                {
                    effect.buffer[dissolveOrder[i]] = color;
                }
            };

            return new Effect(dissolveOrder.Length / duration, duration, dissolveOrder.Length, Color.clear)
            {
                updateHandler = update,
                drawHandler = (effect, x, y) => Spritz.DrawPixels(x, y, w, h, effect.buffer)
            };
        }

        public static Effect CreateLeftRight(int w, int h, Color32 color, float duration)
        {
            Action<Effect> update = effect =>
            {
                for (var i = effect.ticker.lastFrameIndex < 0 ? 0 : effect.ticker.lastFrameIndex; i < effect.ticker.frameIndex; ++i)
                {
                    for (var j = 0; j < h; ++j)
                    {
                        effect.buffer[j * w + i] = color;
                    }
                }
            };

            return new Effect(w / duration, duration, w * h, Color.clear)
            {
                updateHandler = update,
                drawHandler = (effect, x, y) => Spritz.DrawPixels(x, y, w, h, effect.buffer)
            };
        }
    }
}
