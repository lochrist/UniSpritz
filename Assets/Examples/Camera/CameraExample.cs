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

        Spritz.DrawPixel(0, 0, Color.blue);

        Spritz.DrawPixel(0, 10, Color.yellow);


        Spritz.DrawPixel(50, 50, Color.green);

        Spritz.DrawPixel(resolution.x / 2, resolution.y / 2, Color.red);

        /*
        Spritz.DrawPixel(10, resolution.y - 10, Color.blue);

        Spritz.DrawPixel(resolution.x - 10, 10, Color.blue);
        Spritz.DrawPixel(resolution.x - 10, resolution.y - 10, Color.blue);

        Spritz.DrawPixel(resolution.x/2, resolution.y/2, Color.blue);
        */
    }
}
