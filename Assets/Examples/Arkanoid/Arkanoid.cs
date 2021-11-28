using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniMini;

struct Bullet
{
    public Vector2Int pos;
    public int sp;
    public Vector2Int diff;
    public RectInt box => new RectInt(pos.x, pos.y, 3, 4);
}

struct Alien
{
    public int sp;
    public Vector2Int pos;
    public int d;
    public int r;
    public Vector2 m_pos;
    public RectInt box => new RectInt(pos.x, pos.y, 7, 7);
    public bool isActive;
}

struct Explosion
{
    public Vector2Int pos;
    public int t;
}

struct Star
{
    public Vector2Int pos;
    public int speed;
}

struct Ship
{
    public int sp;
    public Vector2Int pos;
    public int h;
    public int p;
    public int t;
    public bool imm;
    public RectInt box => new RectInt(pos.x, pos.y, 7, 7);
}

public class Arkanoid : SpritzGame
{
    private SpriteId m_Ship1;
    private SpriteId m_Ship2;
    private SpriteId m_Alien;
    private SpriteId m_Missile;
    private SpriteId m_HeartFull;
    private SpriteId m_HeartEmpty;

    private Ship m_Ship;
    private Star[] m_Stars;
    private Alien[] m_Aliens;
    private int m_AliensCount;
    private List<Bullet> m_Bullets;
    private List<Explosion> m_Explosions;
    private int m_Tick;
    private AudioClipId m_Sfx;
    private bool m_LastFrameFire;
    private bool m_GameOver;

    public override void InitializeSpritz()
    {
        Spritz.CreateLayer("RandomImages/arka");
        m_Ship1 = new SpriteId("ship_1");
        m_Ship2 = new SpriteId("ship_2");
        m_Alien = new SpriteId("alien");
        m_Missile = new SpriteId("missile");
        m_HeartFull = new SpriteId("heart_full");
        m_HeartEmpty = new SpriteId("heart_empty");

        m_Ship = new Ship()
        {
            sp = 1,
            pos = new Vector2Int(60, 100),
            h = 4,
            p = 0,
            t = 0,
            imm = false,
        };

        Spritz.LoadSoundbankFolder("Audio");
        m_Sfx = new AudioClipId("Shoot - Laser");

        m_Stars = new Star[128];
        for (var i = 0; i < 128; ++i)
            m_Stars[i] = new Star() { pos = new Vector2Int(Random.Range(0, 128), Random.Range(0, 128)), speed = Random.Range(0, 2) + 1 };
        m_Aliens = new Alien[15];
        m_AliensCount = 0;
        m_Tick = 0;

        m_Bullets = new List<Bullet>(20);
        m_Explosions = new List<Explosion>(5);
    }

    public override void UpdateSpritz()
    {
        if (m_GameOver)
            return;

        // TODO: keep tick in Spritz
        m_Tick++;
        if (m_Ship.imm)
        {
            m_Ship.t++;
            if (m_Ship.t > 30)
            {
                m_Ship.imm = false;
                m_Ship.t = 0;
            }
        }

        for (var i = 0; i < m_Stars.Length; ++i)
        {
            m_Stars[i].pos.y += m_Stars[i].speed;
            if (m_Stars[i].pos.y > 128)
            {
                m_Stars[i].pos.y = 0;
                m_Stars[i].pos.x = Random.Range(0, 128);
            }
        }

        if (m_AliensCount == 0)
        {
            Respawn();
        }

        for (var i = 0; i < m_Explosions.Count;)
        {
            var ex = m_Explosions[i];
            ex.t += 1;
            m_Explosions[i] = ex;
            if (ex.t == 13)
                m_Explosions.RemoveAt(i);
            else
                ++i;
        }

        for (var i = 0; i < m_Aliens.Length; ++i)
        {
            if (!m_Aliens[i].isActive)
                continue;
            m_Aliens[i].m_pos.y += 1.3f;
            m_Aliens[i].pos.x = (int)(m_Aliens[i].r * Mathf.Sin(m_Aliens[i].d * m_Tick / 50f) + m_Aliens[i].m_pos.x);
            m_Aliens[i].pos.y = (int)(m_Aliens[i].r * Mathf.Cos(m_Tick / 50f) + m_Aliens[i].m_pos.y);

            if (Collision(m_Ship.box, m_Aliens[i].box) && !m_Ship.imm)
            {
                m_Ship.imm = true;
                m_Ship.h -= 1;
                if (m_Ship.h <= 0)
                {
                    GameOver();
                }
            }

            if (m_Aliens[i].pos.y > 150)
            {
                m_Aliens[i].isActive = false;
                m_AliensCount--;
            }
        }

        for (var i = 0; i < m_Bullets.Count;)
        {
            var bullet = m_Bullets[i];
            bullet.pos += bullet.diff;
            if (bullet.pos.x < 0 || bullet.pos.x > 128 ||
                bullet.pos.y < 0 || bullet.pos.y > 128)
            {
                // To remove bullet;
                m_Bullets.RemoveAt(i);
            }
            else
            {
                m_Bullets[i] = bullet;
                ++i;
            }
            for (var j = 0; j < m_Aliens.Length; ++j)
            {
                if (!m_Aliens[j].isActive)
                    continue;

                if (Collision(bullet.box, m_Aliens[j].box))
                {
                    m_Aliens[j].isActive = false;
                    m_AliensCount--;
                    m_Ship.p += 1;
                    Explode(m_Aliens[j].pos);

                }
            }
        }

        if (m_Tick % 6 < 3)
            m_Ship.sp = 1;
        else
            m_Ship.sp = 2;

        if (Input.GetKey(KeyCode.W))
        {
            m_Ship.pos.y -= 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            m_Ship.pos.x -= 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_Ship.pos.y += 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            m_Ship.pos.x += 1;
        }

        // TODO: handle GetDown type of input directly in Spritz using KeyCode.
        var isFire = Input.GetKey(KeyCode.Return);
        if (isFire && !m_LastFrameFire)
        {
            Fire();
        }
        m_LastFrameFire = isFire;
    }

