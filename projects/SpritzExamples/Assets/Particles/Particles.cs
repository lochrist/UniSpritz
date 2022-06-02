using UniMini;
using UnityEngine;
using System.Collections.Generic;

public class Particles : SpritzGame
{
    enum EmitterExamples
    {
        Fire,
        Smoke,
        WaterSpout,
        Rain,
        Stars,
        ExplosionBurst,
        ConfettiBurst,
        SpaceWarp,
        Amoebas,
        Portal,
        WhirlyBird,
        Firecracker,
        SpiralGalaxyMonster,
        StructuresMouse,
        StructuresArrows
    }

    bool m_ShowInfo;
    List<Emitter> m_Emitters;
    EmitterExamples m_CurrentEmitter;
    UniMini.SpriteId[] m_AllSprites;

    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/particle-system");

        m_AllSprites = Spritz.GetSprites();

        m_Emitters = new List<Emitter>();
        m_ShowInfo = true;
        m_CurrentEmitter = EmitterExamples.Smoke;

        InitEmitter(m_CurrentEmitter);
    }

    public override void UpdateSpritz()
    {
        if (Spritz.GetKeyDown(KeyCode.Escape))
        {
            // Hide/show info
            m_ShowInfo = !m_ShowInfo;
        }

        if (Spritz.GetKeyDown(KeyCode.P))
        {
            PrintAllParticles("pr", m_Emitters[0]);
        }

        // More emitters
        var x = 0;
        var y = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            x -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            x += 1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            y -= 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            y += 1;
        }

        if (x != 0 || y != 0)
        {
            foreach (var e in m_Emitters)
            {
                e.pos.x += x;
                e.pos.y += y;
            }
        }

        if (Spritz.GetKeyDown(KeyCode.Z))
        {
            // Start stop emitters
            if (m_Emitters[0].emitting)
            {
                StopEmitters();
            }
            else
            {
                StartEmitters();
            }
        }

        if (Spritz.GetKeyDown(KeyCode.A))
        {
            // previous examples
            if (m_CurrentEmitter == EmitterExamples.Fire)
                m_CurrentEmitter = EmitterExamples.StructuresArrows;
            else
            {
                m_CurrentEmitter = (EmitterExamples)((int)m_CurrentEmitter - 1);
            }
            InitEmitter(m_CurrentEmitter);
        }
        if (Spritz.GetKeyDown(KeyCode.D))
        {
            // next examples
            if (m_CurrentEmitter == EmitterExamples.StructuresArrows)
                m_CurrentEmitter = EmitterExamples.Fire;
            else
            {
                m_CurrentEmitter = (EmitterExamples)((int)m_CurrentEmitter + 1);
            }

            InitEmitter(m_CurrentEmitter);
        }
        if (Spritz.GetKeyDown(KeyCode.X))
        {
            // Spawn emitter
            AddEmitter(m_CurrentEmitter);
        }

        foreach (var e in m_Emitters)
            e.Update();
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);
        DrawParticles();
        // DrawDebug();
    }

    private void DrawDebug()
    {
        var sprites = GetSprites(22, 23, 25, 27, 28, 29);

        for (var i = 0; i < sprites.Length; ++i)
        {
            Spritz.DrawSprite(sprites[i], i * 24, 10);
        }
    }

    private void DrawParticles()
    {
        // Draw stuff:
        Spritz.DrawRectangle(0, 0, 128, 6, Spritz.palette[1], true);
        Spritz.DrawLine(0, 7, 128, 7, Spritz.palette[2]);
        Spritz.Print(m_CurrentEmitter.ToString(), 1, 1, Spritz.palette[7]);

        if (m_ShowInfo)
        {
            Spritz.Print($"Em: {m_Emitters.Count} Parts: {NumParticles()}", 0, 110, Spritz.palette[7]);
            SpritzUtil.DrawTimeInfo(0, 120);
        }

        foreach (var e in m_Emitters)
            e.Draw();
    }

    private static bool IsSpriteExists(SpriteId id)
    {
        var allSprites = Spritz.GetSprites();
        return System.Array.FindIndex(allSprites, s => s == id) != -1;
    }

    private static AnimSprite GetAnimSpriteFromRange(int startIndex, int endIndex)
    {
        List<SpriteId> sprites = new List<SpriteId>();
        for (var i = startIndex; i < endIndex + 1; ++i)
        {
            var sprName = $"particle-system_{i}";
            var id = new SpriteId(sprName);
            if (!IsSpriteExists(id))
            {
                Debug.LogError($"Sprite doesn't exist {sprName}");
            }
            sprites.Add(id);
        }

        return new AnimSprite(4, sprites.ToArray());
    }

    private static SpriteId[] GetSprites(params int[] frames)
    {
        List<SpriteId> sprites = new List<SpriteId>();
        for (var i = 0; i < frames.Length; ++i)
        {
            var sprName = $"particle-system_{frames[i]}";
            var id = new SpriteId(sprName);
            if (!IsSpriteExists(id))
            {
                Debug.LogError($"Sprite doesn't exist {sprName}");
            }
            sprites.Add(id);
        }
        return sprites.ToArray();
    }

    private static AnimSprite GetAnimSprite(params int[] frames)
    {
        var sprites = GetSprites(frames);
        return new AnimSprite(4, sprites);
    }

    private static Color[] GetColorsFromRange(int startIndex, int endIndex)
    {
        List<Color> colors = new List<Color>();
        for (var i = startIndex; i <= endIndex; ++i)
            colors.Add(Spritz.palette[i]);
        return colors.ToArray();
    }

    private static Color[] GetColors(params int[] colorIndexes)
    {
        List<Color> colors = new List<Color>();
        for (var i = 0; i < colorIndexes.Length; ++i)
            colors.Add(Spritz.palette[colorIndexes[i]]);
        return colors.ToArray();
    }

    private void StartEmitters()
    {
        foreach (var e in m_Emitters)
            e.Start();
    }

    private void StopEmitters()
    {
        foreach (var e in m_Emitters)
            e.Stop();
    }

    private void InitEmitter(EmitterExamples etype)
    {
        StopEmitters();
        m_Emitters.Clear();
        AddEmitter(etype);
    }

    private int NumParticles()
    {
        var i = 0;
        foreach (var e in m_Emitters)
            i += e.numParticles;
        return i;
    }

    static void PrintAllParticles(string title, Emitter e)
    {
        var oldLog = SpritzUtil.debugLogEnabled;
        SpritzUtil.debugLogEnabled = true;
        foreach (var p in e.particles)
            PrintParticle(title, p);

        SpritzUtil.debugLogEnabled = oldLog;
    }

    static string FormatParticle(string title, Particle p)
    {
        // return $"{title} id{p.id} x:{p.pos.x} y:{p.pos.y} vx:{p.velocity.x} vy:{p.velocity.y} sz:{p.size} life:{p.life}, col:{p.colorIndex} ctime:{p.currentColorTime}";
        return $"{title} id{p.id} x:{p.pos.x} y:{p.pos.y} sz:{p.size} life:{p.life} sprIndex: {p.sprite.spriteIndex}";
    }

    static void PrintParticle(string title, Particle p)
    {
        SpritzUtil.Debug(FormatParticle(title, p));
    }

    private void AddEmitter(EmitterExamples etype)
    {
        switch(etype)
        {
            case EmitterExamples.Fire:
            {
                    var main = new Emitter(64, 64, 4, 110);
                    main.pColors = new Color[] { Spritz.palette[8], Spritz.palette[9], Spritz.palette[10], Spritz.palette[5] };
                    main.SetArea(5, 0);
                    main.SetSpeed(15, 5, 20, 20);
                    main.SetLife(0.5f, 1);
                    main.SetAngle(90, 10);
                    main.SetSize(1.5f, 0, 2, 0);
                    var flicker = 1.1f;
                    main.customUpdate = e =>
                    {
                        e.SetAngle(90 + (SpritzUtil.Sinp8(Time.time * flicker) * 27), e.pAngleSpread);
                    };
                    m_Emitters.Add(main);
                    break;
            }
            case EmitterExamples.Smoke:
                {
                    var main = new Emitter(64, 64, 4, 110);
                    main.pColors = new Color[] { Spritz.palette[5], Spritz.palette[6] };
                    main.SetArea(5, 0);
                    main.SetSpeed(8, 5, 10, 5);
                    main.SetLife(0.5f, 4);
                    main.SetAngle(80, 50);
                    m_Emitters.Add(main);
                    break;
                }
            case EmitterExamples.WaterSpout:
                {
                    var main = new Emitter(110, 90, 1, 0);
                    main.SetBurst(false);
                    main.gravityAffected = true;
                    main.pColors = new Color[] { Spritz.palette[12], Spritz.palette[1] };
                    main.SetSize(2, 0, 5);
                    main.SetAngle(90, 45);
                    main.SetLife(2, 2);
                    main.SetSpeed(100, 50);
                    m_Emitters.Add(main);

                    var spray = new Emitter(110, 90, 1, 0);
                    spray.SetBurst(false);
                    spray.gravityAffected = true;
                    spray.pColors = new Color[] { Spritz.palette[7], Spritz.palette[6], Spritz.palette[5] };
                    spray.SetSize(0, 1);
                    spray.SetAngle(90, 45);
                    spray.SetLife(2, 2);
                    spray.SetSpeed(100, 50);
                    m_Emitters.Add(spray);

                    var explo = new Emitter(110, 90, 0.1f, 0);
                    explo.pColors = new Color[] { Spritz.palette[7], Spritz.palette[6], Spritz.palette[5] };
                    explo.SetArea(20, 20);
                    explo.SetSize(2, 0, 2, 0);
                    explo.SetAngle(90, 45);
                    explo.SetLife(1, 1);
                    explo.SetSpeed(10, 10, 10);
                    m_Emitters.Add(explo);

                    break;
                }
            case EmitterExamples.Rain:
                {
                    var main = new Emitter(64, 12, 2, 200);
                    main.pSprite = GetAnimSpriteFromRange(91, 97);
                    main.SetArea(50, 10);
                    main.gravityAffected = true;
                    main.SetSpeed(0);
                    main.SetLife(1.5f, 1);
                    main.SetAngle(90, 10);
                    main.SetSize(1);
                    m_Emitters.Add(main);
                    break;
                }
            case EmitterExamples.Stars:
                {
                    var front = new Emitter(0, 64, 0.2f, 0);
                    front.SetArea(0, 128);
                    front.pColors = new Color[] { Spritz.palette[7] };
                    front.SetSize(1);
                    front.SetAngle(0, 0);
                    front.SetLife(3.5f);
                    front.SetSpeed(34, 34, 10);
                    m_Emitters.Add(front);

                    var midfront = front.Clone();
                    midfront.frequency = 0.15f;
                    midfront.SetLife(4.5f);
                    midfront.pColors = new Color[] { Spritz.palette[6] };
                    midfront.SetSpeed(26, 26, 5);
                    m_Emitters.Add(midfront);

                    var midback = front.Clone();
                    midback.SetLife(6.8f);
                    midback.pColors = new Color[] { Spritz.palette[5] };
                    midback.SetSpeed(18, 18, 5);
                    midback.frequency = 0.1f;
                    m_Emitters.Add(midback);
                    
                    var back = front.Clone();
                    back.frequency = 0.07f;
                    back.SetLife(11);
                    back.pColors = new Color[] { Spritz.palette[1] };
                    back.SetSpeed(10, 10, 5);
                    m_Emitters.Add(back);

                    var special = new Emitter(64, 64, 0.2f, 0);
                    special.pSprite = GetAnimSpriteFromRange(78, 84);
                    special.SetArea(128, 128);
                    special.SetAngle(0, 0);
                    special.frequency = 0.01f;
                    special.SetSpeed(30, 30, 15);
                    special.SetLife(1);
                    m_Emitters.Add(special);

                    break;
                }
            case EmitterExamples.ExplosionBurst:
                {
                    var explo = new Emitter(64, 64, 0, 30);
                    explo.SetSize(4, 0, 3, 0);
                    explo.SetSpeed(0);
                    explo.SetLife(1);
                    explo.pColors = new Color[] { Spritz.palette[7], Spritz.palette[6], Spritz.palette[5] };
                    explo.SetArea(30, 30);
                    explo.SetBurst(true, 10);
                    m_Emitters.Add(explo);

                    var spray = new Emitter(64, 64, 0, 80);
                    spray.SetSize(1);
                    spray.SetSpeed(20, 10, 20, 10);
                    spray.SetLife(0, 1.3f);
                    spray.pColors = new Color[] { Spritz.palette[7], Spritz.palette[6], Spritz.palette[5] };
                    spray.SetBurst(true, 30);
                    m_Emitters.Add(spray);

                    var anim = new Emitter(64, 64, 0, 18);
                    anim.SetSpeed(0);
                    anim.SetLife(1);
                    anim.SetBurst(true, 6);
                    anim.SetArea(30, 30);
                    anim.pSprite = GetAnimSprite(32, 33, 34, 35, 36, 37, 38, 39, 40, 40, 40, 41, 41, 41);
                    m_Emitters.Add(anim);

                    break;
                }
            case EmitterExamples.ConfettiBurst:
                {
                    var left = new Emitter(0, 90, 0, 50);
                    left.gravityAffected = true;
                    left.SetBurst(true);
                    left.SetSize(0, 0, 2);
                    left.SetSpeed(50, 50, 50);
                    left.SetLife(1f, 2f);
                    left.SetAngle(30, 45);
                    left.rndColor = true;
                    left.pColors = GetColorsFromRange(7, 15);
                    m_Emitters.Add(left);

                    var right = left.Clone();
                    right.pos = new Vector2(128, 90);
                    right.SetAngle(105, 45);
                    m_Emitters.Add(right);

                    break;
                }
            case EmitterExamples.SpaceWarp:
                {
                    var warp = new Emitter(70, 70, 11, 520);
                    warp.SetSpeed(30, 200);
                    warp.SetLife(0.8f);
                    warp.SetSize(0, 2, 0.5f, 0);
                    warp.pColors = GetColors(7, 8, 11, 12, 14);
                    warp.rndColor = true;
                    warp.customUpdate = e =>
                    {

                    };

                    m_Emitters.Add(warp);
                    break;
                }
            case EmitterExamples.Firecracker:
                {
                    var warp = new Emitter(70, 70, 11, 520);
                    warp.SetBurst(true, 520);
                    warp.SetSpeed(10, 1, 10, 1);
                    warp.SetLife(2.8f, 1);
                    warp.SetSize(1);
                    warp.pColors = GetColors(7, 8, 11, 12, 14);
                    warp.rndColor = true;
                    warp.customUpdate = e =>
                    {

                    };

                    m_Emitters.Add(warp);
                    break;
                }
            case EmitterExamples.Amoebas:
                {
                    var grav = new Emitter(84, 64, 0.3f, 60);
                    grav.SetSpeed(50, -50, 50, -50);
                    grav.SetLife(1, 1.15f);
                    grav.pSprite = GetAnimSprite(75, 76, 77, 72, 71, 72, 73, 74);
                    grav.SetArea(20, 110);
                    grav.SetAngle(180);

                    m_Emitters.Add(grav);
                    break;
                }
            case EmitterExamples.Portal:
                {
                    break;
                }
            case EmitterExamples.WhirlyBird:
                {
                    var bird = new Emitter(80, 80, 1, 0);
                    bird.pSprite = GetAnimSprite(22, 23, 25, 27, 28, 29);
                    bird.SetLife(3);
                    bird.SetAngle(0);
                    bird.customUpdate = e =>
                    {

                    };

                    m_Emitters.Add(bird);
                    break;
                }
            case EmitterExamples.SpiralGalaxyMonster:
                {
                    break;
                }
            case EmitterExamples.StructuresMouse:
                {
                    break;
                }
            case EmitterExamples.StructuresArrows:
                {
                    break;
                }
        }
    }
}
