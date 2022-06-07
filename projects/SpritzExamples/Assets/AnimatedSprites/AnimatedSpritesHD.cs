using UniMini;
using UnityEngine;

public class AnimatedSpritesHD : SpritzGame
{
    AnimSprite[] m_Sprites;

    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/uf_heroes");

        // Same color as clear color to help when setting transparency of sprites.
        gameObject.GetComponent<Camera>().backgroundColor = Color.grey;
        m_Sprites = ExampleUtils.GetUfHeroes(Spritz.GetSprites());
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
        for (var i = 0; i < m_Sprites.Length; ++i)
        {
            m_Sprites[i].Update();
        }
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.grey);
        int col = 0;
        int row = 0;
        int cellSize = 48;
        int offsetYH = 48;
        for (var i = 0; i < m_Sprites.Length; ++i)
        {
            m_Sprites[i].Draw((col * cellSize), offsetYH + (row * cellSize));
            col++;
            if (col >= 12)
            {
                col = 0;
                row++;
            }
        }

        SpritzUtil.DrawTimeInfo();
    }
}
