using UniMini;
using UnityEngine;

public class Path : DemoReel
{
    enum PathExamples
    {
        Triangle,
        Heart,
        NbExamples
    }

    SpritzPath[] m_Paths;

    public override int nbDemos => (int)PathExamples.NbExamples;

    public override string currentDemoName => ((PathExamples)currentDemoIndex).ToString();

    public override void InitReel()
    {
        currentDemoIndex = (int)PathExamples.Heart;
    }

    public override void InitDemo(int demoIndex)
    {
        var demo = (PathExamples)demoIndex;
        switch(demo)
        {
            case PathExamples.Triangle:
                {
                    var p = new SpritzPath(5, 64, Spritz.palette[1]);
                    p.Forward(60).Left(120).Forward(60).Left(120).Forward(60);
                    m_Paths = new []{ p };
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
