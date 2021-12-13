using UniMini;
using UnityEngine;

public class HelloWorld : SpritzGame
{
    SpriteId m_Heart;
    AudioClipId m_Music;
    string[] text = { "H", "e", "l", "l", "o", " ", "W", "o", "r", "l", "d", "!" };
    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("RandomImages/hello_spritesheet");
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
        // TODO clearly a problem with cos (same as arkanoid)

        Spritz.Clear(Color.black);

        for (var col = 14; col > 7; --col)
        {
            for (var i = 0; i < text.Length; ++i)
            {
                var t1 = Time.realtimeSinceStartup * 30 + i*4 - col*2;
                var x = 8 + i * 8 + SpritzUtil.Cosp8(t1/90) * 3;
                var y = 38 + (col - 7) + SpritzUtil.Cosp8(t1/50) * 5;
                Spritz.Print(text[i], (int)x, (int)y, Spritz.palette[col]);
            }
        }

        Spritz.Print("This is pico-8", 37, 70, Spritz.palette[14]);
        Spritz.Print("Nice to meet you", 34, 80, Spritz.palette[12]);
        Spritz.DrawSprite(m_Heart, 60, 90);
    }
}
