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

        var i = 0;
        var colors = new[] { Color.white, Color.green, Color.red, Color.blue, Color.cyan, Color.grey };
        // 320, 240
        var width = 320;
        var height = 240;

        // Spritz.DrawPixel(, 0, Color.blue);

        // Top line
        for (var x = 0; x < width; ++x)
        {
            Spritz.DrawPixel(x, 0, Color.blue);
        }

            // Bottom Line

            // Left line

            // Right line

    }
}
