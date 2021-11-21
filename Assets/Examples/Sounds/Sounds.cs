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
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            Spritz.camera.x += 1;
            Debug.Log(Spritz.camera);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            Spritz.camera.x -= 1;
            Debug.Log(Spritz.camera);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            Spritz.camera.y -= 1;
            Debug.Log(Spritz.camera);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            Spritz.camera.y += 1;
            Debug.Log(Spritz.camera);
        }

    }

    public override void DrawSpritz()
    {
        Spritz.currentLayerId = 0;
        /*
        for (var i = 0; i < 32; ++i)
            Spritz.DrawPixel(i, i, Color.red);
        */
        Spritz.Clear(Color.black);
        Spritz.DrawPixel(10, 10, Color.red);
        //Spritz.DrawPixel(Spritz.camera.x, Spritz.camera.y, Color.blue);
    }
}
