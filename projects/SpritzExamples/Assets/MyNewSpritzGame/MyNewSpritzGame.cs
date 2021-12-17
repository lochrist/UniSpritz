using UniMini;

public class MyNewSpritzGame : SpritzGame
{
    int color;
    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        // Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
    }

    public override void DrawSpritz()
    {
        // Draw stuff:
        var pal = Spritz.palette;
        Spritz.Print("This is Spritz", 34, 50, pal[14]);
        Spritz.Print("Keep it fresh", 36, 60, pal[12]);

        color++;
        if (color >= pal.Length)
            color = 0;
        Spritz.DrawCircle(40, 75, 6, pal[color], true);
        Spritz.DrawRectangle(50, 75, 12, 12, pal[color], true);
        Spritz.DrawRectangle(65, 75, 12, 12, pal[color], false);
        Spritz.DrawCircle(85, 75, 6, pal[color], false);
        Spritz.DrawLine(34, 100, 95, 100, pal[color]);
    }
}
