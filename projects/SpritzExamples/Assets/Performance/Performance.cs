using UniMini;

using UnityEngine;

public class Performance : SpritzGame
{
    public enum DrawType
    {
        Square,
        SquareFill,
        Circle,
        CircleFill,
        NoDraw,
        Pixels,
        PixelsAllSame,
        Sprites,
    }
 
    public DrawType drawType;
    public int nbFramesPerDrawType = 1;
    public int nbFrames;

    private SpriteId[] m_Sprites;

    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        m_Sprites = Spritz.GetSprites();
        debugPerformanceTitle = FormatTitle(drawType.ToString());
        nbFramesPerDrawType = 1;
        drawType = DrawType.Square;

        // debugPerformanceTitle = null;
        // drawType = DrawType.Circle;
    }

    public override void UpdateSpritz()
    {
    }

    public override void DrawSpritz()
    {
        switch(drawType)
        {
            case DrawType.Square:
                DrawSquare();
                break;
            case DrawType.SquareFill:
                DrawSquareFill();
                break;
            case DrawType.Circle:
                DrawCircle();
                break;
            case DrawType.CircleFill:
                DrawCircleFill();
                break;
            case DrawType.Pixels:
                DrawPixels();
                break;
            case DrawType.PixelsAllSame:
                DrawPixelsAllSame();
                break;
            case DrawType.Sprites:
                DrawSprites();
                break;
            case DrawType.NoDraw:
                debugPerformanceTitle = null;
                break;
        }
    }

    public override void EndFrame()
    {
        if (drawType != DrawType.NoDraw && ++nbFrames == nbFramesPerDrawType)
        {
            nbFrames = 0;
            if (drawType != DrawType.NoDraw)
            {
                drawType = (DrawType)((int)drawType) + 1;
                if (drawType != DrawType.NoDraw)
                    debugPerformanceTitle = FormatTitle(drawType.ToString());
                else
                    debugPerformanceTitle = null;
            }
        }
    }

    string FormatTitle(string title)
    {
        return $"{title}-{resolution}";
    }

    public void DrawSquare()
    {
        Spritz.DrawRectangle(0, 0, resolution.x, resolution.y, Color.blue, false);
    }

    public void DrawSquareFill()
    {
        Spritz.DrawRectangle(0, 0, resolution.x, resolution.y, Color.red, true);
    }

    public void DrawCircle()
    {
        Spritz.DrawCircle(resolution.x / 2, resolution.y / 2, resolution.x / 2, Color.green, false);
    }

    public void DrawCircleFill()
    {
        Spritz.DrawCircle(resolution.x / 2, resolution.y / 2, resolution.x / 2, Color.cyan, true);
    }

    public void DrawPixels()
    {
        debugPerformanceTitle = null;
    }

    public void DrawPixelsAllSame()
    {
        debugPerformanceTitle = null;
    }

    public void DrawSprites()
    {
        debugPerformanceTitle = null;
    }
}
