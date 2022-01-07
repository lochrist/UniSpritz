using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    public delegate AnimNode CombineNode(AnimNode node);
    public delegate AnimNode EvaluationFunction(float t, AnimNode node, float tstart);
    public delegate float EasingFunction(float vstart, float tlin, float vend);

    public struct AnimNode
    {
        public float duration;
        private Dictionary<string, float> m_Values;
        public Dictionary<string, float> values {
            get
            {
                if (m_Values == null)
                    m_Values = new Dictionary<string, float>();
                return m_Values;
            }
        }
        public bool hasValues => m_Values != null && m_Values.Count > 0;
        public AnimNode Evaluate(float t, AnimNode output, float tstart = 0)
        {
            return evaluate(t, output, tstart);
        }
        public AnimNode Evaluate(float t, float tstart = 0)
        {
            var n = new AnimNode()
            {
                m_Values = new Dictionary<string, float>()
            };
            return evaluate(t, n, tstart);
        }

        public AnimNode Parallel(AnimNode node)
        {
            return SpritzAnim.CombineParallel(this, node);
        }

        public AnimNode Sequence(AnimNode node)
        {
            return SpritzAnim.CombineSequence(this, node);
        }

        public EvaluationFunction evaluate;
    }

    public static class SpritzAnim
    {
        public static float EaseLinear(float vstart, float tlin, float vend)
        {
            return (1f - tlin) * vstart + tlin * vend;
        }

        public static float EaseCubic(float vstart, float tlin, float vend)
        {
            var v = 1 - tlin;
            var cube = v * v * v;
            return cube * vstart + (1 - cube) * vend;
        }

        public static float EaseOutBack(float vstart, float tlin, float vend)
        {
            var c1 = 1.70158f;
            var c3 = c1 + 1;
            var t = 1 + c3 * Mathf.Pow(tlin - 1, 3) + c1 * Mathf.Pow(tlin - 1, 2);
            return (1 - t) * vstart + t * vend;
        }

        public static AnimNode DelayAnimation(float t, AnimNode node, float tstart)
        {
            return node;
        }

        public static AnimNode CreateDelayNode(float duration)
        {
            var node = new AnimNode()
            {
                duration = duration,
                evaluate = DelayAnimation
            };
            return node;
        }

        public static AnimNode CreateValueNode(string valueId, float value)
        {
            var node = new AnimNode()
            {
                duration = 0,
                evaluate = (t, output, tstart) =>
                {
                    output.values[valueId] = value;
                    return output;
                }
            };
            return node;
        }

        public static AnimNode CreateInterpolationNode(EasingFunction easing, string valueId, float vend, float duration)
        {
            var node = new AnimNode()
            {
                duration = duration,
                evaluate = (t, output, tstart) =>
                {
                    if (t < tstart + duration && duration != 0)
                    {
                        var tlin = (t - tstart) / duration;
                        var vstart = output.values[valueId];
                        output.values[valueId] = easing(vstart, tlin, vend);
                    }
                    else
                    {
                        output.values[valueId] = vend;
                    }
                    return output;
                }
            };
            return node;
        }

        public static AnimNode CombineSequence(AnimNode a1, AnimNode a2)
        {
            var node = new AnimNode()
            {
                duration = a1.duration + a2.duration,
                evaluate = (t, output, tstart) =>
                {
                    a1.Evaluate(t, output, tstart);
                    if (t >= tstart + a1.duration)
                        a2.Evaluate(t, output, tstart + a1.duration);
                    return output;
                }
            };
            return node;
        }

        public static AnimNode CombineParallel(AnimNode a1, AnimNode a2)
        {
            var node = new AnimNode()
            {
                duration = Mathf.Max(a1.duration, a2.duration),
                evaluate = (t, output, tstart) =>
                {
                    if (t >= tstart) 
                    { 
                        a1.Evaluate(t, output, tstart);
                        a2.Evaluate(t, output, tstart);
                    }
                    return output;
                }
            };
            return node;
        }

        public static AnimNode CombineParallelList(params AnimNode[] nodes)
        {
            return CombineParallelList((IEnumerable<AnimNode>)nodes);
        }

        public static AnimNode CombineParallelList(IEnumerable<AnimNode> nodes)
        {
            var iter = nodes.GetEnumerator();
            iter.MoveNext();
            AnimNode n = iter.Current;
            while (iter.MoveNext())
            {
                n = n.Parallel(iter.Current);
            }
            return n;
        }

        public static AnimNode CombineSequenceList(params AnimNode[] nodes)
        {
            return CombineSequenceList((IEnumerable<AnimNode>)nodes);
        }

        public static AnimNode CombineSequenceList(IEnumerable<AnimNode> nodes)
        {
            var iter = nodes.GetEnumerator();
            iter.MoveNext();
            AnimNode n = iter.Current;
            while (iter.MoveNext())
            {
                n = n.Sequence(iter.Current);
            }
            return n;
        }

        public static AnimNode Stagger(IEnumerable<AnimNode> nodes, float delta)
        {
            var nodesWithDelay = nodes.Select(n => CreateDelayNode(delta).Sequence(n));
            return CombineParallelList(nodesWithDelay);
        }
    }

    // TODO: fully prototype. Api not satisfacory or clean.
    public struct AnimSprite
    {
        public SpriteId[] sprites;
        public int spriteIndex;
        // TODO: Start using looping.
        public bool loop;
        // TODO: this is not fps at all...
        public int fps;
        public SpriteId current => sprites[spriteIndex];
        private int m_Tick;

        // TODO: need a constructor?

        public void Tick()
        {
            ++m_Tick;
            spriteIndex = m_Tick / fps;
            // spriteIndex++;
            if (spriteIndex >= sprites.Length)
            {
                m_Tick = 0;
                spriteIndex = 0;
            }
        }
    }
}
