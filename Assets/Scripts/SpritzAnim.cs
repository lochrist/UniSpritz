using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    public delegate AnimNode CombineNode(AnimNode node);
    public delegate AnimNode Animation(float t, AnimNode node, float tstart);
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
        
        public Animation animate;
        public CombineNode parallel;
        public CombineNode sequence;
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
                animate = DelayAnimation
            };
            node.parallel = g => CombineParallel(node, g);
            node.sequence = g => CombineSequence(node, g);
            return node;
        }

        public static AnimNode CreateValueNode(string valueId, float value)
        {
            var node = new AnimNode()
            {
                duration = 0,
                animate = (t, node, tstart) =>
                {
                    node.values[valueId] = value;
                    return node;
                }
            };
            node.parallel = g => CombineParallel(node, g);
            node.sequence = g => CombineSequence(node, g);
            return node;
        }

        public static AnimNode CreateInterpolationNode(EasingFunction easing, string valueId, float vend, float duration)
        {
            var node = new AnimNode()
            {
                duration = duration,
                animate = (t, node, tstart) =>
                {
                    if (t < tstart + duration && duration != 0)
                    {
                        var tlin = (t - tstart) / duration;
                        var vstart = node.values[valueId];
                        node.values[valueId] = easing(vstart, tlin, vend);
                    }
                    else
                    {
                        node.values[valueId] = vend;
                    }
                    return node;
                }
            };
            node.parallel = g => CombineParallel(node, g);
            node.sequence = g => CombineSequence(node, g);
            return node;
        }

        public static AnimNode CombineSequence(AnimNode a1, AnimNode a2)
        {
            var node = new AnimNode()
            {
                duration = a1.duration + a2.duration,
                animate = (t, node, tstart) =>
                {
                    a1.animate(t, node, tstart);
                    if (t >= tstart + a1.duration)
                        a2.animate(t, node, tstart);
                    return node;
                }
            };
            node.parallel = g => CombineParallel(node, g);
            node.sequence = g => CombineSequence(node, g);
            return node;
        }

        public static AnimNode CombineParallel(AnimNode a1, AnimNode a2)
        {
            var node = new AnimNode()
            {
                duration = Mathf.Max(a1.duration, a2.duration),
                animate = (t, node, tstart) =>
                {
                    if (t >= tstart)
                        a1.animate(t, node, tstart);
                    else
                        a2.animate(t, node, tstart);
                    return node;
                }
            };
            node.parallel = g => CombineParallel(node, g);
            node.sequence = g => CombineSequence(node, g);

            return node;
        }

        public static AnimNode CombineParallel(IEnumerable<AnimNode> nodes)
        {
            var iter = nodes.GetEnumerator();
            var n = iter.Current;
            while (iter.MoveNext())
            {
                n = n.parallel(iter.Current);
            }
            return n;
        }

        public static AnimNode CombineSequence(IEnumerable<AnimNode> nodes)
        {
            var iter = nodes.GetEnumerator();
            var n = iter.Current;
            while (iter.MoveNext())
            {
                n = n.sequence(iter.Current);
            }
            return n;
        }
    }
}
