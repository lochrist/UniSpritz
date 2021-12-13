using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

public class TextureLayerGame : SpritzGame
{
    SpriteId[] m_Sprites;
    Vector3[] stars;
    public int count = 1000;
    public float minZ = 1f;
    public float maxZ = 50f;
    public float xSize = 512f;
    public float ySize = 512f;
    public float speed = 50f;


    public string longText = @"Mista Busta, where the fuck you at?
Can't scrap a lick, so I know you got your gat
Your dick on hard, from fuckin' your road dogs
The hood you threw up with, niggas you grew up with
Don't even respect yo' ass
That's why it's time for the doctor to check your ass, nigga
Used to be my homie, used to be my ace
Now I wanna slap the taste out yo' mouth
Make you bow down to the Row
Fuckin' me, now I'm fuckin' you, little ho
Oh, don't think I forgot, let you slide
Let me ride, just another homicide
Yeah it's me so I'ma talk on
Stompin' on the easiest streets that you can walk on
So strap on your Compton hat, your locs
And watch your back 'cause you might get smoked, loc
And pass the bud and stay low-key
B-G 'cause you lost all your homies' love
Now call it what you want to
You fucked wit' me, now it's a must that I fuck wit' you";

    public override void InitializeSpritz()
    {

        InitSprites();
        // InitStarField();
    }

    public override void UpdateSpritz()
    {
    }

    public override void DrawSpritz()
    {
        // DrawColors();
        // DrawStarField();
        // DrawSprites();
        DrawText();
    }

    private void DrawColors()
    {
        var colors = new[] { Color.blue, Color.white, Color.green, Color.red, Color.cyan, Color.yellow, Color.gray, new Color(0.9f, 0.45f, 0.3f) };
        for (var i = 0; i < 8; ++i)
            Spritz.DrawPixel(i, i, colors[i]);
    }

    private void InitSprites()
    {
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        // Spritz.CreateLayer("Spritesheets/SimpleSprites");
        m_Sprites = Spritz.GetSprites(0);
    }

    private void InitStarField()
    {
        stars = new Vector3[count];
        for (int i = 0; i < stars.Length; i++)
            stars[i] = new Vector3(Random.Range(-xSize, xSize), Random.Range(-ySize, ySize), (float)(stars.Length - i) * maxZ / stars.Length + minZ);
    }

    private void DrawSprites()
    {
        Spritz.DrawSprite(m_Sprites[1], 0, 0);

        Spritz.DrawSprite(m_Sprites[2], 32, 32);

        Spritz.DrawSprite(m_Sprites[3], 64, 64);
    }

    private void DrawText()
    {
        Spritz.Print(longText, 10, 10, Color.blue);
    }

    private void DrawStarField()
    {
        Spritz.Clear(Color.black);
        for (int i = 0; i < stars.Length; i++)
        {
            int x = (int)(stars[i].x / stars[i].z) + resolution.x / 2;
            int y = (int)(stars[i].y / stars[i].z) + resolution.y / 2;
            byte b = (byte)(255f / stars[i].z);
            var c = b / 255f;

            if (x < resolution.x)
                stars[i] += new Vector3(Time.deltaTime * speed, 0f, 0f);
            else
                stars[i] = new Vector3(-xSize, stars[i].y, stars[i].z);

            if (x >= 0 && x < resolution.x && y >= 0 && y < resolution.y)
                Spritz.DrawPixel(x, y, new Color(c, c, c));
        }
    }
}
