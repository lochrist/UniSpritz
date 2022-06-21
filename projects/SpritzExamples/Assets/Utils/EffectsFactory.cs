using UnityEngine;
using System;

namespace UniMini
{
    public static class EffectsFactory
    {
        public static Effect CreateDissolve(RectInt geom, Color32 color, float duration)
        {
            var dissolveOrder = new int[geom.width * geom.height];
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

            return new Effect(dissolveOrder.Length / duration, duration, geom, dissolveOrder.Length, Color.clear)
            {
                updateHandler = update
            };
        }

        public static Effect CreateLeftRight(RectInt geom, Color32 color, float duration)
        {
            Action<Effect> update = effect =>
            {
                for (var i = effect.ticker.lastFrameIndex < 0 ? 0 : effect.ticker.lastFrameIndex; i < effect.ticker.frameIndex; ++i)
                {
                    for (var j = 0; j < geom.height; ++j)
                    {
                        effect.buffer[j * geom.width + i] = color;
                    }
                }
            };

            return new Effect(geom.width / duration, duration, geom, geom.width * geom.height, Color.clear)
            {
                updateHandler = update
            };
        }
    }
}
