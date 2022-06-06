using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    [Flags]
    public enum ParticleBehaviors
    {
        None = 0,
        Gravity = 1 << 1,
        Size = 1 << 2,
        Velocity = 1 << 3,
        LifeBox = 1 << 5
    }

    public struct Particle
    {
        public static float gGravity = 50;

        public ParticleBehaviors behaviors;

        public int id;
        public Vector2 pos;
        public float lifeInitial;
        public float life;

        public Vector2 velocity;
        public Vector2 velInitial;
        public Vector2 velFinal;

        public bool dead;

        public float size;
        public float sizeInitial;
        public float sizeFinal;

        public AnimSprite sprite;
        
        public Color[] colors;
        public float colorDuration;
        public float currentColorTime;
        public int colorIndex;
        public Color currentColor;

        public void Init(ParticleBehaviors behaviors, float x, float y, float life, float angle, float initialSpeed, float finalSpeed, float initialSize, float finalSize)
        {
            this.behaviors = behaviors;

                pos = new Vector2(x, y);
            lifeInitial = this.life = life;

            var angleRadians = Mathf.Deg2Rad * angle;

            // TO check Why -Mathf.sin???
            velocity = new Vector2(initialSpeed * Mathf.Cos(angleRadians), initialSpeed * -Mathf.Sin(angleRadians));
            velInitial = velocity;
            velFinal = new Vector2(finalSpeed * Mathf.Cos(angleRadians), finalSpeed * -Mathf.Sin(angleRadians));

            dead = false;

            this.size = sizeInitial = initialSize;
            sizeFinal = finalSize;


            if (!SpritzUtil.Approximately(sizeInitial, sizeFinal))
            {
                this.behaviors |= ParticleBehaviors.Size;
            }

            if (!SpritzUtil.Approximately(velInitial.x, velFinal.x))
            {
                this.behaviors |= ParticleBehaviors.Velocity;
            }
        }

        public void SetColors(Color[] c)
        {
            sprite.fps = 0;
            colors = c;
            colorDuration = 1f / colors.Length * life;
            currentColorTime = colorDuration;
            colorIndex = 0;
            currentColor = colors[colorIndex];
        }

        public void SetSprites(AnimSprite s)
        {
            sprite = s;
            sprite.SetFps(sprite.frames.Length / life);
        }

        public void Update()
        {
            var dt = Time.deltaTime;

            life -= dt;

            // TO check Should Update of Particles be computed in Emitter with a different function/behavior?

            if (behaviors.HasFlag(ParticleBehaviors.Gravity))
            {
                velocity.y = velocity.y + Time.deltaTime * gGravity;
            }

            // Size over lifetime
            if (behaviors.HasFlag(ParticleBehaviors.Size))
            {
                var sizeChange = ((sizeInitial - sizeFinal) / lifeInitial) * dt;
                size -= sizeChange;
            }

            // Velocity over lifetime
            // why checking only x???
            if (behaviors.HasFlag(ParticleBehaviors.Velocity))
            {
                // use Mathf.lerp
                velocity.x -= ((velInitial.x - velFinal.x) / lifeInitial) * dt;
                velocity.y -= ((velInitial.y - velFinal.y) / lifeInitial) * dt;
            }

            if (sprite.isValid)
            {
                sprite.Update();
            }
            else if (colors.Length > 1)
            {
                // Changing Color
                currentColorTime -= dt;
                if (currentColorTime < 0)
                {
                    colorIndex += 1;
                    if (colorIndex >= colors.Length)
                        colorIndex = 0;
                    currentColor = colors[colorIndex];
                    currentColorTime = colorDuration;
                }
            }

            // Moving particles
            if (life > 0)
            {
                pos.x += velocity.x * dt;
                pos.y += velocity.y * dt;

                // SpritzUtil.Debug($"x:{pos.x} y:{pos.y} vx: {velocity.x} vy: {velocity.y} size:{size} life: {life} color: {colorIndex} currentColorTime:{currentColorTime}");
            }
            else
            {
                SpritzUtil.debugLogEnabled = false;
                Die();
            }
        }

        public void Draw()
        {
            if (sprite.isValid)
                sprite.Draw(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            else if (size <= 1)
                Spritz.DrawPixel(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), currentColor);
            else
            {
                var s = (int)(size);
                if (size > 0)
                    Spritz.DrawCircle(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), s, currentColor, true);
            }
        }

        public void Die()
        {
            dead = true;
        }

        public override string ToString()
        {
            return $"{pos} - {life}";
        }
    }

    [Flags]
    public enum EmitterSpawnOptions
    {
        None = 0,
        Gravity = 1 << 1,
        Burst = 1 << 2,
        RandomColor = 1 << 3,
        RandomSprite = 1 << 4,
        UseArea = 1 << 5,
    }

    public struct ValueSpan
    {
        public float min;
        public float max;
        public float spread => max - min;

        public static ValueSpan Range(float min, float max)
        {
            if (min < max)
                return new ValueSpan() { min = min, max = max };
            return new ValueSpan() { min = max, max = min };
        }

        public static ValueSpan Spread(float value, float spread)
        {
            if (spread < 0)
                return new ValueSpan() { min = value - spread, max = value };
            return new ValueSpan() { min = value, max = value + spread };
        }

        public static ValueSpan Value(float v)
        {
            return new ValueSpan() { min = v, max = v };
        }

        public float Random()
        {
            if (min != max)
                return UnityEngine.Random.Range(min, max);
            return min;
        }
    }

    public class Emitter
    {
        private List<Particle> m_Particles;
        private List<int> m_ToRemove;

        public EmitterSpawnOptions spawnOptions;

        public Vector2 pos;
        public bool emitting;
        public float frequency;
        public float emitTime;
        public int maxParticles;
        public int burstAmount;

        public Vector2 spawnArea;

        public Color[] pColors;
        public AnimSprite pSprite;

        public ValueSpan pLife;
        public ValueSpan pAngle;
        public ValueSpan pSpeedInitial;
        public ValueSpan pSpeedFinal;
        public ValueSpan pSizeInitial;
        public ValueSpan pSizeFinal;
        private ParticleBehaviors pParticleDefaultBehaviors;

        public IEnumerable<Particle> particles => m_Particles;

        public int numParticles => m_Particles.Count;

        public System.Action<Emitter> customUpdate;

        public Emitter(float x, float y, float frequency, int maxParticles)
        {
            this.maxParticles = maxParticles;

            m_Particles = new List<Particle>(maxParticles);
            m_ToRemove = new List<int>(maxParticles);

            pos = new Vector2(x, y);
            this.frequency = frequency;
            emitting = true;
            emitTime = 0;
            spawnOptions = EmitterSpawnOptions.None;

            pColors = new Color[] { Spritz.palette[1] };
            pLife = ValueSpan.Value(1);
            pAngle = ValueSpan.Spread(0, 360);
            pSpeedInitial = ValueSpan.Value(10);
            pSpeedFinal = ValueSpan.Value(10);
            pSizeInitial = ValueSpan.Value(1);
            pSizeFinal = ValueSpan.Value(1);
        }

        public void Update()
        {
            Emit(Time.deltaTime);

            customUpdate?.Invoke(this);

            for (var i = 0; i < m_Particles.Count; ++i)
            {
                var p = m_Particles[i];
                p.Update();
                m_Particles[i] = p;
                if (m_Particles[i].dead)
                    m_ToRemove.Add(i);
            }

            if (m_ToRemove.Count > 0)
            {
                for (var i = m_ToRemove.Count - 1; i >= 0; --i)
                {
                    m_Particles.RemoveAt(m_ToRemove[i]);
                }
                m_ToRemove.Clear();
            }
        }

        public void Draw()
        {
            for (var i = 0; i < m_Particles.Count; ++i)
            {
                m_Particles[i].Draw();
            }
        }

        public void Start()
        {
            emitting = true;
        }

        public void Stop()
        {
            emitting = false;
        }

        public void SetBurst(int burstAmount = 0)
        {
            spawnOptions |= EmitterSpawnOptions.Burst;
            this.burstAmount = burstAmount > 0 ? burstAmount : maxParticles;
        }

        public void SetSpawnArea(float width, float height)
        {
            spawnOptions |= EmitterSpawnOptions.UseArea;
            spawnArea = new Vector2(width, height);
        }

        public void SetLife(ValueSpan value)
        {
            pLife = value;
        }

        public void SetAngle(ValueSpan v)
        {
            pAngle = v;
        }

        public void SetSpeed(ValueSpan initial, ValueSpan final)
        {
            pSpeedInitial = initial;
            pSpeedFinal = final;
        }
        
        public void SetSize(ValueSpan sizeInitial, ValueSpan sizeFinal)
        {
            pSizeInitial = sizeInitial;
            pSizeFinal = sizeFinal;
        }

        public Emitter Clone()
        {
            var emitter = new Emitter(pos.x, pos.y, frequency, maxParticles)
            {
                spawnOptions = spawnOptions,
            };
            emitter.emitTime = emitTime;
            emitter.burstAmount = burstAmount;
            emitter.spawnArea= spawnArea;

            emitter.pColors = pColors;
            emitter.pSprite = pSprite;
            emitter.pLife = pLife;
            emitter.pAngle = pAngle;
            emitter.pSpeedInitial = pSpeedInitial;
            emitter.pSpeedFinal = pSpeedFinal;
            emitter.pSizeInitial = pSizeInitial;
            emitter.pSizeFinal = pSizeFinal;

            emitter.customUpdate = customUpdate;

            return emitter;
        }

        private Color[] GetParticleColors()
        {
            if (spawnOptions.HasFlag(EmitterSpawnOptions.RandomColor))
            {
                if (pColors != null && pColors.Length > 0)
                {
                    return new[] { pColors[UnityEngine.Random.Range(0, pColors.Length)] };
                }
                else
                {
                    return new[] { Spritz.palette[UnityEngine.Random.Range(0, Spritz.palette.Length)] };
                }
            }
            else if (pColors != null && pColors.Length > 0)
            {
                return pColors;
            }
            else
            {
                return new[] { Spritz.palette[1] };
            }
        }

        private Particle CreateParticle()
        {
            var x = pos.x;
            var y = pos.y;
            if (spawnOptions.HasFlag(EmitterSpawnOptions.UseArea))
            {
                x += UnityEngine.Random.Range(0, spawnArea.x) - spawnArea.x / 2;
                y += UnityEngine.Random.Range(0, spawnArea.y) - spawnArea.y / 2;
            }

            var p = new Particle();
            p.id = m_Particles.Count;
            p.Init(pParticleDefaultBehaviors, x, y, 
                    pLife.Random(), 
                    pAngle.Random(),
                    pSpeedInitial.Random(), pSpeedFinal.Random(),
                    pSizeInitial.Random(), pSizeFinal.Random());
            if (pSprite.isValid)
            {
                p.SetSprites(pSprite);
            }
            else
            {
                p.SetColors(GetParticleColors());
            }

            return p;
        }

        private void Emit(float dt)
        {
            if (!emitting)
                return;

            pParticleDefaultBehaviors = ParticleBehaviors.None;
            if (spawnOptions.HasFlag(EmitterSpawnOptions.Gravity))
                pParticleDefaultBehaviors |= ParticleBehaviors.Gravity;

            if (spawnOptions.HasFlag(EmitterSpawnOptions.Burst))
            {
                var nbNewParticles = GetNbParticleToSpawn(burstAmount);
                for (var i = 0; i < nbNewParticles; ++i)
                {
                    m_Particles.Add(CreateParticle());
                }
                emitting = false;
            }
            else
            {
                emitTime += frequency;
                if (emitTime >= 1)
                {
                    var nbNewParticles = GetNbParticleToSpawn(Mathf.FloorToInt(emitTime));
                    for (var i = 0; i < nbNewParticles; ++i)
                    {
                        m_Particles.Add(CreateParticle());
                    }
                    emitTime -= nbNewParticles;
                }
            }
        }

        private int GetNbParticleToSpawn(int spawnAmount)
        {
            if (maxParticles != 0 && m_Particles.Count + spawnAmount > maxParticles)
                return maxParticles - m_Particles.Count;
            return spawnAmount;
        }
    }
}
