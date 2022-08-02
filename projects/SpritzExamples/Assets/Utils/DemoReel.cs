using System.Collections;
using System.Collections.Generic;
using UniMini;
using UnityEngine;

public abstract class DemoReel : SpritzGame
{
    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);
        Spritz.DrawRectangle(0, 0, 128, 6, Spritz.palette[1], true);
        Spritz.DrawLine(0, 7, 128, 7, Spritz.palette[2]);
        Spritz.Print(currentDemoName, 1, 1, Spritz.palette[7]);

        DrawDemo();
    }

    public override void InitializeSpritz()
    {
        InitReel();
        InitDemo(currentDemoIndex);
    }

    public override void UpdateSpritz()
    {
        if (Spritz.GetKeyDown(KeyCode.Escape))
        {
            // Hide/show info
            showDebug = !showDebug;
        }
        if (Spritz.GetKeyDown(KeyCode.P))
        {
            // Hide/show info
            isPaused = !isPaused;
        }
        if (Spritz.GetKeyDown(KeyCode.A))
        {
            if (currentDemoIndex == 0)
                currentDemoIndex = nbDemos - 1;
            else
                currentDemoIndex = currentDemoIndex - 1;
            InitDemo(currentDemoIndex);
        }
        if (Spritz.GetKeyDown(KeyCode.D))
        {
            // next examples
            if (currentDemoIndex == nbDemos - 1)
                currentDemoIndex = 0;
            else
            {
                currentDemoIndex = currentDemoIndex + 1;
            }
            InitDemo(currentDemoIndex);
        }

        if (!isPaused)
            UpdateDemo();
    }

    public int currentDemoIndex { get; set; }
    public bool showDebug { get; set; }
    public bool isPaused { get; set; }

    public abstract int nbDemos { get; }
    public abstract string currentDemoName { get; }
    public abstract void InitReel();
    public abstract void InitDemo(int demoIndex);
    public abstract void UpdateDemo();
    public abstract void DrawDemo();
}
