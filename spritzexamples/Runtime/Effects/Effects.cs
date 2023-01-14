using System.Linq;
using UniMini;
using UnityEngine;

public class Effects : SpritzGame
{
    SpriteId m_Sprite1;
    SpriteId m_Sprite2;
    Effect m_Dissolve;
    int spriteSize = 16;
    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        var sprites = Spritz.GetSpriteDescs().Select(desc => desc.id).ToArray();
        m_Sprite1 = sprites[0];
        m_Sprite2 = sprites[7];

        // Effect layer
        Spritz.CreateLayer();
        m_Dissolve = EffectsFactory.CreateDissolve(spriteSize, spriteSize, Color.black, 1f);
        m_Dissolve.playing = false;
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
        m_Dissolve.Update();

        if (Spritz.GetMouseDown(0))
        {
            m_Dissolve.playing = !m_Dissolve.playing;
            m_Dissolve.Reset();
        }
    }

    public override void DrawSpritz()
    {
        Spritz.currentLayerId = 0;
        Spritz.Clear(Color.black);
        // Draw stuff:
        Spritz.Print("Click to dissolve", 5, 5, Color.red);

        Spritz.DrawSprite(m_Sprite1, 32, 32);
        Spritz.DrawSprite(m_Sprite2, 32, 64);

        Spritz.currentLayerId = 1;
        Spritz.Clear(Color.clear);
        m_Dissolve.Draw(32, 32);

        m_Dissolve.Draw(32, 64);
    }
}
