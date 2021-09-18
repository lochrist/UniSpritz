using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class TinyDungeonGame : SpritzGame
{
    SpriteId[] m_Sprites;
    SpriteId[] m_SpritesKenney;
    public void Initialize()
    {
        Spritz.CreateLayer("RandomImages/tiny_dungeon_monsters");
        Spritz.CreateLayer("RandomImages/tiny_dungeon_monsters");
        Spritz.CreateLayer("RandomImages/1bit-kennynl");
        Spritz.CreateLayer("RandomImages/1bit-kennynl");

        m_Sprites = Spritz.GetSprites(0);
        m_SpritesKenney = Spritz.GetSprites(2);
    }

    public void Update()
    {
    }

    public void Render()
    {
        Spritz.currentLayerId = 0;
        Spritz.DrawSprite(m_Sprites[6], 0, 0);
        Spritz.DrawSprite(m_Sprites[7], 16, 16);
        Spritz.DrawSprite(m_Sprites[8], 32, 32);
        Spritz.DrawSprite(m_Sprites[9], 64, 64);

        Spritz.currentLayerId = 1;
        Spritz.DrawSprite(m_Sprites[3], -16, -16);

        Spritz.currentLayerId = 2;
        Spritz.DrawSprite(m_SpritesKenney[12], -32, -32);
        Spritz.DrawSprite(m_SpritesKenney[3], 0, 16);

        Spritz.currentLayerId = 3;
        Spritz.DrawSprite(m_SpritesKenney[8], 0, 32);
    }
}
