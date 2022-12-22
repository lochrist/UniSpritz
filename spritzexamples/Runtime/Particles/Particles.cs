using UniMini;
using UnityEngine;
using System.Collections.Generic;

public class Particles : DemoReel
{
    enum EmitterExamples
    {
        Fire,
        Smoke,
        WaterSpout,
        Rain,
        Stars,
        ExplosionBurst,
        PixelsExplosion,
        ConfettiBurst,
        SpaceWarp,
        SpaceWarpInABox,
        Amoebas,
        WhirlyBird,
        Firecracker,
        NbEXamples
    }

    List<Emitter> m_Emitters;

    EmitterExamples currentEmitter => (EmitterExamples)currentDemoIndex;

    public override int nbDemos => (int)EmitterExamples.NbEXamples;

    public override string currentDemoName => ((EmitterExamples)currentDemoIndex).ToString();

    public override void InitReel()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/particle-system");
        m_Emitters = new List<Emitter>();
        currentDemoIndex = (int)EmitterExamples.Fire;
        showDebug = true;
    }

    public override void InitDemo(int demoIndex)
    {
        InitEmitter(currentEmitter);
    }

    public override void UpdateDemo()
    {
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

        if (Spritz.GetKeyDown(KeyCode.X))
        {
            // Spawn emitter
            AddEmitter(currentEmitter);
        }

        foreach (var e in m_Emitters)
            e.Update();
    }

    public override void DrawDemo()
    {
        // Draw stuff:
        if (showDebug)
        {
            Spritz.Print($"Em: {m_Emitters.Count} Parts: {NumParticles()}", 0, 110, Spritz.palette[7]);
            SpritzUtil.DrawTimeInfo(0, 120);
        }

        foreach (var e in m_Emitters)
            e.Draw();
    }

    private void DrawDebug()
    {
        var sprites = GetSprites(22, 23, 25, 27, 28, 29);

        for (var i = 0; i < sprites.Length; ++i)
        {
            Spritz.DrawSprite(sprites[i], i * 24, 10);
        }
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

    private static Color32[] GetColorsFromRange(int startIndex, int endIndex)
    {
        List<Color32> colors = new List<Color32>();
        for (var i = startIndex; i <= endIndex; ++i)
            colors.Add(Spritz.palette[i]);
        return colors.ToArray();
    }

    private static Color32[] GetColors(params int[] colorIndexes)
    {
        List<Color32> colors = new List<Color32>();
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
        return $"{title} id{p.id} x:{p.pos.x} y:{p.pos.y} sz:{p.size} life:{p.life} sprIndex: {p.sprite.frameIndex}";
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
                    main.pColors = new Color32[] { Spritz.palette[8], Spritz.palette[9], Spritz.palette[10], Spritz.palette[5] };
                    main.SetSpawnArea(5, 0);
                    main.SetSpeed(ValueSpan.Spread(15, 20), ValueSpan.Spread(5, 20));
                    main.SetLife(ValueSpan.Spread(0.5f, 1));
                    main.SetAngle(ValueSpan.Spread(90, 10));
                    main.SetSize(ValueSpan.Spread(1.5f, 2), ValueSpan.Spread(0, 0));
                    var flicker = 1.1f;
                    main.customUpdate = e =>
                    {
                        e.SetAngle(ValueSpan.Spread(90 + (SpritzUtil.Sinp8(Time.time * flicker) * 27), e.pAngle.spread));
                    };
                    m_Emitters.Add(main);
                    break;
            }
            case EmitterExamples.Smoke:
                {
                    var main = new Emitter(64, 64, 4, 110);
                    main.pColors = new Color32[] { Spritz.palette[5], Spritz.palette[6] };
                    main.SetSpawnArea(5, 0);
                    main.SetSpeed(ValueSpan.Spread(8, 10), ValueSpan.Spread(5, 5));
                    main.SetLife(ValueSpan.Spread(0.5f, 4));
                    main.SetAngle(ValueSpan.Spread(80, 50));
                    m_Emitters.Add(main);
                    break;
                }
            case EmitterExamples.WaterSpout:
                {
                    var main = new Emitter(110, 90, 1, 0);
                    main.spawnOptions |= EmitterOptions.Gravity;
                    main.pColors = new Color32[] { Spritz.palette[12], Spritz.palette[1] };
                    main.SetSize(ValueSpan.Spread(2, 5), ValueSpan.Spread(0, 5));
                    main.SetAngle(ValueSpan.Spread(90, 45));
                    main.SetLife(ValueSpan.Spread(2, 2));
                    main.SetSpeed(ValueSpan.Value(100), ValueSpan.Value(50));
                    m_Emitters.Add(main);

                    var spray = new Emitter(110, 90, 1, 0);
                    main.spawnOptions |= EmitterOptions.Gravity;
                    spray.pColors = new Color32[] { Spritz.palette[7], Spritz.palette[6], Spritz.palette[5] };
                    spray.SetSize(ValueSpan.Value(0), ValueSpan.Value(1));
                    spray.SetAngle(ValueSpan.Spread(90, 45));
                    spray.SetLife(ValueSpan.Spread(2, 2));
                    spray.SetSpeed(ValueSpan.Value(100), ValueSpan.Value(50));
                    m_Emitters.Add(spray);

                    var explo = new Emitter(110, 90, 0.1f, 0);
                    explo.pColors = new Color32[] { Spritz.palette[7], Spritz.palette[6], Spritz.palette[5] };
                    explo.SetSpawnArea(20, 20);
                    explo.SetSize(ValueSpan.Spread(2, 2), ValueSpan.Value(0));
                    explo.SetAngle(ValueSpan.Spread(90, 45));
                    explo.SetLife(ValueSpan.Spread(1, 1));
                    explo.SetSpeed(ValueSpan.Spread(10, 10), ValueSpan.Spread(10, 10));
                    m_Emitters.Add(explo);

                    break;
                }
            case EmitterExamples.Rain:
                {
                    var main = new Emitter(64, 12, 2, 200);
                    main.pSprite = GetAnimSpriteFromRange(91, 97);
                    main.SetSpawnArea(50, 10);
                    main.spawnOptions |= EmitterOptions.Gravity;
                    main.SetSpeed(ValueSpan.Value(0), ValueSpan.Value(0));
                    main.SetLife(ValueSpan.Spread(1.5f, 1));
                    main.SetAngle(ValueSpan.Spread(90, 10));
                    main.SetSize(ValueSpan.Value(1), ValueSpan.Value(1));
                    m_Emitters.Add(main);
                    break;
                }
            case EmitterExamples.Stars:
                {
                    var front = new Emitter(0, 64, 0.2f, 0);
                    front.SetSpawnArea(0, 128);
                    front.pColors = new Color32[] { Spritz.palette[7] };
                    front.SetSize(ValueSpan.Value(1), ValueSpan.Value(1));
                    front.SetAngle(ValueSpan.Value(0));
                    front.SetLife(ValueSpan.Value(3.5f));
                    front.SetSpeed(ValueSpan.Spread(34, 10), ValueSpan.Spread(34, 10));
                    m_Emitters.Add(front);

                    var midfront = front.Clone();
                    midfront.frequency = 0.15f;
                    midfront.SetLife(ValueSpan.Value(4.5f));
                    midfront.pColors = new Color32[] { Spritz.palette[6] };
                    midfront.SetSpeed(ValueSpan.Spread(26, 5), ValueSpan.Spread(26, 5));
                    m_Emitters.Add(midfront);

                    var midback = front.Clone();
                    midback.SetLife(ValueSpan.Value(6.8f));
                    midback.pColors = new Color32[] { Spritz.palette[5] };
                    midback.SetSpeed(ValueSpan.Spread(18, 5), ValueSpan.Spread(18, 5));
                    midback.frequency = 0.1f;
                    m_Emitters.Add(midback);
                    
                    var back = front.Clone();
                    back.frequency = 0.07f;
                    back.SetLife(ValueSpan.Value(11));
                    back.pColors = new Color32[] { Spritz.palette[1] };
                    back.SetSpeed(ValueSpan.Spread(10, 5), ValueSpan.Spread(10, 5));
                    m_Emitters.Add(back);

                    var special = new Emitter(64, 64, 0.2f, 0);
                    special.pSprite = GetAnimSpriteFromRange(78, 84);
                    special.SetSpawnArea(128, 128);
                    special.SetAngle(ValueSpan.Value(0));
                    special.frequency = 0.01f;
                    special.SetSpeed(ValueSpan.Spread(30, 15), ValueSpan.Spread(30, 15));
                    special.SetLife(ValueSpan.Value(1));
                    m_Emitters.Add(special);

                    break;
                }
            case EmitterExamples.ExplosionBurst:
                {
                    var explo = new Emitter(64, 64, 0, 30);
                    explo.SetSize(ValueSpan.Spread(4, 3), ValueSpan.Value(0));
                    explo.SetSpeed(ValueSpan.Value(0), ValueSpan.Value(0));
                    explo.SetLife(ValueSpan.Value(1));
                    explo.pColors = new Color32[] { Spritz.palette[7], Spritz.palette[6], Spritz.palette[5] };
                    explo.SetSpawnArea(30, 30);
                    explo.SetBurst(10);
                    m_Emitters.Add(explo);

                    var spray = new Emitter(64, 64, 0, 80);
                    spray.SetSize(ValueSpan.Value(1), ValueSpan.Value(1));
                    spray.SetSpeed(ValueSpan.Spread(20, 20), ValueSpan.Spread(10, 10));
                    spray.SetLife(ValueSpan.Spread(0, 1.3f));
                    spray.pColors = new Color32[] { Spritz.palette[7], Spritz.palette[6], Spritz.palette[5] };
                    spray.SetBurst(30);
                    m_Emitters.Add(spray);

                    var anim = new Emitter(64, 64, 0, 18);
                    anim.SetSpeed(ValueSpan.Value(0), ValueSpan.Value(0));
                    anim.SetLife(ValueSpan.Value(1));
                    anim.SetBurst(6);
                    anim.SetSpawnArea(30, 30);
                    anim.pSprite = GetAnimSprite(32, 33, 34, 35, 36, 37, 38, 39, 40, 40, 40, 41, 41, 41);
                    m_Emitters.Add(anim);

                    break;
                }
            case EmitterExamples.PixelsExplosion:
                {
                    var left = new Emitter(64, 64, 0, 200);
                    left.spawnOptions |= EmitterOptions.Gravity;
                    left.SetBurst();
                    left.SetSpeed(ValueSpan.Spread(50, 50), ValueSpan.Spread(10, 50));
                    left.SetLife(ValueSpan.Value(3f));
                    left.SetAngle(ValueSpan.Spread(90, 35));
                    left.spawnOptions |= EmitterOptions.RandomColor;
                    left.pColors = GetColorsFromRange(7, 15);
                    m_Emitters.Add(left);

                    var right = left.Clone();
                    left.SetAngle(ValueSpan.Spread(65, 35));
                    m_Emitters.Add(right);

                    break;
                }
            case EmitterExamples.ConfettiBurst:
                {
                    var left = new Emitter(0, 90, 0, 50);
                    left.spawnOptions |= EmitterOptions.Gravity;
                    left.SetBurst();
                    left.SetSize(ValueSpan.Spread(0, 2), ValueSpan.Spread(0, 2));
                    left.SetSpeed(ValueSpan.Spread(50, 50), ValueSpan.Spread(50, 50));
                    left.SetLife(ValueSpan.Spread(1f, 2f));
                    left.SetAngle(ValueSpan.Spread(30, 45));
                    left.spawnOptions |= EmitterOptions.RandomColor;
                    left.pColors = GetColorsFromRange(7, 15);
                    m_Emitters.Add(left);

                    var right = left.Clone();
                    right.pos = new Vector2(128, 90);
                    right.SetAngle(ValueSpan.Spread(105, 45));
                    m_Emitters.Add(right);

                    break;
                }
            case EmitterExamples.SpaceWarp:
                {
                    var warp = new Emitter(70, 70, 11, 520);
                    warp.SetSpeed(ValueSpan.Value(30), ValueSpan.Value(200));
                    warp.SetLife(ValueSpan.Value(0.8f));
                    warp.SetSize(ValueSpan.Spread(0, 0.5f), ValueSpan.Spread(2, 0));
                    warp.pColors = GetColors(7, 8, 11, 12, 14);
                    warp.spawnOptions |= EmitterOptions.RandomColor;
                    warp.customUpdate = e =>
                    {

                    };

                    m_Emitters.Add(warp);
                    break;
                }
            case EmitterExamples.SpaceWarpInABox:
                {
                    var warp = new Emitter(70, 70, 11, 520);
                    warp.SetSpeed(ValueSpan.Value(30), ValueSpan.Value(200));
                    warp.SetLife(ValueSpan.Value(0.8f));
                    warp.SetSize(ValueSpan.Spread(0, 0.5f), ValueSpan.Spread(2, 0));
                    warp.SetParticleBoundingBox(new Rect(50, 50, 50, 50));
                    warp.pColors = GetColors(7, 8, 11, 12, 14);
                    warp.spawnOptions |= EmitterOptions.RandomColor;
                    warp.customUpdate = e =>
                    {

                    };

                    m_Emitters.Add(warp);
                }
                break;
            case EmitterExamples.Firecracker:
                {
                    var warp = new Emitter(70, 70, 11, 520);
                    warp.SetBurst(520);
                    warp.SetSpeed(ValueSpan.Spread(10, 10), ValueSpan.Spread(1, 1));
                    warp.SetLife(ValueSpan.Spread(2.8f, 1));
                    warp.SetSize(ValueSpan.Value(1), ValueSpan.Value(1));
                    warp.pColors = GetColors(7, 8, 11, 12, 14);
                    warp.spawnOptions |= EmitterOptions.RandomColor;
                    warp.customUpdate = e =>
                    {

                    };

                    m_Emitters.Add(warp);
                    break;
                }
            case EmitterExamples.Amoebas:
                {
                    var grav = new Emitter(84, 64, 0.3f, 60);
                    grav.SetSpeed(ValueSpan.Spread(50, 50), ValueSpan.Spread(-50, -50));
                    grav.SetLife(ValueSpan.Spread(1, 1.15f));
                    grav.pSprite = GetAnimSprite(75, 76, 77, 72, 71, 72, 73, 74);
                    grav.SetSpawnArea(20, 110);
                    grav.SetAngle(ValueSpan.Value(180));

                    m_Emitters.Add(grav);
                    break;
                }
            case EmitterExamples.WhirlyBird:
                {
                    var bird = new Emitter(80, 80, 1, 0);
                    bird.pSprite = GetAnimSprite(22, 23, 25, 27, 28, 29);
                    bird.SetLife(ValueSpan.Value(3));
                    bird.SetAngle(ValueSpan.Value(0));
                    bird.customUpdate = e =>
                    {

                    };

                    m_Emitters.Add(bird);
                    break;
                }
        }
    }
}
