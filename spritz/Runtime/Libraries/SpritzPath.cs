using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    public struct NodePath
    {
        public Vector2 pos;
        public float distance;
        public float angle;
        public Color32 color;
        public float speed;
        public int size;
        public bool lineVisible;
        public bool visible;

        public static NodePath Node(float x, float y)
        {
            return new NodePath()
            {
                pos = new Vector2(x, y)
            };
        }

        public override string ToString()
        {
            return $"{pos} {color}";
        }
    }

    public class SpritzPath
    {
        Vector2 m_Pos;
        Vector2 m_CurrentPos;
        string m_Name;
        Color32 m_Color;
        float m_Speed;
        int m_PenSize;
        bool m_Drawing;
        float m_Angle;
        float m_DrawAngle;
        float m_TotalDistance;
        float m_Dt;
        bool m_Playing;
        int m_NodeIndex;
        bool m_Finalized;
        bool m_Debug;
        List<NodePath> m_Nodes;
        Vector2 m_LastNodeDrawPos;

        public float x
        {
            get
            {
                return m_Nodes.Count > 0 ? m_Nodes[m_Nodes.Count - 1].pos.x : m_CurrentPos.x;
            }
            set
            {
                Goto(value, y);
            }
        }

        public float y
        {
            get
            {
                return m_Nodes.Count > 0 ? m_Nodes[m_Nodes.Count - 1].pos.y : m_CurrentPos.y;
            }
            set
            {
                Goto(x, value);
            }
        }

        public float heading
        {
            get
            {
                return m_Angle;
            }
            set
            {
                m_Angle = Mathf.Deg2Rad * value;
            }
        }

        public Vector2 position => m_CurrentPos;

        public bool isDrawing
        {
            get => m_Drawing;
            set => m_Drawing = value;
        }

        public int penSize
        {
            get => m_PenSize;
            set => m_PenSize = value;
        }

        public bool isPlaying
        {
            get => m_Playing;
            set => m_Playing = value;
        }

        public float speed
        {
            get => m_Speed;
            set => m_Speed = value;
        }

        public Color32 color
        {
            get => m_Color;
            set => m_Color = value;
        }

        public bool debugDraw
        {
            get => m_Debug;
            set => m_Debug = value;
        }

        public bool finalized
        {
            get => m_Finalized;
        }

        public SpritzPath(float x, float y, Color32 color, string name = "no name")
        {
            m_CurrentPos = m_Pos = new Vector2(x, y);
            m_Name = name;
            m_Color = color;
            m_Speed = 10;
            m_PenSize = 1;
            m_Nodes = new List<NodePath>();
            m_Drawing = true;
            m_Angle = 0;
            m_DrawAngle = 0;
            m_TotalDistance = 0;
            m_Dt = 0;
            m_Playing = true;
            m_NodeIndex = -1;
            m_Finalized = false;
            m_Debug = false;
        }

        public void Update()
        {
            if (m_Nodes.Count == 0 || m_Finalized || !isPlaying)
                return;
            var lastPos = m_Pos;
            var nodeIndex = Mathf.Max(m_NodeIndex, 0);
            var node = m_Nodes[nodeIndex];
            var speed = node.speed;
            var angle = node.angle;
            m_Dt += Time.deltaTime * speed * 10;
            var ratio = Mathf.Min(1, Mathf.Max(0, m_Dt / m_TotalDistance));
            var reachDistance = m_TotalDistance * ratio;
            for (var i = 0; i < m_Nodes.Count; ++i)
            {
                node = m_Nodes[i];
                var diff = reachDistance - node.distance;
                if (diff < 0)
                {
                    m_NodeIndex = i;
                    m_LastNodeDrawPos = Vector2.Lerp(lastPos, node.pos, reachDistance / node.distance);
                    m_CurrentPos = m_LastNodeDrawPos;
                    break;
                }

                reachDistance = diff;
                lastPos = node.pos;
                m_CurrentPos = lastPos;
            }

            m_DrawAngle = angle;
            if (ratio == 1 && !m_Finalized)
            {
                m_NodeIndex = m_Nodes.Count - 1;
                m_LastNodeDrawPos = m_Nodes[m_NodeIndex].pos;
                m_Finalized = true;
            }
        }

        public void Draw()
        {
            DrawPath();
            if (m_Debug)
                DrawDebug();
        }

        public SpritzPath PenDown()
        {
            isDrawing = true;
            return this;
        }

        public SpritzPath PenUp()
        {
            isDrawing = false;
            return this;
        }

        public SpritzPath Goto(float x, float y)
        {
            var v = new Vector2(x, y);
            var pos = m_Pos;
            if (m_Nodes.Count > 0)
                pos = m_Nodes[m_Nodes.Count - 1].pos;
            var angle = Vector2.Angle(v, pos);
            var node = CreateNode(x, y);
            node.angle = angle;
            node.visible = m_Drawing;
            m_Nodes.Add(node);
            return this;
        }

        public SpritzPath Home()
        {
            return Goto(m_Pos.x, m_Pos.y);
        }

        public SpritzPath Forward(float d)
        {
            var pos = m_Pos;
            if (m_Nodes.Count > 0)
                pos = m_Nodes[m_Nodes.Count - 1].pos;
            pos = pos.AddScalarWithAngle(d, m_Angle);
            m_Nodes.Add(CreateNode(pos.x, pos.y));
            m_Finalized = false;
            CalculateTotalDistance();
            return this;
        }

        public SpritzPath Back(float d)
        {
            return Forward(-d);
        }

        public SpritzPath Right(float angleInDegrees)
        {
            m_Angle += Mathf.Deg2Rad * angleInDegrees;
            return this;
        }

        public SpritzPath Left(float angleInDegrees)
        {
            m_Angle -= Mathf.Deg2Rad * angleInDegrees;
            return this;
        }

        public SpritzPath TurnLeft()
        {
            heading = -90;
            return this;
        }

        public SpritzPath TurnRight()
        {
            heading = 90;
            return this;
        }

        public SpritzPath Undo(int nbOfSteps = 1)
        {
            var end = Mathf.Max(0, m_Nodes.Count - nbOfSteps);
            for (var i = m_Nodes.Count - 1; i >= end; --i)
            {
                m_Nodes.RemoveAt(i);
            }
            m_NodeIndex = Mathf.Min(m_Nodes.Count - 1, m_NodeIndex);
            CalculateTotalDistance();
            return this;
        }

        public void Clear()
        {
            m_NodeIndex = -1;
            m_Dt = 0;
            m_Finalized = false;
            m_Nodes.Clear();
        }

        public void Reset()
        {
            m_CurrentPos = m_Pos;
            m_NodeIndex = -1;
            m_Dt = 0;
            m_Finalized = false;
        }

        private NodePath CreateNode(float x, float y)
        {
            var n = NodePath.Node(x, y);
            n.speed = m_Speed;
            n.color = m_Color;
            n.angle = m_Angle;
            n.size = m_PenSize;
            n.lineVisible = m_Drawing;
            return n;
        }

        private void CalculateTotalDistance()
        {
            var dist = 0f;
            var lastPos = m_Pos;
            for (var i = 0; i < m_Nodes.Count; ++i)
            {
                var n = m_Nodes[i];
                var vd = Vector2.Distance(n.pos, lastPos);
                n.distance = vd;
                m_Nodes[i] = n;
                dist += vd;
                lastPos = n.pos;
            }
            m_TotalDistance = dist;
        }

        private void DrawPath()
        {
            var lastPos = m_Pos;
            for (var i = 0; i <= m_NodeIndex; ++i)
            {
                var node = m_Nodes[i];
                if (node.lineVisible)
                {
                    var drawPos = node.pos;

                    if (i == m_NodeIndex)
                    {
                        drawPos = m_LastNodeDrawPos;
                    }

                    Spritz.DrawLine((int)lastPos.x, (int)lastPos.y, (int)drawPos.x, (int)drawPos.y, node.color, penSize);
                }
                lastPos = node.pos;
            }
        }

        private void DrawDebug()
        {
            for (var i = 0; i < m_Nodes.Count; ++i)
            {
                var node = m_Nodes[i];
                var drawPos = node.pos;
                var c = i <= m_NodeIndex ? Spritz.palette[2] : Spritz.palette[3];
                Spritz.DrawCircle((int)drawPos.x, (int)drawPos.y, 2, c, true);
                Spritz.Print($"{i}", (int)drawPos.x, (int)drawPos.y, Spritz.palette[5]);
            }

            Spritz.DrawCircle((int)m_LastNodeDrawPos.x, (int)m_LastNodeDrawPos.y, 2, Spritz.palette[4], true);
        }
    }
}
