using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UniMini;
using UnityEngine;

public class ExampleUtils
{
    public static AnimSprite[] GetTinyMonsters(SpriteId[] allSprites)
    {
        var nbAnimSprites = allSprites.Length / 2;
        var sprites = new AnimSprite[nbAnimSprites];
        int spriteIndex = 0;
        int m_AnimSpriteIndex = 0;
        while (m_AnimSpriteIndex < nbAnimSprites)
        {
            // Process sprites by batch of 16/32 (2 lines on the sheet).
            // second animated sprite is at +16
            var spriteEnd = Mathf.Min(m_AnimSpriteIndex + 16, nbAnimSprites);
            for (; m_AnimSpriteIndex < spriteEnd; ++m_AnimSpriteIndex, ++spriteIndex)
            {
                sprites[m_AnimSpriteIndex] = new AnimSprite(4, new[] { allSprites[spriteIndex], allSprites[spriteIndex + 16] })
                {
                    loop = true
                };
            }
            spriteIndex += 16;
        }

        return sprites;
    }

    public static AnimSprite[] GetUfHeroes(SpriteId[] allSprites)
    {
        var nbSpritesPerAnimation = 4;
        var nbAnimSprites = allSprites.Length / nbSpritesPerAnimation;
        var sprites = new AnimSprite[nbAnimSprites];
        int spriteIndex = 0;
        int animSpriteIndex = 0;
        while (animSpriteIndex < nbAnimSprites)
        {
            sprites[animSpriteIndex] = new AnimSprite(4, new[] { allSprites[spriteIndex], allSprites[spriteIndex + 1], allSprites[spriteIndex + 2], allSprites[spriteIndex + 3] })
            {
                loop = true
            };
            animSpriteIndex++;
            spriteIndex += 4;
        }

        return sprites;
    }

    public static void Shuffle<T>(T[] ts)
    {
        var count = ts.Length;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
