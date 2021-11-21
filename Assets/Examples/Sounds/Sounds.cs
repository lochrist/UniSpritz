using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class Sounds : SpritzGame
{
    AudioClipId m_Sfx;
    AudioClipId m_Music;
    bool m_IsMusicPlaying;

    public override void InitializeSpritz()
    {
        Spritz.CreateLayer("RandomImages/tiny_dungeon_monsters");
        Spritz.LoadSoundbankFolder("Audio");
        m_Music = new AudioClipId("Music - Disco");
        m_Sfx = new AudioClipId("Shoot - Laser");
    }

    public override void UpdateSpritz()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Spritz.PlaySound(m_Sfx, 5, false);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (m_IsMusicPlaying)
            {
                Spritz.StopMusic();
                m_IsMusicPlaying = false;
            }
            else
            {
                Spritz.PlayMusic(m_Music, 5, true);
                m_IsMusicPlaying = true;
            }
        }
    }

    public override void DrawSpritz()
    {
        Spritz.currentLayerId = 0;
        Spritz.DrawPixel(10, 10, Color.blue);
    }
}
