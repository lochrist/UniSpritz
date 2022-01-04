using UniMini;
using UnityEngine;

public class StarTravel : SpritzGame
{
    public struct Star
    {
        public int x;
        public int y;
        public int color;
        public float depth;
        public int color2;
    }

    public struct Ship
    {
        public int x;
        public int y;
        public float angle;
        public float force;
        public float dx;
        public float dy;
        public float da;
        public float df;
        public float af;
    }

    public struct Screen
    {
        public int width;
        public int height;
        public int x;
        public int y;
    }

    Star[] stars;
    int starCount = 79;
    Screen screen;
    Ship ship;
    public override void InitializeSpritz()
    {
        screen = new Screen()
        {
            width = 128,
            height = 128,
            x = 127,
            y = 127
        };

        ship = new Ship()
        {
            x = 64,
            y = 64,
            angle = 0,
            force = 0,
            dx = 0,
            dy = 0,
            da = 0.01f,
            df = 0,
            af = 0.02f
        };

        // Create Layer and initialize various states
        var depths = new float[] { 1,0.7f, 0.5f, 0.3f };
        var colors = new int[] { 7, 13, 5, 1};
        var fades = new int[] { 13, 1, 0, 0 };
        stars = new Star[starCount];
        for (var x = 0; x < starCount; ++x)
        {
            var i = (int)(x/20f);
            var star = new Star()
            {
                x = Random.Range(0, screen.x),
                y = Random.Range(0, screen.y),
                color = colors[i],
                depth = depths[i],
                color2 = fades[i]
            };
            stars[x] = star;
        }
    }

    public override void UpdateSpritz()
    {
        if (Input.GetKey(KeyCode.A))
        {
            ship.angle -= ship.da;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            ship.angle += ship.da;
        }
        ship.angle = ship.angle % 1;

        if (Input.GetKey(KeyCode.W))
        {
            ship.df += ship.af;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            ship.df -= ship.af;
        }
        else
        {
            ship.df = 0;
            ship.force *= 0.95f;
        }
        ship.force += ship.df;

        if (Mathf.Abs(ship.force)<0.04f)
        {
            ship.force = 0;
        }
        ship.force = Mathf.Clamp(ship.force, -6, 6);

        ship.dx = SpritzUtil.Cosp8(ship.angle) * ship.force;
        ship.dy = -SpritzUtil.Sinp8(ship.angle) * ship.force;

        for(var i = 0; i < stars.Length;++i)
        {
            stars[i].x = (int)(stars[i].x - ship.dx * stars[i].depth);
            if (stars[i].x<0)
            {
                stars[i].x = screen.x + stars[i].x;
            }
            if (stars[i].x > screen.x)
            {
                stars[i].x = stars[i].x - screen.x;
            }
            stars[i].y = (int)(stars[i].y - ship.dy * stars[i].depth);
            if (stars[i].y < 0)
            {
                stars[i].y = screen.y + stars[i].y;
            }
            if (stars[i].y > screen.y)
            {
                stars[i].y = stars[i].y - screen.y;
            }
        }
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);

        // if we're going fast draw a trail
        if (Mathf.Abs(ship.force) > 3)
        {
            foreach (var star in stars)
            {
                if (star.depth > 0.5f)
                {
                    Spritz.DrawLine(star.x, star.y,
                        (int)(star.x + ship.dx * Mathf.Abs(ship.force) / 3 * star.depth),
                        (int)(star.y + ship.dy * Mathf.Abs(ship.force) / 3 * star.depth),
                        Spritz.palette[star.color2]);
                }
            }
        }

        // Draw stars
        foreach (var star in stars)
        {
            Spritz.DrawPixel(star.x, star.y, Spritz.palette[star.color]);
        }

        // Draw ship tail:
        Spritz.DrawLine(ship.x, ship.y, (int)(ship.x - ship.dx), (int)(ship.y - ship.dy), Spritz.palette[9]);
        Spritz.DrawPixel(ship.x, ship.y, Spritz.palette[8]);

        // Draw ship ugly shape:
        var len = 5;
        var ang = 0.37f;
        var col = 2;
        var tx = (int)(ship.x + SpritzUtil.Cosp8(ship.angle) * len);
        var ty = (int)(ship.y - SpritzUtil.Sinp8(ship.angle) * len);
        var lx = (int)(ship.x + SpritzUtil.Cosp8(ship.angle - ang) * len);
        var ly = (int)(ship.y - SpritzUtil.Sinp8(ship.angle - ang) * len);
        var rx = (int)(ship.x + SpritzUtil.Cosp8(ship.angle + ang) * len);
        var ry = (int)(ship.y - SpritzUtil.Sinp8(ship.angle + ang) * len);
        Spritz.DrawLine(tx, ty, lx, ly, Spritz.palette[col]);
        Spritz.DrawLine(tx, ty, rx, ry, Spritz.palette[col]);
        Spritz.DrawLine(rx, ry, lx, ly, Spritz.palette[col]);
    }
}
