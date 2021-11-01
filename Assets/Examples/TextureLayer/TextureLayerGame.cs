using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class TextureLayerGame : SpritzGame
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
        Spritz.DrawPixel(0, 0, Color.blue);

        Spritz.DrawPixel(160, 90, Color.blue);

        Spritz.DrawPixel(319, 179, Color.blue);
    }
}
