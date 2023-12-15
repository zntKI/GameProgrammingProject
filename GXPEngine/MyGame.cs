using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game {
	public MyGame() : base(1024, 1024, false, false)     // Create a window that's 800x600 and NOT fullscreen
	{
		//Console.WriteLine(targetFps);
		TestBlock testBlock = new TestBlock(width / 2, height - 32);
        AddChild(testBlock);

        Player player = new Player(width / 2, height / 2); ;
		AddChild(player);
	}

	// For every game object, Update is called every frame, by the engine:
	void Update() {
		
	}

	static void Main()                          // Main() is the first method that's called when the program is run
	{
		new MyGame().Start();                   // Create a "MyGame" and start it
	}
}