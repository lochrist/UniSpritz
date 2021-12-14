using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class PerformanceExamples : SpritzGame
{
    public override void InitializeSpritz()
    {
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
    }

    public override void UpdateSpritz()
    {
    }

    public override void DrawSpritz()
    {
        Spritz.currentLayerId = 0;

        var i = 0;
        var colors = new[] { Color.white, Color.green, Color.red, Color.blue, Color.cyan, Color.grey };
        for(var x = -160; x < 160; ++x)
        {
            for (var y = -120; y < 120; ++y)
            {
                Spritz.DrawPixel(x, y, colors[i++ % colors.Length]);
            }
        }
    }
}
