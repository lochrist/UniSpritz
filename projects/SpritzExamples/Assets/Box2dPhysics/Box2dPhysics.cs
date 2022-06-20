using UniMini;
using UniMini.Physics.Box2D;
using UnityEngine;
using System.Collections.Generic;

public class Demo
{
    public System.Action setup;
    public string name;
}

public class Box2dPhysics : SpritzGame
{
    Body bomb;

    const float timeStep = 1.0f / 60.0f;
    const int iterations = 10;
    Vector2 gravity = new Vector2(0.0f, -10.0f);
    int demoIndex = 0;

    int width = 1280;
    int height = 720;
    float zoom = 10.0f;
    float pan_y = 8.0f;

    int color;

    List<Demo> demos;

    World world;

    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        world = new World(gravity, iterations);

        demos = new List<Demo>();
        demos.Add(new Demo()
        {
            name = "Demo 1: A Single Box",
            setup = SetupDemo1
        });

        InitDemo(0);
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
        world.Step(Time.deltaTime);
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);

        Spritz.Print(demos[demoIndex].name, 10, 10, Color.red);

        foreach (var b in world.bodies)
            DrawBody(b);

        foreach (var b in world.joints)
            DrawJoint(b);
    }

    private void DrawBody(Body b)
    {
        var R = new Matrix2x2(b.rotation);
        var x = b.position;
        var h = 0.5f * b.width;

        var v1 = x + R * new Vector2(-h.x, -h.y);
        var v2 = x + R * new Vector2(h.x, -h.y);
        var v3 = x + R * new Vector2(h.x, h.y);
        var v4 = x + R * new Vector2(-h.x, h.y);

        if (b == bomb)
        {

        }
        else
        {
            var width = v2.x - v1.x;
            var height = v3.y - v1.y;
            Spritz.DrawRectangle(Mathf.RoundToInt(v1.x * pixelPerUnit), Mathf.RoundToInt(resolution.y - (v1.y * pixelPerUnit)), Mathf.RoundToInt(width * pixelPerUnit), Mathf.RoundToInt(height * pixelPerUnit), Color.white, false);
        }
    }

    private void DrawJoint(UniMini.Physics.Box2D.Joint j)
    {
        Body b1 = j.body1;
        Body b2 = j.body2;

        var R1 =  new Matrix2x2(b1.rotation);
        var R2 = new Matrix2x2(b2.rotation);

        var x1 = b1.position;
        var p1 = x1 + R1 * j.localAnchor1;

        var x2 = b2.position;
        var p2 = x2 + R2 * j.localAnchor2;

        Spritz.DrawCircle((int)x1.x, (int)x1.y, 10, Color.white, false);
    }

    private void InitDemo(int index)
    {
        world.Clear();
        bomb = null;

        demoIndex = index;
        demos[index].setup();
    }

    private void SetupDemo1()
    {
        {
            var b1 = new Body();
            b1.Set(new Vector2(100, 20), float.MaxValue);
            b1.position.Set(0, 10);
            world.Add(b1);
        }

        {
            var b1 = new Body();
            b1.Set(new Vector2(1, 1), 200);
            b1.position.Set(0.4f, 4f);
            world.Add(b1);
        }
    }
}
