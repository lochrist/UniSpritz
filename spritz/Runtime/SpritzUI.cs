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

    public class SpritzUITheme
    {
        public struct Style
        {
            public Color32 fg;
            public Color32 bg;
            public SpriteId sprite; 
        }

        public Style normal;
        public Style hovered;
        public Style active;

        public UIDrawer checkboxDrawer;
        public UIDrawer buttonDrawer;
        public UIDrawer labelDrawer;

        public Style GetStyleForState(UIState state)
        {
            if (state == UIState.Active)
                return active;
            if (state == UIState.Hovered)
                return hovered;
            return normal;
        }

        public SpritzUITheme()
        {
            normal = new Style()
            {
                bg = new Color(0.25f, 0.25f, 0.25f),
                fg = new Color(0.73f, 0.73f, 0.73f),
            };

            hovered = new Style()
            {
                bg = new Color(0.19f, 0.6f, 0.73f),
                fg = new Color(1f, 1f, 1f),
            };

            active = new Style()
            {
                bg = new Color(1f, 0.6f, 0f),
                fg = new Color(1f, 1f, 1f),
            };

            buttonDrawer = DefaultButtonDraw;
            labelDrawer = DefaultLabel;
            checkboxDrawer = DefaultCheckbox;
        }

        #region defaultDrawers
        public static void DefaultButtonDraw(SpritzUI ui, SpritzUITheme theme, DrawCommand c)
        {
            var style = theme.GetStyleForState(c.state);
            Spritz.DrawRectangle(c.rect.x, c.rect.y, c.rect.width, c.rect.height, style.bg, true);
            if (c.sprite.isValid)
            {
                Spritz.DrawSprite(c.sprite, c.rect.x, c.rect.y);
            }
            else
            {
                // Could better compute y to better aligned text
                Spritz.Print(c.opts.font, c.textValue, c.rect.x + 2, c.rect.y + 2, style.fg);
            }
        }

        public static void DefaultLabel(SpritzUI ui, SpritzUITheme theme, DrawCommand c)
        {
            var style = theme.GetStyleForState(c.state);
            // Could better compute y to better aligned text
            Spritz.Print(c.opts.font, c.textValue, c.rect.x + 2, c.rect.y + 2, style.fg);
        }

        public static void DefaultCheckbox(SpritzUI ui, SpritzUITheme theme, DrawCommand c)
        {
            var style = theme.GetStyleForState(c.state);
            // Could better compute y to better aligned text
            Spritz.Print(c.opts.font, c.textValue, c.rect.x + 2, c.rect.y + 2, style.fg);

            // TODO
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

    public struct DrawCommand
    {
        public int id;
        public RectInt rect;
        public UIState state;
        public UIOptions opts;
        public UIDrawer drawer;
        public SpriteId sprite;
        public string textValue;
        public int intValue;
        public float floatValue;
    }

    public struct UIOptions
    {
        public Font font;
        public UIDrawer drawer;
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
                textValue = text,
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
                sprite = sprite,
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
                textValue = text,
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
                textValue = text,
                drawer = opts.drawer ?? ui.theme.labelDrawer
            };
            ui.RegisterDraw(drawCommand);
            return ui.DefaultUIResult(id);
        }
    }
}

