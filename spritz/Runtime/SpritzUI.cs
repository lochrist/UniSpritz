using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    public delegate void UIDrawer(SpritzUI ui, SpritzUITheme theme, DrawCommand command);
    public delegate bool HitTest(int x, int y);

    public class SpritzCellLayout
    {
        struct LayoutState
        {
            public RectInt currentCellRect;
            public Vector2Int padding;
            public bool isFirstCell;
        }

        public RectInt currentCellRect
        {
            get => m_CurrentState.currentCellRect;
            set => m_CurrentState.currentCellRect = value;
        }

        public Vector2Int currentCellSize => currentCellRect.size;
        public Vector2Int currentCellPos => currentCellRect.position;

        public Vector2Int padding
        {
            get => m_CurrentState.padding;
            set => m_CurrentState.padding = value;
        }
        public Vector2Int defaultCellSize;

        private LayoutState m_CurrentState;
        private Stack<LayoutState> m_StateStack;

        // TODO: keep ordered list of rect to iterate? and to be able to go up and down iteration
        // TODO: Group to keep rect relative to the parent

        public SpritzCellLayout(int x = 0, int y = 0)
        {
            defaultCellSize = new Vector2Int(20, 20);
            m_CurrentState = new LayoutState();
            m_StateStack = new Stack<LayoutState>();
            Reset(x, y);
        }

        public void Reset(int x = 0, int y = 0)
        {
            currentCellRect = new RectInt(x, y, int.MaxValue, int.MaxValue);
            m_CurrentState.isFirstCell = true;
        }

        public void Push(int x = 0, int y = 0)
        {
            m_StateStack.Push(m_CurrentState);
            Reset(x, y);
        }

        public void Pop()
        {
            var lastSize = currentCellSize;
            m_CurrentState = m_StateStack.Pop();
            m_CurrentState.currentCellRect.width = Mathf.Max(lastSize.x, GetValidLength(currentCellRect.width));
            m_CurrentState.currentCellRect.height = Mathf.Max(lastSize.y, GetValidLength(currentCellRect.height));
        }

        public RectInt Down(int w = 0, int h = 0)
        {
            var x = currentCellRect.x;
            var y = currentCellRect.y + GetValidLength(currentCellRect.height);
            GetValidSizeOrDefault(ref w, ref h);
            if (!m_CurrentState.isFirstCell)
                y += padding.x;
            m_CurrentState.isFirstCell = false;
            currentCellRect = new RectInt(x, y, w, h);
            return currentCellRect;
        }

        public RectInt Row(int w = 0, int h = 0)
        {
            return Down(w, h);
        }

        public RectInt Up(int w = 0, int h = 0)
        {
            var x = currentCellRect.x;
            var y = currentCellRect.y - GetValidLength(currentCellRect.height);
            GetValidSizeOrDefault(ref w, ref h);
            if (!m_CurrentState.isFirstCell)
                y -= padding.x;
            m_CurrentState.isFirstCell = false;
            currentCellRect = new RectInt(x, y, w, h);
            return currentCellRect;
        }

        public RectInt Right(int w = 0, int h = 0)
        {
            var x = currentCellRect.x + GetValidLength(currentCellRect.width);
            var y = currentCellRect.y;
            GetValidSizeOrDefault(ref w, ref h);
            if (!m_CurrentState.isFirstCell)
                x += padding.x;
            m_CurrentState.isFirstCell = false;
            currentCellRect = new RectInt(x, y, w, h);
            return currentCellRect;
        }

        public RectInt Col(int w = 0, int h = 0)
        {
            return Right(w, h);
        }

        public RectInt Left(int w = 0, int h = 0)
        {
            var x = currentCellRect.x - GetValidLength(currentCellRect.width);
            var y = currentCellRect.y;
            GetValidSizeOrDefault(ref w, ref h);
            if (!m_CurrentState.isFirstCell)
                x -= padding.x;
            m_CurrentState.isFirstCell = false;
            currentCellRect = new RectInt(x, y, w, h);
            return currentCellRect;
        }

        private void GetValidSizeOrDefault(ref int w, ref int h)
        {
            w = w > 0 ? w : (currentCellRect.width != int.MaxValue) ? currentCellRect.width : defaultCellSize.x;
            h = h > 0 ? h : (currentCellRect.height != int.MaxValue) ? currentCellRect.height : defaultCellSize.y;
        }

        private int GetValidLength(int l)
        {
            return l == int.MaxValue ? 0 : l;
        }
    }

    public struct Style
    {
        public Style(Color32 fg, Color32 bg)
        {
            this.fg = fg;
            this.bg = bg;
            sprite = new SpriteId();
        }

        public Style(SpriteId sprite)
        {
            this.fg = Color.clear;
            this.bg = Color.clear;
            this.sprite = sprite;
        }

        public Color32 fg;
        public Color32 bg;
        public SpriteId sprite;
        public bool isValid => (fg.a != 0 && bg.a != 0) || sprite.isValid;
    }

    public struct ControlStyle
    {
        public ControlStyle(Style normal, Style hovered, Style active)
        {
            this.normal = normal;
            this.hovered = hovered;
            this.active = active;
            font = null;
            this.isValid = true;
        }

        public Font font;
        public Style normal;
        public Style hovered;
        public Style active;
        public bool isValid;
    }

    public class SpritzUITheme
    {
        public Font defaultFont;
        
        public ControlStyle defaultStyle;
        public ControlStyle buttonStyle;
        public ControlStyle labelStyle;
        public ControlStyle checkboxStyle;

        public UIDrawer checkboxDrawer;
        public UIDrawer buttonDrawer;
        public UIDrawer labelDrawer;

        public void GetStyle(ControlStyle baseStyle, DrawCommand c, out Style style, out Font font)
        {
            var controlStyle = defaultStyle;
            if (c.opts.style.isValid)
            {
                controlStyle = c.opts.style;
            }
            else if (baseStyle.isValid)
            {
                controlStyle = baseStyle;
            }
            style = GetStyle(controlStyle, c.state);
            font = c.opts.style.font ?? baseStyle.font ?? defaultFont;
        }

        public Style GetStyle(ControlStyle baseStyle, UIState state)
        {
            if (state == UIState.Active)
                return baseStyle.active;
            if (state == UIState.Hovered)
                return baseStyle.hovered;
            return baseStyle.normal;
        }

        static RectInt zeroRect = new RectInt(0, 0, 0, 0);

        public static void AlignContent(UIContent content, Font font, RectInt rect, out RectInt textRect, out RectInt spriteRect)
        {
            textRect = zeroRect;
            spriteRect = zeroRect;

            if (content.sprite.isValid && !string.IsNullOrEmpty(content.textValue))
            {
                var spriteAlignment = GetValidAlignment(content.spriteAlignment);
                var spriteSize = GetSize(content.sprite);
                var textAlignment = GetValidAlignment(content.textAlignment);
                var textSize = font.GetTextSize(content.textValue);
                var expandLeft = false;
                var expandRight = false;
                var testOnLeft = !content.textAlignment.HasFlag(UIAlignment.RightZone) && !content.spriteAlignment.HasFlag(UIAlignment.LeftZone);
                var leftSideMinSize = textSize;
                var rightSideMinSize = spriteSize;

                if (testOnLeft)
                {
                    expandLeft = content.textAlignment.HasFlag(UIAlignment.ExpandWidth);
                    expandRight = content.spriteAlignment.HasFlag(UIAlignment.ExpandWidth);
                }
                else
                {
                    expandLeft = content.spriteAlignment.HasFlag(UIAlignment.ExpandWidth);
                    expandRight = content.textAlignment.HasFlag(UIAlignment.ExpandWidth);
                    leftSideMinSize = spriteSize;
                    rightSideMinSize = textSize;
                }

                var leftSideRect = rect;
                var rightSideRect = leftSideRect.CutRight(rect.width / 2);
                if (expandLeft)
                {
                    if (!expandRight)
                    {
                        leftSideRect = rect;
                        rightSideRect = leftSideRect.CutRight(rightSideMinSize.x);
                    }
                }
                else if (expandRight)
                {
                    rightSideRect = rect;
                    leftSideRect = rightSideRect.CutLeft(leftSideMinSize.x);
                }

                if (testOnLeft)
                {
                    AlignContentRect(textAlignment, leftSideRect, textSize, out textRect);
                    AlignContentRect(spriteAlignment, rightSideRect, spriteSize, out spriteRect);
                }
                else
                {
                    AlignContentRect(spriteAlignment, leftSideRect, spriteSize, out spriteRect);
                    AlignContentRect(textAlignment, rightSideRect, textSize, out textRect);
                }
            }
            else if (content.sprite.isValid)
            {
                // Sprite takes all space
                var alignment = GetValidAlignment(content.spriteAlignment);
                var spriteSize = GetSize(content.sprite);
                AlignContentRect(alignment, rect, spriteSize, out spriteRect);
            }
            else if (!string.IsNullOrEmpty(content.textValue))
            {
                // Text tables all space
                var alignment = GetValidAlignment(content.textAlignment);
                var textSize = font.GetTextSize(content.textValue);
                AlignContentRect(alignment, rect, textSize, out textRect);
            }
        }

        public static bool IsValid(RectInt r)
        {
            return r.width > 0 && r.height > 0;
        }

        public static UIAlignment GetValidAlignment(UIAlignment a)
        {
            if (a == UIAlignment.None)
                return UIAlignment.Center;
            if (a.HasFlag(UIAlignment.TopRight))
                return UIAlignment.TopRight;
            if (a.HasFlag(UIAlignment.MidRight))
                return UIAlignment.MidRight;
            if (a.HasFlag(UIAlignment.BottomRight))
                return UIAlignment.BottomRight;

            if (a.HasFlag(UIAlignment.TopCenter))
                return UIAlignment.TopCenter;
            if (a.HasFlag(UIAlignment.Center))
                return UIAlignment.Center;
            if (a.HasFlag(UIAlignment.BottomCenter))
                return UIAlignment.BottomCenter;

            if (a.HasFlag(UIAlignment.TopLeft))
                return UIAlignment.TopLeft;
            if (a.HasFlag(UIAlignment.BottomLeft))
                return UIAlignment.BottomLeft;
            if (a.HasFlag(UIAlignment.MidLeft))
                return UIAlignment.MidLeft;
            return UIAlignment.Center;
        }

        public static Vector2Int GetSize(SpriteId sprite)
        {
            var s = Spritz.GetSpriteDesc(sprite).rect.size;
            return new Vector2Int((int)s.x, (int)s.y);
        }

        public static void AlignContentRect(UIAlignment a, RectInt r, Vector2Int size, out RectInt contentRect)
        {
            contentRect = new RectInt(0, 0, size.x, size.y);
            switch (a)
            {
                case UIAlignment.TopRight:
                    contentRect.y = r.y;
                    contentRect.x = r.xMax - size.x;
                    break;
                case UIAlignment.TopLeft:
                    contentRect.y = r.y;
                    contentRect.x = r.x;
                    break;
                case UIAlignment.TopCenter:
                    contentRect.y = r.y;
                    contentRect.x = (r.x + (r.width / 2)) - (size.x / 2);
                    break;
                case UIAlignment.MidRight:
                    contentRect.y = (r.y + (r.height / 2)) - (size.y / 2);
                    contentRect.x = r.xMax - size.x;
                    break;
                case UIAlignment.MidLeft:
                    contentRect.y = (r.y + (r.height / 2)) - (size.y / 2);
                    contentRect.x = r.x;
                    break;
                case UIAlignment.BottomRight:
                    contentRect.y = r.yMax - size.y;
                    contentRect.x = r.xMax - size.x;
                    break;
                case UIAlignment.BottomLeft:
                    contentRect.y = r.yMax - size.y;
                    contentRect.x = r.x;
                    break;
                case UIAlignment.BottomCenter:
                    contentRect.x = (r.x + (r.width / 2)) - (size.x / 2);
                    contentRect.y = r.yMax - size.y;
                    break;
                default:
                    contentRect.x = (r.x + (r.width / 2)) - (size.x / 2);
                    contentRect.y = (r.y + (r.height / 2)) - (size.y / 2);
                    // Go full center
                    break;
            }
        }

        public SpritzUITheme()
        {
            defaultStyle = new ControlStyle(
                new Style(new Color(0.73f, 0.73f, 0.73f), new Color(0.25f, 0.25f, 0.25f)),
                new Style(new Color(1f, 1f, 1f), new Color(0.19f, 0.6f, 0.73f)),
                new Style(new Color(1f, 1f, 1f), new Color(1f, 0.6f, 0f)));

            buttonStyle = labelStyle = checkboxStyle = defaultStyle;
            defaultFont = Spritz.font;

            buttonDrawer = DefaultButtonDraw;
            labelDrawer = DefaultLabel;
            checkboxDrawer = DefaultCheckbox;
        }

        public static void DrawBackground(DrawCommand c, Style style)
        {
            if (style.sprite.isValid)
            {
                Spritz.DrawSprite(style.sprite, c.rect.x, c.rect.y);
            }
            else
            {
                Spritz.DrawRectangle(c.rect.x, c.rect.y, c.rect.width, c.rect.height, style.bg, true);
            }
        }

        public static void DrawContent(DrawCommand c, Style style, Font font)
        {
            DrawContent(c.content, c.rect, style, font);
        }

        public static void DrawContent(UIContent content, RectInt rect, Style style, Font font)
        {
            AlignContent(content, font, rect, out var textRect, out var spriteRect);
            if (IsValid(spriteRect))
            {
                Spritz.DrawSprite(content.sprite, spriteRect.x, spriteRect.y);
            }

            if (IsValid(textRect))
            {
                // Could better compute y to better aligned text
                Spritz.Print(font, content.textValue, textRect.x + 2, textRect.y + 2, style.fg);
            }
        }

        #region defaultDrawers
        public static void DefaultButtonDraw(SpritzUI ui, SpritzUITheme theme, DrawCommand c)
        {
            theme.GetStyle(theme.buttonStyle, c, out var style, out var font);
            DrawBackground(c, style);
            DrawContent(c, style, font);
        }

        public static void DefaultLabel(SpritzUI ui, SpritzUITheme theme, DrawCommand c)
        {
            theme.GetStyle(theme.buttonStyle, c, out var style, out var font);
            // DrawBackground(c, style);
            DrawContent(c, style, font);
        }

        public static void DefaultCheckbox(SpritzUI ui, SpritzUITheme theme, DrawCommand c)
        {
            theme.GetStyle(theme.buttonStyle, c, out var style, out var font);
            DrawBackground(c, style);
            DrawContent(c, style, font);
        }
        #endregion
    }

    public enum UIState
    { 
        Active,
        Hovered,
        Hit,
        Normal
    }

    public struct UIResult
    {
        public int id;
        public bool isHit;
        public bool isHovered;
        public bool mouseEntering;
        public bool mouseLeaving;
    }

    public enum UIAlignment
    {
        None = 0,
        TopRight = 1 << 1,
        MidRight = 1 << 2,
        BottomRight = 1 << 3,

        TopCenter = 1 << 4,
        Center = 1 << 5,
        BottomCenter = 1 << 6,

        TopLeft = 1 << 7,
        MidLeft = 1 << 8,
        BottomLeft = 1 << 9,

        LeftZone = 1 << 10,
        RightZone = 1 << 11,

        ExpandWidth = 1 << 12,
    }

    public struct UIContent
    {
        public SpriteId sprite;
        public UIAlignment spriteAlignment;
        public string textValue;
        public UIAlignment textAlignment;
        public int intValue;
        public float floatValue;
    }

    public struct DrawCommand
    {
        public int id;
        public RectInt rect;
        public UIState state;
        public UIContent content;
        public UIOptions opts;
        public UIDrawer drawer;
    }

    public struct UIOptions
    {
        public UIDrawer drawer;
        public ControlStyle style;
    }

    public class SpritzUI
    {
        public Vector2Int mousePos;
        public SpritzUITheme theme;

        public bool buttonDown
        {
            get;
            set;
        }

        public int hovered
        {
            get;
            set;
        }

        public int hoveredLast
        {
            get;
            set;
        }

        public int active
        {
            get;
            set;
        }

        public int hit
        {
            get;
            set;
        }

        public int keyboardFocus
        {
            get;
            set;
        }

        public bool anyHovered => hovered != 0;
        public bool anyActive => active != 0;
        public bool anyKeyboardFocus => keyboardFocus != 0;

        public int keyDown
        {
            get;
            set;
        }

        public char textChar
        {
            get;
            set;
        }

        public string candidateText
        {
            get;
            set;
        }

        public int candidateStart
        {
            get;
            set;
        }

        public int candidateLength
        {
            get;
            set;
        }

        private List<DrawCommand> m_Commands;

        public SpritzUI(SpritzUITheme theme = null)
        {
            this.theme = theme ?? new SpritzUITheme();
            m_Commands = new List<DrawCommand>(512);
        }

        public void SetHit(int id)
        {
            hit = id;
            active = id;
            hovered = id;
            buttonDown = false;
        }

        public UIState GetState(int id)
        {
            if (active == id)
                return UIState.Active;

            if (hovered == id)
                return UIState.Hovered;

            if (hit == id)
                return UIState.Hit;

            return UIState.Normal;
        }

        public UIState RegisterMouseHit(int id, int x, int y, HitTest hitTest)
        {
            if (!anyHovered && hitTest(mousePos.x - x, mousePos.y - y))
            {
                hovered = id;
                if (!anyActive && buttonDown)
                {
                    active = id;
                }
            }
            return GetState(id);
        }

        public UIState RegisterHitBox(int id, int x, int y, int w, int h)
        {
            return RegisterMouseHit(id, x, y, (x, y) =>
            {
                return x >= 0 && x <= w && y >= 0 && y <= h;
            });
        }

        public bool CheckMouseReleasedOn(int id)
        {
            if (!buttonDown && active == id && hovered == id)
            {
                hit = id;
                return true;
            }
            return false;
        }

        public void UpdateMouse(int x, int y, bool buttonDown)
        {
            mousePos = new Vector2Int(x, y);
            this.buttonDown = buttonDown;
        }

        public void OnKeyPressed(int k)
        {
            keyDown = k;
        }

        public void OnTextInput(char c)
        {
            textChar = c;
        }

        public void OnTextEdited(string text, int start, int length)
        {
            candidateText = text;
            candidateStart = start;
            candidateLength = length;
        }

        public bool GrabKeyboardFocus(int id)
        {
            if (active == id)
            {
                // Grab keyboard
                keyboardFocus = id;
            }
            return anyKeyboardFocus;
        }

        public bool IsKeyPressedOn(int id, int key)
        {
            return keyboardFocus == id && keyDown == key;
        }

        public void StartFrame()
        {
            if (!buttonDown)
                active = 0;
            else if (!anyActive)
                active = 0;

            hoveredLast = hovered;
            hovered = 0;
            keyDown = 0;
            textChar = (char)0;
            GrabKeyboardFocus(0);
            hit = 0;
        }

        public void EndFrame()
        {

        }

        public void Update()
        {
            mousePos = Spritz.pixelMousePos;
            buttonDown = Spritz.GetMouseDown(0);
        }

        public void RegisterDraw(DrawCommand c)
        {
            m_Commands.Add(c);
        }

        public void Draw()
        {
            // Empty draw command queue
            foreach(var c in m_Commands)
            {
                c.drawer(this, theme, c);
            }
            m_Commands.Clear();

            EndFrame();
        }

        public UIResult DefaultUIResult(int id)
        {
            return new UIResult()
            {
                id = id,
                isHit = CheckMouseReleasedOn(id),
                isHovered = hovered == id,
                mouseEntering = hovered == id && hoveredLast != id,
                mouseLeaving = hovered != id && hoveredLast == id
            };
        }
    }

    public static class SpritzUIExtensions
    {
        public static UIResult Button(this SpritzUI ui, RectInt r, string text, UIOptions opts = new UIOptions())
        {
            var id = text.GetHashCode();
            var w = r.width > 0 ? r.width : text.Length * 7;
            var h = r.height > 0 ? r.height : 7;
            var uiState = ui.RegisterHitBox(id, r.x, r.y, w, h);
            var drawCommand = new DrawCommand()
            {
                id = id,
                rect = r,
                opts = opts,
                state = uiState,
                content = new UIContent() { textValue = text },
                drawer = opts.drawer ?? ui.theme.buttonDrawer
            };
            ui.RegisterDraw(drawCommand);
            return ui.DefaultUIResult(id);
        }

        public static UIResult Button(this SpritzUI ui, RectInt r, SpriteId sprite, UIOptions opts = new UIOptions())
        {
            var id = sprite.GetHashCode();
            var uiState = ui.RegisterHitBox(id, r.x, r.y, r.width, r.height);
            var drawCommand = new DrawCommand()
            {
                id = id,
                rect = r,
                opts = opts,
                state = uiState,
                content = new UIContent() { sprite = sprite },
                drawer = opts.drawer ?? ui.theme.buttonDrawer
            };
            ui.RegisterDraw(drawCommand);
            return ui.DefaultUIResult(id);
        }

        public static UIResult Checkbox(this SpritzUI ui, RectInt r, ref bool value, string text, UIOptions opts = new UIOptions())
        {
            var id = text.GetHashCode();
            var w = r.width > 0 ? r.width : text.Length * 7;
            var h = r.height > 0 ? r.height : 7;
            var uiState = ui.RegisterHitBox(id, r.x, r.y, w, h);
            if (ui.CheckMouseReleasedOn(id))
            {
                value = !value;
            }
            var drawCommand = new DrawCommand()
            {
                id = id,
                rect = r,
                opts = opts,
                state = uiState,
                content = new UIContent() { textValue = text },
                drawer = opts.drawer ?? ui.theme.checkboxDrawer
            };
            ui.RegisterDraw(drawCommand);
            return ui.DefaultUIResult(id);
        }

        public static UIResult Label(this SpritzUI ui, RectInt r, string text, UIOptions opts = new UIOptions())
        {
            var id = text.GetHashCode();
            var w = r.width > 0 ? r.width : text.Length * 7;
            var h = r.height > 0 ? r.height : 7;
            var uiState = ui.RegisterHitBox(id, r.x, r.y, w, h);
            var drawCommand = new DrawCommand()
            {
                id = id,
                rect = r,
                opts = opts,
                state = uiState,
                content = new UIContent() { textValue = text },
                drawer = opts.drawer ?? ui.theme.labelDrawer
            };
            ui.RegisterDraw(drawCommand);
            return ui.DefaultUIResult(id);
        }
    }
}
