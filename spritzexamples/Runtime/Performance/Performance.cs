using System.Linq;
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
        Clear,
        Sprites,
    }
 
    public DrawType drawType;
    public int nbFramesPerDrawType = 1;
    public int nbFrames;
    public bool renderingReport;

    private SpriteId[] m_Sprites;

    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        debugPerformanceTitle = FormatTitle(drawType.ToString());
        nbFramesPerDrawType = 1;
        drawType = DrawType.Square;
        m_Sprites = Spritz.GetSpriteDescs().Select(desc => desc.id).ToArray();
    }

    public override void UpdateSpritz()
    {
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);

        switch (drawType)
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
            case DrawType.Clear:
                DrawClear();
                break;
            case DrawType.NoDraw:
                debugPerformanceTitle = null;
                break;
        }

        SpritzUtil.DrawTimeInfo(5, 5);
    }

    public override void EndFrame()
    {
        if (renderingReport && drawType != DrawType.NoDraw && ++nbFrames == nbFramesPerDrawType)
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
        var colorIndex = 0;
        for (var x = 0; x < resolution.x; ++x)
        {
            for (var y = 0; y < resolution.y; ++y)
            {
                Spritz.DrawPixel(x, y, Spritz.palette[colorIndex++ % Spritz.palette.Length]);
            }
        }
    }

    public void DrawPixelsAllSame()
    {
        for (var x = 0; x < resolution.x; ++x)
        {
            for (var y = 0; y < resolution.y; ++y)
            {
                Spritz.DrawPixel(x, y, Color.green);
            }
        }
    }

    public void DrawSprites()
    {
        var sprite = m_Sprites[0];
        var spriteW = 16;
        var spriteH = 16;
        var nbFramesWidth = resolution.x / spriteW;
        var nbFramesHeight = resolution.y / spriteH;
        var spriteIndex = 0;

        for (var x = 0; x < nbFramesWidth; ++x)
        {
            for (var y = 0; y < nbFramesHeight; ++y)
            {
                Spritz.DrawSprite(m_Sprites[spriteIndex++], x * spriteW, y * spriteH);
            }
        }
    }

    public void DrawClear()
    {
        Spritz.Clear(Color.green);
    }
}
