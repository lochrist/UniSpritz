using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class TinyDungeon : SpritzGame
{
    SpriteId[] m_Sprites;
    public override void InitializeSpritz()
    {
        Spritz.CreateLayer("RandomImages/tiny_dungeon_monsters");
        m_Sprites = Spritz.GetSprites(0);
    }

    public override void UpdateSpritz()
    {
    }

    public override void DrawSpritz()
    {
        Spritz.currentLayerId = 0;
        Spritz.DrawSprite(m_Sprites[0], 0, 0);
        
    }
}
