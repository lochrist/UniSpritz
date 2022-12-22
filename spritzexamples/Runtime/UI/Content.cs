using UniMini;
using UnityEngine;
public class Content : SpritzGame
{
    SpriteId m_Sprite;
    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        m_Sprite = Spritz.GetSprites()[0];
    }

    public override void UpdateSpritz()
    {
        
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);
        
        var lyt = new SpritzCellLayout()
        {
            padding = new Vector2Int(5, 5)
        };

        var cellSize = new Vector2Int(85, 50);
        var nbRow = 9; 
        var nbCol = 6;

        var alignment = new[] {
            UIAlignment.TopLeft, UIAlignment.TopCenter, UIAlignment.TopRight,
            UIAlignment.MidLeft, UIAlignment.Center, UIAlignment.MidRight,
            UIAlignment.BottomLeft, UIAlignment.BottomCenter, UIAlignment.BottomRight,
        };

        for (var i = 0; i < nbRow; i++)
        {
            lyt.Reset(3, i * (cellSize.y + 3));
            for (var j = 0; j < nbCol; j++)
            {
                var r = lyt.Col(cellSize.x, cellSize.y);
                Spritz.DrawRectangle(r.x, r.y, r.width, r.height, Color.blue, true);
                
                var aIndex = (i % 3) * 3 + j % 3;
                var a = alignment[aIndex];
                var aString = a.ToString().Replace("Top", "T_").Replace("Mid", "M_").Replace("Bottom", "B_");
                var style = new Style() { fg = Color.red };

                var content = new UIContent();
                if (i < 3 && j < 3)
                {
                    // Only text
                    content.textAlignment = a;
                    content.textValue = aString;
                }
                else if (i < 3 && j > 2)
                {
                    // Only sprite
                    content.spriteAlignment = a;
                    content.sprite = m_Sprite;
                }
                else if (i >=3 && i < 6 && j < 3)
                {
                    // Text and sprite: both expand
                    content.spriteAlignment = a;
                    content.sprite = m_Sprite;
                    content.textAlignment = a;
                    content.textValue = aString;
                }
                else if (i >= 3 && i < 6 && j > 2)
                {
                    // Sprite and Text both expand
                    content.spriteAlignment = a | UIAlignment.ExpandWidth;
                    content.sprite = m_Sprite;
                    content.textAlignment = a | UIAlignment.ExpandWidth | UIAlignment.RightZone;
                    content.textValue = aString;
                }
                else if (j < 3) 
                {
                    // Sprite expand
                    content.spriteAlignment = a | UIAlignment.ExpandWidth;
                    content.sprite = m_Sprite;
                    content.textAlignment = a;
                    content.textValue = aString;
                }
                else
                {
                    // text expand
                    content.spriteAlignment = a;
                    content.sprite = m_Sprite;
                    content.textAlignment = a | UIAlignment.ExpandWidth;
                    content.textValue = aString;
                }
                SpritzUITheme.DrawContent(content, r, style, Spritz.font);
            }
        }
    }
}

