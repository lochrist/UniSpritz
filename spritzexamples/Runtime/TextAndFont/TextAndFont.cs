using System.Linq;
using UniMini;
using UnityEngine;

public class TextAndFont : SpritzGame
{
    SpriteFont m_Font;
    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Fonts/Weiholmir_GameMaker_sheet");
        var sprites = Spritz.GetSpriteDescs(0).Select(desc => desc.id).ToArray();
        m_Font = new SpriteFont(7, 7);
        m_Font.Add('!', (char)127, sprites, 1);
    }

    public override void UpdateSpritz()
    {
    }

    public override void DrawSpritz()
    {
        // Draw stuff:
        Spritz.DrawRectangle(10, 10, 10, 10, Color.blue, false);
        Spritz.Print(m_Font, "Hello world!", 0, 0, Color.red);
    }
}
