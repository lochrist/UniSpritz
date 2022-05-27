using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
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

            // TO check -> Pico 8 angle?
            // var angleRadians = angle * 3.14159f / 1131f;
            // Why -Mathf.sin???
            velocity = new Vector2(initialSpeed * Mathf.Cos(angleRadians), initialSpeed * -Mathf.Sin(angleRadians));
            // velocity = new Vector2(initialSpeed * SpritzUtil.Cosp8(angleRadians), initialSpeed * SpritzUtil.Sinp8(angleRadians));
            velInitial = velocity;
            // velFinal = new Vector2(finalSpeed * SpritzUtil.Cosp8(angleRadians), finalSpeed * SpritzUtil.Sinp8(angleRadians));
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

            if (gravityAffected)
            {
                velocity.y = velocity.y + Time.deltaTime * gGravity;
            }

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

    public class Emitter
    {
        private List<Particle> m_Particles;
        private List<int> m_ToRemove;

        public Vector2 pos;
        public bool emitting;
        public float frequency;
        public float emitTime;
        public int maxParticles;
        public bool gravityAffected;
        public bool burst;
        public int burstAmount;
        public bool pooling;
        // pool?
        public bool rndColor;
        public bool rndSprite;
        public bool useArea;
        public float areaWidth;
        public float areaHeight;

        public Color[] pColors;
        public AnimSprite pSprite;
        public float pLife;
        public float pLifeSpread;
        public float pAngle;
        public float pAngleSpread;
        public float pSpeedInitial;
        public float pSpeedFinal;
        public float pSpeedSpreadInitial;
        public float pSpeedSpreadFinal;
        public float pSizeInitial;
        public float pSizeFinal;
        public float pSizeSpreadInitial;
        public float pSizeSpreadFinal;

        public float flicker;

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
            pooling = false;
            rndColor = false;
            rndSprite = false;
            useArea = false;
            areaWidth = 0;
            areaHeight = 0;

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

        public void SetArea(float width, float height)
        {
            useArea = width > 0 || height > 0;
            areaWidth = width;
            areaHeight = height;
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
            emitter.pooling = pooling;

            emitter.rndColor = rndColor;
            emitter.rndSprite = rndSprite;
            emitter.useArea = useArea;
            emitter.areaWidth= areaWidth;
            emitter.areaHeight = areaHeight;

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
                x += UnityEngine.Random.Range(0, areaWidth) - areaWidth / 2;
                y += UnityEngine.Random.Range(0, areaHeight) - areaHeight / 2;
            }

            // Check about pooling.

            var p = new Particle();
            p.id = m_Particles.Count;
            if (pSprite.isValid)
            {
                p.Set(x, y, gravityAffected, pLife + Random.Range(0, pLifeSpread), pAngle + Random.Range(0, pAngleSpread),
                    pSpeedInitial + Random.Range(0, pSpeedSpreadInitial), pSpeedFinal + Random.Range(0, pSpeedSpreadFinal),
                    (pSizeInitial + Random.Range(0, pSizeSpreadInitial)), (pSizeFinal + Random.Range(0, pSizeSpreadFinal)),
                    pSprite
                    );
            }
            else
            {
                p.Set(x, y, gravityAffected, pLife + Random.Range(0, pLifeSpread), pAngle + Random.Range(0, pAngleSpread),
                    pSpeedInitial + Random.Range(0, pSpeedSpreadInitial), pSpeedFinal + Random.Range(0, pSpeedSpreadFinal),
                    (pSizeInitial + Random.Range(0, pSizeSpreadInitial)), (pSizeFinal + Random.Range(0, pSizeSpreadFinal)),
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