    private void GameOver()
    {
        m_GameOver = true;
    }


    private void Explode(Vector2Int pos)
    {
        m_Explosions.Add(new Explosion()
        {
            pos = pos
        });
    }

    private bool Collision(RectInt a, RectInt b)
    {
        return a.Overlaps(b);
    }

    private void Fire()
    {
        var bullet = new Bullet()
        {
            sp = 3,
            pos = new Vector2Int(m_Ship.pos.x + 2, m_Ship.pos.y),
            diff = new Vector2Int(0, -3)
        };
        m_Bullets.Add(bullet);
        Spritz.PlaySound(m_Sfx, 0.5f, false);
    }

    private void Respawn()
    {
        m_AliensCount = Random.Range(0, 9) + 2;
        for(var i = 0; i < m_AliensCount; ++i)
        {
            var d = -1;
            if (Random.Range(0f, 1f) < 0.5f)
            {
                d = 1;
            }
            var alien = new Alien()
            {
                sp = 17,
                m_pos = new Vector2Int(i * 16, 20 - i * 8),
                d = d,
                pos = new Vector2Int(-32, -32),
                r = 12,
                isActive = true
            };
            m_Aliens[i] = alien;
        }
    }

    private void TestDrawSprites()
    {
        Spritz.DrawSprite(m_Ship1, 1, 1);
        Spritz.DrawSprite(m_Ship2, 1, 32);
        Spritz.DrawSprite(m_Alien, 32, 1);
        Spritz.DrawSprite(m_Missile, 32, 32);
        Spritz.DrawSprite(m_HeartFull, 64, 1);
        Spritz.DrawSprite(m_HeartEmpty, 64, 32);
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);
        if (m_GameOver)
        {
            Spritz.Print("Game Over", 50, 50, Color.red);
            return;
        }


        for (var i = 0; i < m_Stars.Length; ++i)
        {
            Spritz.DrawPixel(m_Stars[i].pos.x, m_Stars[i].pos.y, Color.white);
        }

        Spritz.Print($"{m_Ship.p}", 5, 5, Color.red);
        if (!m_Ship.imm || m_Tick%8 < 4)
        {
            Spritz.DrawSprite(m_Ship.sp == 1 ? m_Ship1 : m_Ship2, m_Ship.pos.x, m_Ship.pos.y);
        }

        for (var i = 0; i < m_Explosions.Count; ++i)
        {
            Spritz.DrawCircle(m_Explosions[i].pos.x, m_Explosions[i].pos.y, m_Explosions[i].t / 2, Color.red, false);
        }

        for (var i = 0; i < m_AliensCount; ++i)
        {
            if (m_Aliens[i].isActive)
                Spritz.DrawSprite(m_Alien, m_Aliens[i].pos.x, m_Aliens[i].pos.y);
        }

        for (var i = 0; i < m_Bullets.Count; ++i)
        {
            Spritz.DrawSprite(m_Missile, m_Bullets[i].pos.x, m_Bullets[i].pos.y);
        }

        for (var i = 0; i < 4; ++i)
        {
            if (i < m_Ship.h)
                Spritz.DrawSprite(m_HeartFull, 80+6*i, 3);
            else
                Spritz.DrawSprite(m_HeartEmpty, 80 + 6 * i, 3);
        }
    }
}
