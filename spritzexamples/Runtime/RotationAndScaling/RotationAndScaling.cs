using UniMini;
using UnityEngine;

public class RotationAndScaling : SpritzGame
{
    SpriteId m_Sprite;
    float m_Angle;
    bool m_FlipX;
    bool m_FlipY;
    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        m_Sprite = Spritz.GetSpriteDescs()[0].id;
        m_Angle = 0;
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
        if (Spritz.GetMouseDown(0))
        {
            m_Angle += 10;
            if (m_Angle >= 360)
                m_Angle = 0;
        }
        else if (Spritz.GetMouseDown(1))
        {
            m_Angle -= 10;
            if (m_Angle < 0)
                m_Angle = 360;
        }
        else if (Spritz.GetKeyDown(KeyCode.Z))
        {
            m_FlipX = !m_FlipX;
        }
        else if (Spritz.GetKeyDown(KeyCode.X))
        {
            m_FlipY = !m_FlipY;
        }
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);

        Spritz.Print($"Angle: {m_Angle}", 0, 0, Color.red);

        Spritz.DrawSprite(m_Sprite, 64, 0);
        Spritz.DrawSprite(m_Sprite, 64, 64, m_FlipX, m_FlipY, m_Angle);
    }
}
