using UniMini;
using UnityEngine;

public class AnimatedSprites : SpritzGame
{
    AnimSprite[] m_Sprites;

    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");

        // Same color as clear color to help when setting transparency of sprites.
        gameObject.GetComponent<Camera>().backgroundColor = Color.grey;

        var nbAnimSprites = (128 / 16) * (128 / 16);
        m_Sprites = new AnimSprite[nbAnimSprites];
        var allSprites = Spritz.GetSprites();
        int spriteIndex = 0;
        int m_AnimSpriteIndex = 0;
        while (m_AnimSpriteIndex < nbAnimSprites)
        {
            // Process sprites by batch of 16/32 (2 lines on the sheet).
            // second animated sprite is at +16
            var spriteEnd = Mathf.Min(m_AnimSpriteIndex + 16, nbAnimSprites);
            for (; m_AnimSpriteIndex < spriteEnd; ++m_AnimSpriteIndex, ++spriteIndex)
            {
                m_Sprites[m_AnimSpriteIndex] = new AnimSprite(4, new[] { allSprites[spriteIndex], allSprites[spriteIndex + 16] })
                {
                    loop = true
                };
            }
            spriteIndex += 16;
        }
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
