using UniMini;

public class RotationAndScaling : SpritzGame
{
    SpriteId m_Sprite;
    float m_Angle;
    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        m_Sprite = Spritz.GetSprites()[0];
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
    }

    public override void DrawSpritz()
    {
        Spritz.DrawSprite(m_Sprite, 64, 0);
        Spritz.DrawSprite(m_Sprite, 64, 64, m_Angle);
    }
}
