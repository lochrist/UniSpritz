using UniMini;
using UnityEngine;

public class CircleAnimations : SpritzGame
{
    int x;
    int radius;
    AnimNode[] animators;
    float startTime;
    int demo = 0;
    int color;
    public override void InitializeSpritz()
    {
        var complex = SpritzAnim.CreateValueNode("cx", 20)
                    .Sequence(SpritzAnim.CreateValueNode("cr", 0))
                    .Sequence(SpritzAnim.CreateValueNode("color", 0))
                    .Sequence(SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseCubic, "cr", 10, 1))
                    .Sequence(SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseCubic, "cx", 100, 1).Parallel(SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseCubic, "cr", 50, 1)))
                    .Sequence(SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseCubic, "cr", 0, 1))
                    .Parallel(SpritzAnim.CreateInterpolationNode(SpritzAnim.EaseLinear, "color", 200, 2));

        animators = new[] { complex };
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
        var results = animators[demo].Evaluate(Time.time, startTime);
        x = (int)results.values["cx"];
        radius = (int)results.values["cr"];
        color = (int)(results.values["color"] % Spritz.palette.Length);
        if (Spritz.GetMouseDown(0))
        {
            startTime = Time.time;
            Debug.Log("restart!");
        }
        else if (Spritz.GetMouseDown(1))
        {
            demo++;
            if (demo >= animators.Length)
                demo = 0;
        }
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);
        SpritzUtil.DrawTimeInfo();
        Spritz.DrawCircle(x, 64, radius, Spritz.palette[color], false);
    }
}
