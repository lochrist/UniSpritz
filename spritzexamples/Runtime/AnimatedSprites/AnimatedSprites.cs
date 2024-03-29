using UniMini;
using UnityEngine;
using System.Linq;

public class AnimatedSprites : SpritzGame
{
    AnimSprite[] m_Sprites;

    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");

        // Same color as clear color to help when setting transparency of sprites.
        gameObject.GetComponent<Camera>().backgroundColor = Color.grey;
        var sprites = Spritz.GetSpriteDescs().Select(desc => desc.id).ToArray();
        m_Sprites = ExampleUtils.GetTinyMonsters(sprites);
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
        for (var i= 0; i < m_Sprites.Length; ++i)
        {
            m_Sprites[i].Update();
        }
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.grey);
        int col = 0;
        int row = 0;
        int cellSize = 16;
        for (var i = 0; i < m_Sprites.Length; ++i)
        {
            m_Sprites[i].Draw(col * cellSize, row * cellSize);
            col++;
            if (col >= 8)
            {
                col = 0;
                row++;
            }
        }
    }
}
