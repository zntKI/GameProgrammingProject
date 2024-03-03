using System;
using GXPEngine;
using System.Drawing;
using TiledMapParser;
using System.Collections.Generic;
using System.Reflection.Emit;

public class MyGame : Game
{
    public MyGame() : base(128, 128, false, false, 1024, 1024, true)
    {
        new Sound("Sounds/main.wav", true, true).Play();

        AddChild(new GameLevel("Levels/level5.tmx", 5, Time.time, 0, 0));
        //AddChild(new MenuLevel("Levels/level0.tmx", 0));
    }

    void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(Key.TAB))
        {
            Console.WriteLine(GetDiagnostics());
        }
#endif
    }

    static void Main()
    {
        new MyGame().Start();
    }
}