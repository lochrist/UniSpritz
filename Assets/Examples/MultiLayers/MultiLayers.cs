using UniMini;
using UnityEngine;

public class MultiLayers : SpritzGame
{
    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");

        Spritz.CreateLayer("Spritesheets/1bit-kennynl");
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
    }

    public override void DrawSpritz()
    {
        DrawForeground();
        DrawBackground();
    }

    public void DrawBackground()
    {
        Spritz.currentLayerId = 0;
        Spritz.Clear(Color.black);
        Spritz.Print("this is my background of doom", 0, 64, Color.blue);
    }

    public void DrawForeground()
    {
        Spritz.currentLayerId = 1;
        Spritz.Clear(Color.clear);
        // Spritz.Print("over the top", 0, 64, Color.red);
    }
}
