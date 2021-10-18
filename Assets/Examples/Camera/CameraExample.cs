using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class CameraExample : SpritzGame
{
    public override void InitializeSpritz()
    {
        Spritz.CreateLayer("RandomImages/tiny_dungeon_monsters");
    }

    public override void UpdateSpritz()
    {
    }

    public override void DrawSpritz()
    {
        Spritz.currentLayerId = 0;

        // Draw a Pixel in each corner:

        Spritz.DrawPixel(0, 0, Color.yellow);


        Spritz.DrawSprite(Spritz.GetSprites(0)[3], 10, 30);
        Spritz.DrawSprite(Spritz.GetSprites(0)[2], 30, 30);

        Spritz.DrawSprite(Spritz.GetSprites(0)[4], 10, 60);
        Spritz.DrawSprite(Spritz.GetSprites(0)[1], 30, 60);

        // Spritz.DrawPixel(0, 10, Color.yellow);


        // Spritz.DrawPixel(50, 50, Color.green);

        // Spritz.DrawPixel(resolution.x / 2, resolution.y / 2, Color.red);

        /*
        Spritz.DrawPixel(10, resolution.y - 10, Color.blue);

        Spritz.DrawPixel(resolution.x - 10, 10, Color.blue);
        Spritz.DrawPixel(resolution.x - 10, resolution.y - 10, Color.blue);

        Spritz.DrawPixel(resolution.x/2, resolution.y/2, Color.blue);
        */
    }
}
