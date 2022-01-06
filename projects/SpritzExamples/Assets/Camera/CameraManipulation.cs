using UniMini;
using UnityEngine;

public class CameraManipulation : SpritzGame
{
    float m_Offset;
    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        // Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
    }

    public override void UpdateSpritz()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Spritz.camera.x = 0;
            Spritz.camera.y = 0;
        }
        if (Input.GetKey(KeyCode.Return))
        {
            m_Offset = 1.5f;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            Spritz.camera.x += 1;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Spritz.camera.x -= 1;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            Spritz.camera.y += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            Spritz.camera.y -= 1;
        }
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);

        if (m_Offset > 0)
            ShakeScreen(ref m_Offset);

        Spritz.Print($"Cam: {Spritz.camera}", Spritz.camera.x, Spritz.camera.y, Color.red);

        Spritz.Print("Use Arrow to move camera", 10, 10, Color.red);
        Spritz.Print("Press Enter to shake", 10, 20, Color.red);
        Spritz.Print("Press Esc to center camera", 10, 30, Color.red);
        Spritz.DrawCircle(10, 40, 5, Color.blue, true);
        Spritz.DrawCircle(10, 55, 5, Color.blue, false);
        Spritz.DrawRectangle(10, 70, 20, 10, Color.green, true);
        Spritz.DrawRectangle(10, 90, 20, 10, Color.green, false);

        Spritz.DrawLine(10, 110, 80, 110, Color.green);
    }

    public static void ShakeScreen(ref float offset)
    {
        var fade = 0.9f;
        var offsetX = 16f - Random.Range(0, 32f);
        var offsetY = 16f - Random.Range(0, 32f);

        offsetX *= offset;
        offsetY *= offset;

        Spritz.camera = new Vector2Int((int)offsetX, (int)offsetY);
        offset *= fade;
        if (offset < 0.05f)
            offset = 0f;
    }
}
