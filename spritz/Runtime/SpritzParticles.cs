using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    [Flags]
    enum ParticleBehaviors
    {
        Gravity = 1 << 1,
        Size = 1 << 2,
        Velocity = 1 << 3,
        Color = 1 << 4,
        LifeBox = 1 << 5
    }

    public struct Particle
    {
        public static float gGravity = 50;

        public int id;
        public Vector2 pos;
        public float lifeInitial;
        public float life;

        public Vector2 velocity;
        public Vector2 velInitial;
        public Vector2 velFinal;

        public bool dead;
        public bool gravityAffected;

        public float size;
        public float sizeInitial;
        public float sizeFinal;

        public AnimSprite sprite;
        
        public Color[] colors;
        public float colorDuration;
        public float currentColorTime;
        public int colorIndex;
        public Color currentColor;

        public void Set(float x, float y, bool gravityAffected, float life, float angle, float initialSpeed, float finalSpeed, float initialSize, float finalSize, Color[] c)
        {
            Set(x, y, gravityAffected, life, angle, initialSpeed, finalSpeed, initialSize, finalSize);
            sprite.fps = 0;

            colors = c;
            colorDuration = 1f / colors.Length * life;
            currentColorTime = colorDuration;
            colorIndex = 0;
            currentColor = colors[colorIndex];

            // SpritzUtil.Debug($"Set x:{pos.x} y:{pos.y} vx: {velocity.x} vy: {velocity.y} vxf: {velFinal.x} vyf: {velFinal.y} size:{size} life: {life} color: {colorIndex} colorDuration: {colorDuration} colorTime: {currentColorTime}");
        }

        public void Set(float x, float y, bool gravityAffected, float life, float angle, float initialSpeed, float finalSpeed, float initialSize, float finalSize, AnimSprite s)
        {
            Set(x, y, gravityAffected, life, angle, initialSpeed, finalSpeed, initialSize, finalSize);

            sprite = s;
            sprite.SetFps(sprite.frames.Length / life);
        }

        private void Set(float x, float y, bool gravityAffected, float life, float angle, float initialSpeed, float finalSpeed, float initialSize, float finalSize)
        {
            pos = new Vector2(x, y);
            lifeInitial = this.life = life;

            var angleRadians = Mathf.Deg2Rad * angle;

            // TO check Why -Mathf.sin???
            velocity = new Vector2(initialSpeed * Mathf.Cos(angleRadians), initialSpeed * -Mathf.Sin(angleRadians));
            velInitial = velocity;
            velFinal = new Vector2(finalSpeed * Mathf.Cos(angleRadians), finalSpeed * -Mathf.Sin(angleRadians));

            dead = false;
            this.gravityAffected = gravityAffected;

            this.size = sizeInitial = initialSize;
            sizeFinal = finalSize;
        }

        public void Update()
        {
            var dt = Time.deltaTime;

            life -= dt;

            // TO check Should Update of Particles be computed in Emitter with a different function/behavior?
            if (gravityAffected)
            {
                velocity.y = velocity.y + Time.deltaTime * gGravity;
            }

            // TO CHECK: Could be computed once.
            // Size over lifetime
            if (!SpritzUtil.Approximately(sizeInitial, sizeFinal))
            {
                var sizeChange = ((sizeInitial - sizeFinal) / lifeInitial) * dt;
                size -= sizeChange;
            }

            // Velocity over lifetime

            // TO CHECK: Could be computed once.
            // why checking only x???
            if (!SpritzUtil.Approximately(velInitial.x, velFinal.x))
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
    enum EmitterSpawnOptions
    {
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

        public ValueSpan Range(float min, float max)
        {
            if (min < max)
                return new ValueSpan() { min = min, max = max };
            return new ValueSpan() { min = max, max = min };
        }

        public ValueSpan Spread(float value, float spread)
        {
            if (spread < 0)
                return new ValueSpan() { min = value - spread, max = value };
            return new ValueSpan() { min = value, max = value + spread };
        }

        public ValueSpan Value(float v)
        {
            return new ValueSpan() { min = v, max = v };
        }

        public float Random()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }

    public class Emitter
    {
        private List<Particle> m_Particles;
        private List<int> m_ToRemove;

        public Vector2 pos;
        public bool emitting;
        public float frequency;
        public float emitTime;
        public int maxParticles;

        // Flags?
        public bool gravityAffected;
        public bool burst;
        public bool rndColor;
        public bool rndSprite;
        public bool useArea;

        public int burstAmount;

        // Vector2
        public Vector2 spawnArea;

        public Color[] pColors;
        public AnimSprite pSprite;

        // Span
        public float pLife;
        public float pLifeSpread;

        // Span
        public float pAngle;
        public float pAngleSpread;

        // Span
        public float pSpeedInitial;
        public float pSpeedSpreadInitial;

        // Span
        public float pSpeedFinal;
        public float pSpeedSpreadFinal;

        // Span
        public float pSizeInitial;
        public float pSizeFinal;

        // Span
        public float pSizeSpreadInitial;
        public float pSizeSpreadFinal;

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
            rndColor = false;
            rndSprite = false;
            useArea = false;

            pColors = new Color[] { Spritz.palette[1] };
            pLife = 1;
            pLifeSpread = 0;
            pAngle = 0;
            pAngleSpread = 360;
            pSpeedInitial = 10;
            pSpeedFinal = 10;
            pSpeedSpreadInitial = 0;
            pSpeedSpreadFinal = 0;
            pSizeInitial = 1;
            pSizeFinal = 1;
            pSizeSpreadInitial = 0;
            pSizeSpreadFinal = 0;
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

        public void SetBurst(bool burst, int burstAmount = 0)
        {
            this.burst = burst;
            this.burstAmount = burstAmount > 0 ? burstAmount : maxParticles;
        }

        public void SetSpawnArea(float width, float height)
        {
            useArea = width > 0 || height > 0;
            spawnArea = new Vector2(width, height);
        }

        public void SetLife(float life, float lifeSpread = float.NaN)
        {
            pLife = life;
            pLifeSpread = float.IsNaN(lifeSpread) ? 0 : lifeSpread;
        }

        public void SetAngle(float angle, float angleSpread = float.NaN)
        {
            pAngle = angle;
            pAngleSpread = float.IsNaN(angleSpread) ? 0 : angleSpread;
        }

        public void SetSpeed(float speedInitial, float speedFinal = float.NaN, float speedSpreadInitial = float.NaN, float speedSpreadFinal = float.NaN)
        {
            pSpeedInitial = speedInitial;
            pSpeedFinal = float.IsNaN(speedFinal) ? speedInitial : speedFinal;
            pSpeedSpreadInitial = float.IsNaN(speedSpreadInitial) ? 0 : speedSpreadInitial;
            pSpeedSpreadFinal = float.IsNaN(speedSpreadFinal) ? pSpeedSpreadInitial : speedSpreadFinal;
        }

        public void SetSize(float sizeInitial, float sizeFinal = float.NaN, float sizeSpreadInitial = float.NaN, float sizeSpreadFinal = float.NaN)
        {
            pSizeInitial = sizeInitial;
            pSizeFinal = float.IsNaN(sizeFinal) ? sizeInitial : sizeFinal;
            pSizeSpreadInitial = float.IsNaN(sizeSpreadInitial) ? 0 : sizeSpreadInitial;
            pSizeSpreadFinal = float.IsNaN(sizeSpreadFinal) ? pSizeSpreadInitial : sizeSpreadFinal;
        }

        public Emitter Clone()
        {
            var emitter = new Emitter(pos.x, pos.y, frequency, maxParticles);
            emitter.emitTime = emitTime;
            emitter.gravityAffected = gravityAffected;
            emitter.burst = burst;
            emitter.burstAmount = burstAmount;

            emitter.rndColor = rndColor;
            emitter.rndSprite = rndSprite;
            emitter.useArea = useArea;
            emitter.spawnArea= spawnArea;

            emitter.pColors = pColors;
            emitter.pSprite = pSprite;
            emitter.pLife = pLife;
            emitter.pLifeSpread = pLifeSpread;
            emitter.pAngle = pAngle;
            emitter.pAngleSpread = pAngleSpread;
            emitter.pSpeedInitial = pSpeedInitial;
            emitter.pSpeedFinal = pSpeedFinal;
            emitter.pSpeedSpreadInitial = pSpeedSpreadInitial;
            emitter.pSpeedSpreadFinal = pSpeedSpreadFinal;
            emitter.pSizeInitial = pSizeInitial;
            emitter.pSizeFinal = pSizeFinal;
            emitter.pSizeSpreadInitial = pSizeSpreadInitial;
            emitter.pSizeSpreadFinal = pSizeSpreadFinal;

            emitter.customUpdate = customUpdate;

            return emitter;
        }

        private Color[] GetParticleColors()
        {
            if (rndColor)
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
            if (useArea)
            {
                x += UnityEngine.Random.Range(0, spawnArea.x) - spawnArea.x / 2;
                y += UnityEngine.Random.Range(0, spawnArea.y) - spawnArea.y / 2;
            }

            // Check about pooling.

            var p = new Particle();
            p.id = m_Particles.Count;
            if (pSprite.isValid)
            {
                p.Set(x, y, gravityAffected, pLife + UnityEngine.Random.Range(0, pLifeSpread), pAngle + UnityEngine.Random.Range(0, pAngleSpread),
                    pSpeedInitial + UnityEngine.Random.Range(0, pSpeedSpreadInitial), pSpeedFinal + UnityEngine.Random.Range(0, pSpeedSpreadFinal),
                    (pSizeInitial + UnityEngine.Random.Range(0, pSizeSpreadInitial)), (pSizeFinal + UnityEngine.Random.Range(0, pSizeSpreadFinal)),
                    pSprite
                    );
            }
            else
            {
                p.Set(x, y, gravityAffected, pLife + UnityEngine.Random.Range(0, pLifeSpread), pAngle + UnityEngine.Random.Range(0, pAngleSpread),
                    pSpeedInitial + UnityEngine.Random.Range(0, pSpeedSpreadInitial), pSpeedFinal + UnityEngine.Random.Range(0, pSpeedSpreadFinal),
                    (pSizeInitial + UnityEngine.Random.Range(0, pSizeSpreadInitial)), (pSizeFinal + UnityEngine.Random.Range(0, pSizeSpreadFinal)),
                    GetParticleColors()
                    );
            }

            return p;
        }

        private void Emit(float dt)
        {
            if (!emitting)
                return;

            if (burst)
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
