using UniMini;
using UnityEngine;

public class Path : DemoReel
{
    enum PathExamples
    {
        Triangle,
        Heart,
        SpiderWeb,
        Spore,
        NbExamples
    }

    SpritzPath[] m_Paths;

    public override int nbDemos => (int)PathExamples.NbExamples;

    public override string currentDemoName => ((PathExamples)currentDemoIndex).ToString();

    public override void InitReel()
    {
        currentDemoIndex = (int)PathExamples.Spore;
    }

    public override void InitDemo(int demoIndex)
    {
        var demo = (PathExamples)demoIndex;
        switch (demo)
        {
            case PathExamples.Triangle:
                {
                    var p = new SpritzPath(5, 64, Spritz.palette[1]);
                    p.Forward(60).Left(120).Forward(60).Left(120).Forward(60);
                    m_Paths = new[] { p };
                    break;
                }
            case PathExamples.Heart:
                {
                    var origin = new Vector2(64, 120);
                    var lp = new SpritzPath(origin.x, origin.y, Spritz.palette[1]);
                    lp.isDrawing = false;
                    lp.Goto(lp.position.x, lp.position.y);
                    lp.isDrawing = true;
                    lp.speed = 2;
                    lp.Left(140).Forward(48);

                    var rp = new SpritzPath(origin.x, origin.y, Spritz.palette[1]);
                    rp.isDrawing = false;
                    rp.Goto(rp.position.x, rp.position.y);
                    rp.isDrawing = true;
                    rp.speed = 2;
                    rp.Left(40).Forward(48);

                    for (var i = 0; i < 90; ++i)
                    {
                        lp.Right(2).Forward(1);
                        rp.Left(2).Forward(1);
                    }

                    m_Paths = new[] { rp, lp };
                    break;
                }
            case PathExamples.SpiderWeb:
                {
                    var origin = new Vector2(64, 64);
                    var rp = new SpritzPath(origin.x, origin.y, Spritz.palette[1]);
                    rp.speed = 15;
                    var side = 64;
                    for (var i = 0; i < 6; ++i)
                    {
                        rp.PenDown().Forward(side).
                            PenUp().Back(side).Right(60);
                    }

                    rp.PenUp();
                    for (var i = 0; i < 7; ++i)
                    {
                        rp.speed = 11;
                        rp.heading = 0;
                        rp.Forward(side).Right(120);

                        rp.PenDown();
                        for (var j = 0; j < 6; ++j)
                        {
                            rp.Forward(side).Right(60);
                        }

                        rp.PenUp();
                        rp.heading = 0;
                        rp.speed = 15;
                        rp.Back(side);
                        side -= 8;
                    }

                    m_Paths = new[] { rp };
                    break;
                }
            case PathExamples.Spore:
                {
                    var origin = new Vector2(64, 0);
                    var rp = new SpritzPath(origin.x, origin.y, Spritz.palette[1]);
                    rp.speed = 100;
                    rp.PenDown();
                    var a = 1;
                    var b = 1;
                    while (b < 128)
                    {
                        rp.Forward(a).Right(b);
                        a += 1;
                        b += 1;
                    }
                    m_Paths = new[] { rp };
                    break;
                }
        }
        
    }

    public override void UpdateDemo()
    {
        // Update objects behavior according to input
        foreach(var p in m_Paths)
            p.Update();
    }

    public override void DrawDemo()
    {
        foreach (var p in m_Paths)
            p.Draw();
    }
}
