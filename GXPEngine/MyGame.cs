using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using TiledMapParser;
using System.Collections.Generic;
using System.Reflection.Emit;

public class MyGame : Game
{

    public MyGame() : base(128, 128, false, false, 1024, 1024, true)
    {
        new Sound("Sounds/main.wav", true, true).Play();

        AddChild(new Level("level8.tmx", 8));
    }

    void Update()
    {
        if (Input.GetKeyDown(Key.ENTER))
        {
            Console.WriteLine(GetDiagnostics());
        }
    }

    static void Main()
    {
        new MyGame().Start();                   // Create a "MyGame" and start it
    }
}