using UniMini;
using UnityEngine;

public class Effects : SpritzGame
{
    SpriteId m_Sprite;
    Effect m_Dissolve;
    int spriteSize = 16;
    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        m_Sprite = Spritz.GetSprites()[0];

        // Effect layer
        Spritz.CreateLayer();
        // m_Dissolve = EffectsFactory.CreateDissolve(new RectInt(32, 32, spriteSize, spriteSize), Color.black, 1f);
        m_Dissolve = EffectsFactory.CreateLeftRight(new RectInt(32, 32, spriteSize, spriteSize), Color.black, 1f);
        m_Dissolve.ticker.isRunning = false;
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
        m_Dissolve.Update();

        if (Spritz.GetMouseDown(0))
        {
            m_Dissolve.ticker.isRunning = !m_Dissolve.ticker.isRunning;
            m_Dissolve.Reset();
        }
    }

    public override void DrawSpritz()
    {
        Spritz.currentLayerId = 0;
        Spritz.Clear(Color.black);
        // Draw stuff:
        Spritz.Print("Click to dissolve", 5, 5, Color.red);

        Spritz.DrawSprite(m_Sprite, 32, 32);

        Spritz.currentLayerId = 1;
        Spritz.Clear(Color.clear);
        m_Dissolve.Draw();
    }
}
