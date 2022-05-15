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
        // Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        m_Emitters = new List<Emitter>();
        m_ShowInfo = true;
        m_CurrentEmitter = EmitterExamples.Fire;
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
            y += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            y -= 1;
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
                    var main = new Emitter(64, 64, 4, 1);
                    main.pColors = new Color [] { Spritz.palette[8], Spritz.palette[9], Spritz.palette[10], Spritz.palette[5] };
                    // main.SetArea(5, 0);
                    // main.SetSpeed(15, 5, 20, 20);
                    main.SetSpeed(15, 5);
                    // main.SetLife(0.5f, 1);
                    main.SetLife(0.5f);
                    // main.SetAngle(90, 10);
                    main.SetAngle(90);
                    // main.SetSize(1.5f, 0, 2, 0);
                    main.SetSize(1.5f, 0);
                    main.flicker = 1.1f;
                    main.customUpdate = e =>
                    {
                        // e.SetAngle(90 + (SpritzUtil.Sinp8(Time.time * e.flicker) * 27), e.pAngleSpread);
                    };
                    m_Emitters.Add(main);
                    break;
            }
            case EmitterExamples.WaterSpout:
                {
                    break;
                }
            case EmitterExamples.Rain:
                {
                    break;
                }
            case EmitterExamples.Stars:
                {
                    break;
                }
            case EmitterExamples.ExplosionBurst:
                {
                    break;
                }
            case EmitterExamples.ConfettiBurst:
                {
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
