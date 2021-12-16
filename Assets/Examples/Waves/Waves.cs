using UniMini;
using UnityEngine;

public class Waves : SpritzGame
{
    public override void InitializeSpritz()
    {
        
    }

    public override void UpdateSpritz()
    {
        
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);
        var r = 64;
        for (var y = -r; y < r; y += 3)
        {
            for (var x = -r; x < r; x += 2)
            {
                var dist = Mathf.Sqrt(x*x + y*y);
                var z = (int)(SpritzUtil.Cosp8(dist / 40 - Time.time) * 6);
                Spritz.DrawPixel(r + x, r + y - z, Spritz.palette[6]);
            }
        }
    }
}
