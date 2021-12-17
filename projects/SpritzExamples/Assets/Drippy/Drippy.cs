using UniMini;
using UnityEngine;

public class Drippy : SpritzGame
{
    int x = 64;
    int y = 64;
    int c = 8;
    public override void InitializeSpritz()
    {
        x = 64;
        y = 64;
        c = 8;
    }

    public override void UpdateSpritz()
    {
        if (Input.GetKey(KeyCode.W))
            y -= 1;
        if (Input.GetKey(KeyCode.A))
            x -= 1;
        if (Input.GetKey(KeyCode.S))
            y += 1;
        if (Input.GetKey(KeyCode.D))
            x += 1;

        c = c + (Spritz.frame % 8 == 0 ? 1 : 0);
        if (c >= Spritz.palette.Length) c = 8;
        for (var i = 0; i < 800; ++i)
        {
            var x2 = Random.Range(0, 127);
            var y2 = Random.Range(0, 127);
            var c = Spritz.GetPixel(x2, y2);
            if (c != Color.black)
            {
                Spritz.DrawPixel(x2, y2 + 1, c);
            }
        }
    }

    public override void DrawSpritz()
    {
        Spritz.DrawPixel(x, y, Spritz.palette[c]);
    }
}
