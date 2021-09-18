using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class TinyDungeonGame : SpritzGame
{
    SpriteId[] m_Sprites;
    public void Initialize()
    {
        var layerId = Spritz.CreateLayer("RandomImages/tiny_dungeon_monsters");

        m_Sprites = Spritz.GetSprites(layerId);
    }

    public void Update()
    {        
    }

    public void Render()
    {
        Spritz.DrawSprite(m_Sprites[6], 1, 1);

        /*
        for (var i = 0; i < 8; ++i)
        {
            for (var j = 0; j < 8; ++j)
            {
                var spriteIndex = i * 8 + j;
                var spriteId = m_Sprites[spriteIndex];
                Spritz.DrawSprite(spriteId, i - 4, j - 4);
            }
        }
        */
    }
}
