using UniMini;
using UnityEngine;

public class HelloWorld : SpritzGame
{
    SpriteId m_Heart;
    AudioClipId m_Music;
    string[] text = { "H", "e", "l", "l", "o", " ", "W", "o", "r", "l", "d", "!" };
    public override void InitializeSpritz()
    {
        Spritz.CreateLayer("Spritesheets/hello_spritesheet");
        Spritz.LoadSoundbankFolder("Audio");
        m_Music = new AudioClipId("hello");
        m_Heart = new SpriteId("heart");

        Spritz.PlayMusic(m_Music, 0.5f, true);
    }

    public override void UpdateSpritz()
    {
        
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);
        var pal = Spritz.palette;

        // SpritzUtil.DrawTimeInfo();

        for (var col = 14; col >= 7; --col)
        {
            for (var i = 0; i < text.Length; ++i)
            {
                var t1 = Time.realtimeSinceStartup * 30 + i*4 - col*2;
                var x = 8 + (i + 1) * 8 + SpritzUtil.Cosp8(t1/90) * 3;
                var y = 38 + (col - 7) + SpritzUtil.Cosp8(t1/50) * 5;
                Spritz.Print(text[i], (int)x, (int)y, pal[col]);
            }
        }

        Spritz.Print("This is Spritz", 34, 70, pal[14]);
        Spritz.Print("Keep it fresh", 34, 80, pal[12]);
        Spritz.DrawSprite(m_Heart, 60, 90);
    }
}
