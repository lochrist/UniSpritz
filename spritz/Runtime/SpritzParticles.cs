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
        Size = 1 << 2,
        Velocity = 1 << 3
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
        
        public Color32[] colors;
        public float colorDuration;
        public float currentColorTime;
        public int colorIndex;
        public Color32 currentColor;

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

            // TOD CHECK: why checking only x???
            if (!SpritzUtil.Approximately(velInitial.x, velFinal.x))
            {
                this.behaviors |= ParticleBehaviors.Velocity;
            }
        }

        public void SetColors(Color32[] c)
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
    public enum EmitterOptions
    {
        None = 0,
        Gravity = 1 << 1,
        Burst = 1 << 2,
        RandomColor = 1 << 3,
        RandomSprite = 1 << 4,
        SpawnArea = 1 << 5,
        ParticleBoundingBox = 1 << 6
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

        public EmitterOptions spawnOptions;

        public Vector2 pos;
        public bool emitting;
        public float frequency;
        public float emitTime;
        public int maxParticles;
        public int burstAmount;
        public Particle[] particleBurst;

        public Vector2 spawnArea;
        public Rect particlesBoundingBox;

        public Color32[] pColors;
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
            spawnOptions = EmitterOptions.None;

            pColors = new Color32[] { Spritz.palette[1] };
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
                UpdateParticle(ref p);
                m_Particles[i] = p;
                if (p.dead)
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
            spawnOptions |= EmitterOptions.Burst;
            this.burstAmount = burstAmount > 0 ? burstAmount : maxParticles;
        }

        public void SetParticlesBurst(Particle[] burst)
        {
            spawnOptions |= EmitterOptions.Burst;
            particleBurst = burst;
        }

        public void SetSpawnArea(float width, float height)
        {
            spawnOptions |= EmitterOptions.SpawnArea;
            spawnArea = new Vector2(width, height);
        }

        public void SetParticleBoundingBox(Rect bb)
        {
            spawnOptions |= EmitterOptions.ParticleBoundingBox;
            particlesBoundingBox = bb;
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

        private void UpdateParticle(ref Particle p)
        {
            var dt = Time.deltaTime;

            p.life -= dt;

            // TO CHECK: Each particle value could have its own function to process behavior.

            if (spawnOptions.HasFlag(EmitterOptions.Gravity))
            {
                p.velocity.y = p.velocity.y + Time.deltaTime * Particle.gGravity;
            }

            // Size over lifetime
            if (p.behaviors.HasFlag(ParticleBehaviors.Size))
            {
                var sizeChange = ((p.sizeInitial - p.sizeFinal) / p.lifeInitial) * dt;
                p.size -= sizeChange;
            }

            // Velocity over lifetime
            if (p.behaviors.HasFlag(ParticleBehaviors.Velocity))
            {
                // use Mathf.lerp
                p.velocity.x -= ((p.velInitial.x - p.velFinal.x) / p.lifeInitial) * dt;
                p.velocity.y -= ((p.velInitial.y - p.velFinal.y) / p.lifeInitial) * dt;
            }

            if (p.sprite.isValid)
            {
                p.sprite.Update();
            }
            else if (p.colors.Length > 1)
            {
                // Changing Color
                p.currentColorTime -= dt;
                if (p.currentColorTime < 0)
                {
                    p.colorIndex += 1;
                    if (p.colorIndex >= p.colors.Length)
                        p.colorIndex = 0;
                    p.currentColor = p.colors[p.colorIndex];
                    p.currentColorTime = p.colorDuration;
                }
            }

            // Moving particles
            if (p.life > 0)
            {
                p.pos.x += p.velocity.x * dt;
                p.pos.y += p.velocity.y * dt;

                if (spawnOptions.HasFlag(EmitterOptions.ParticleBoundingBox) && !particlesBoundingBox.Contains(p.pos))
                {
                    p.Die();
                }
                // SpritzUtil.Debug($"x:{pos.x} y:{pos.y} vx: {velocity.x} vy: {velocity.y} size:{size} life: {life} color: {colorIndex} currentColorTime:{currentColorTime}");
            }
            else
            {
                p.Die();
            }
        }

        private Color32[] GetParticleColors()
        {
            if (spawnOptions.HasFlag(EmitterOptions.RandomColor))
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
            if (spawnOptions.HasFlag(EmitterOptions.SpawnArea))
            {
                // Spawn area is centered around the emitter pos.
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
            if (spawnOptions.HasFlag(EmitterOptions.Burst))
            {
                if (particleBurst != null)
                {
                    var nbNewParticles = GetNbParticleToSpawn(particleBurst.Length);
                    for (var i = 0; i < nbNewParticles; ++i)
                    {
                        m_Particles.Add(particleBurst[i]);
                    }
                }
                else
                {
                    var nbNewParticles = GetNbParticleToSpawn(burstAmount);
                    for (var i = 0; i < nbNewParticles; ++i)
                    {
                        m_Particles.Add(CreateParticle());
                    }
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
