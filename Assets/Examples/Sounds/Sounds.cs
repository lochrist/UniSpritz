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
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        Spritz.LoadSoundbankFolder("Audio");
        m_Music = new AudioClipId("Music - Disco");
        m_Sfx = new AudioClipId("Shoot - Laser");
    }

    public override void UpdateSpritz()
    {
        if (Spritz.GetMouseDown(0))
        {
            Spritz.PlaySound(m_Sfx, 5, false);
        }
        else if (Spritz.GetMouseDown(1))
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
        
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            Spritz.camera.x += 1;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Spritz.camera.x -= 1;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            Spritz.camera.y += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            Spritz.camera.y -= 1;
        }
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);
        Spritz.Print($"Cam: {Spritz.camera}", Spritz.camera.x, Spritz.camera.y, Color.red);
        Spritz.DrawCircle(64, 64, 5, Color.blue, true);
    }
}
