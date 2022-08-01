using UniMini;

public class DrawPrimitives : SpritzGame
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
        Spritz.DrawLine(5, 5, 32, 5, pal[3], 4);

        Spritz.DrawLine(0, 20, 16, 64, pal[3], 4);
    }
}
