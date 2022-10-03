using System.Collections.Generic;
using System.Linq;
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

    public struct AnimTick
    {
        public int frameIndex;
        public int numberOfFrames;
        public bool loop;
        public float fps;
        public float duration;
        public bool hasValue => loop || frameIndex < numberOfFrames - 1;
        public bool playing;
        public int lastFrameIndex;

        private float m_PlayTime;
        private float m_TimePerFrame;

        public AnimTick(float fps, int numFrames, bool loop = false)
        {
            this.fps = fps;
            numberOfFrames = numFrames;
            duration = numFrames / fps;
            this.loop = loop;

            m_TimePerFrame = 1f / fps;
            lastFrameIndex = -1;
            frameIndex = 0;
            m_PlayTime = 0;
            playing = true;
        }

        public AnimTick(float fps, float duration, bool loop = false)
        {
            this.fps = fps;
            this.duration = duration;
            numberOfFrames = Mathf.RoundToInt(duration * fps);
            this.loop = loop;

            m_TimePerFrame = 1f / fps;
            lastFrameIndex = -1;
            frameIndex = 0;
            m_PlayTime = 0;
            playing = true;
        }

        public void SetFps(float fps)
        {
            this.fps = fps;
            m_TimePerFrame = 1f / fps;
            Reset();
        }

        public void Reset()
        {
            frameIndex = 0;
            lastFrameIndex = -1;
            m_PlayTime = 0;
        }

        public void Update()
        {
            // if the animation doesn't loop we stop at the last frame
            if (!playing || !hasValue)
                return;

            m_PlayTime += Spritz.deltaTime;

            // update to the next frame if it's time
            lastFrameIndex = frameIndex;
            while (m_PlayTime * fps >= 1)
            {
                frameIndex = loop ? ++frameIndex % numberOfFrames : ++frameIndex;
                m_PlayTime -= m_TimePerFrame;
            }
        }
    }

    public struct AnimSprite
    {
        public SpriteId[] frames;
        public AnimTick ticker;
        public bool isValid => ticker.fps > 0 && frames != null && frames.Length > 0;
        public SpriteId current => frames[ticker.frameIndex];
        public int frameIndex => ticker.frameIndex;

        public static AnimSprite CreateAnimSprite(float fps, Sprite[] sprites, bool loop)
        {
            var ids = sprites.Select(s => new SpriteId(s.name)).ToArray();
            return new AnimSprite(fps, ids, loop);
        }

        public AnimSprite(float fps, SpriteId[] frames, bool loop = false)
        {
            this.frames = frames;
            ticker = new AnimTick(fps, frames != null ? frames.Length : 0, loop);
        }

        public void SetFps(float fps)
        {
            ticker.SetFps(fps);
        }

        public void Update()
        {
            ticker.Update();
        }

        public void Draw(int x, int y)
        {
            Spritz.DrawSprite(current, x, y);
        }

        public void Reset()
        {
            ticker.Reset();
        }
    }
}
