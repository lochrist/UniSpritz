using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class KenneyGame : SpritzGame
{
    SpriteId[] sprites;
    public void Initialize()
    {        
        var layer = Spritz.CreateLayer("RandomImages/1bit-kennynl");
        sprites = Spritz.GetSprites(layer);
    }

    public void Update()
    {

    }

    public void Render()
    {
        Spritz.DrawSprite(sprites[3], 0, 0);

        /*
        var index = 0;
        for(var i = 0; i < 8; ++i)
        {
            for (var j = 0; j < 8; ++j)
            {
                Spritz.DrawSprite(sprites[i], i / 2f, j / 2f);
                ++index;
            }
        }
        */
    }
}
