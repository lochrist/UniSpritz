using UniMini;
using UnityEngine;

public class Layout : SpritzGame
{
    int colorIndex;
    public override void InitializeSpritz()
    {

    }

    public override void UpdateSpritz()
    {

    }

    public override void DrawSpritz()
    {
        colorIndex = 0;
        Spritz.Clear(Color.gray);

        var l = new SpritzCellLayout();
        /*
        DrawRect(l.Row());
        DrawRect(l.Col());
        DrawRect(l.Row());
        */
        /*
        DrawRect(l.Col());
        DrawRect(l.Row());
        DrawRect(l.Col());
        DrawRect(l.Col());
        DrawRect(l.Row());
        */

        DrawRect(l.Row());
        DrawRect(l.Col());
        DrawRect(l.Row(100, 40));
        DrawRect(l.Col(30, 20));
        DrawRect(l.Row());
        DrawRect(l.Row(30, 60));
        DrawRect(l.Col(300, 60));
        DrawRect(l.Row());
        l.Reset(0, l.currentCellPos.y);
        DrawRect(l.Row());
        DrawRect(l.Row());
    }

    private void DrawRect(RectInt r)
    {
        Spritz.DrawRectangle(r.x, r.y, r.width, r.height, Spritz.palette[colorIndex], false);
        colorIndex = (colorIndex + 1) % Spritz.palette.Length;
    }
}
