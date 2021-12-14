using UniMini;
using UnityEngine;

public class TextAndFont : SpritzGame
{
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
        Spritz.DrawRectangle(10, 10, 10, 10, Color.blue, false);
        Spritz.Print("Hello world!", 0, 0, Color.red);
    }
}
