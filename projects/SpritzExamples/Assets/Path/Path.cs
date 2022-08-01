using UniMini;

public class Path : SpritzGame
{
    SpritzPath m_Path;
    public override void InitializeSpritz()
    {
        m_Path = new SpritzPath(5, 64, 10, Spritz.palette[1])
        {
            debugDraw = false
        };
        m_Path.Forward(60).Left(120).Forward(60).Left(120).Forward(60);
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
        m_Path.Update();
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Spritz.palette[0]);
        m_Path.Draw();
        if (m_Path.finalized)
            m_Path.Reset();
    }
}
