using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class Sounds : SpritzGame
{
    AudioClipId m_Sfx;
    AudioClipId m_Music;
    bool m_IsMusicPlaying;
    int m_Time;


    public override void InitializeSpritz()
    {
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        Spritz.LoadSoundbankFolder("Audio");
        m_Music = new AudioClipId("Music - Disco");
        m_Sfx = new AudioClipId("Shoot - Laser");
        m_Time = -1;
    }

    public override void UpdateSpritz()
    {
        // TODO: GetMouseButtonDown does'nt work well since we throttle the frame.
        if (Input.GetMouseButton(0))
        {
            Spritz.PlaySound(m_Sfx, 5, false);
            m_Time = 0;
        }
        else if (Input.GetMouseButton(1))
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
        if (m_Time >= 0)
        {
            m_Time++;
            if (m_Time > 14)
                m_Time = -1;
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
        if (m_Time >= 1)
        {
            Spritz.DrawCircle(10, 10, m_Time / 2, Color.blue, false);
        }

        Spritz.Print($"{m_Time}", 0, 0, Color.red);


        // Spritz.Rectangle(1, 1, 3, 3, Color.red, false);
        // Spritz.Rectangle(1, 1, 5, 3, Color.red, true);
        // Spritz.Rectangle(10, 75, 20, 90, Color.red, true);
        //Spritz.DrawPixel(Spritz.camera.x, Spritz.camera.y, Color.blue);
    }
}
