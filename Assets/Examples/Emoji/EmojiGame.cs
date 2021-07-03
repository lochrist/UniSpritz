using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class EmojiGame : SpritzGame
{
    SpriteId m_S1;
    SpriteId m_S2;
    public void Initialize()
    {
        Spritz.CreateLayer("emoji/emojione-2");
        m_S1 = new SpriteId("emojione-2_0");
        m_S2 = new SpriteId("emojione-2_8");
    }

    public void Update()
    {

    }

    public void Render()
    {
        // Spritz.DrawSprite(m_S1, UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-4f, 4f));
        // Spritz.DrawSprite(m_S2, UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-4f, 4f));

        Spritz.DrawSprite(m_S1, 0, 0);
        // Spritz.DrawSprite(m_S2, 2, 2);
    }
}
