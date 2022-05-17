using UniMini;
using UnityEngine;
using System.Collections.Generic;

public class Particles : SpritzGame
{
    enum EmitterExamples
    {
        Fire,
        WaterSpout,
        Rain,
        Stars,
        ExplosionBurst,
        ConfettiBurst,
        SpaceWarp,
        Amoebas,
        Portal,
        WhirlyBird,
        SpiralGalaxyMonster,
        StructuresMouse,
        StructuresArrows
    }

    bool m_ShowInfo;
    List<Emitter> m_Emitters;
    EmitterExamples m_CurrentEmitter;

    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/particle-system");
        m_Emitters = new List<Emitter>();
        m_ShowInfo = true;
        m_CurrentEmitter = EmitterExamples.ConfettiBurst;

        InitEmitter(m_CurrentEmitter);
    }

    public override void UpdateSpritz()
    {
        if (Spritz.GetKeyDown(KeyCode.Escape))
        {
            // Hide/show info
            m_ShowInfo = !m_ShowInfo;
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

    private static AnimSprite GetAnimSprite(int startIndex, int endIndex)
    {
        List<SpriteId> sprites = new List<SpriteId>();
        for (var i = startIndex; i < endIndex + 1; ++i)
            sprites.Add(new SpriteId($"particle-system_{i}"));
        return new AnimSprite(4, sprites.ToArray());
    }

    private static AnimSprite GetAnimSpriteFromFrames(params int[] frames)
    {
        List<SpriteId> sprites = new List<SpriteId>();
        for (var i = 0; i < frames.Length; ++i)
            sprites.Add(new SpriteId($"particle-system_{frames[i]}"));
        return new AnimSprite(4, sprites.ToArray());
    }

    private static Color[] GetColors(int startIndex, int endIndex)
    {
        List<Color> colors = new List<Color>();
        for (var i = startIndex; i <= endIndex; ++i)
            colors.Add(Spritz.palette[i]);
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

    private void AddEmitter(EmitterExamples etype)
    {
        switch(etype)
        {
            case EmitterExamples.Fire:
            {
                    var main = new Emitter(64, 64, 4, 110);
                    main.pColors = new Color [] { Spritz.palette[8], Spritz.palette[9], Spritz.palette[10], Spritz.palette[5] };
                    main.SetArea(5, 0);
                    main.SetSpeed(15, 5, 20, 20);
                    main.SetLife(0.5f, 1);
                    main.SetAngle(90, 10);
                    main.SetSize(1.5f, 0, 2, 0);
                    main.flicker = 1.1f;
                    main.customUpdate = e =>
                    {
                        e.SetAngle(90 + (SpritzUtil.Sinp8(Time.time * e.flicker) * 27), e.pAngleSpread);
                    };
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
                    main.pSprite = GetAnimSprite(91, 97);
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
                    special.pSprite = GetAnimSprite(78, 84);
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
                    anim.pSprite = GetAnimSpriteFromFrames(32, 33, 34, 35, 36, 37, 38, 39, 40, 40, 40, 41, 41, 41);
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
                    left.pColors = GetColors(7, 15);
                    m_Emitters.Add(left);

                    var right = left.Clone();
                    right.pos = new Vector2(128, 90);
                    right.SetAngle(105, 45);
                    m_Emitters.Add(right);

                    break;
                }
            case EmitterExamples.SpaceWarp:
                {
                    break;
                }
            case EmitterExamples.Amoebas:
                {
                    break;
                }
            case EmitterExamples.Portal:
                {
                    break;
                }
            case EmitterExamples.WhirlyBird:
                {
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
