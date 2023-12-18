using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game {
	public MyGame() : base(1024, 1024, false, false)     // Create a window that's 800x600 and NOT fullscreen
	{
        //Console.WriteLine(targetFps);
        TestBlock testBlock01 = new TestBlock(width / 2 - 64, height - 32);
        AddChild(testBlock01);
        TestBlock testBlock02 = new TestBlock(width / 2 - 128, height - 32);
        AddChild(testBlock02);
        TestBlock testBlock03 = new TestBlock(width / 2 - 192, height - 32);
        AddChild(testBlock03);
        TestBlock testBlock032 = new TestBlock(width / 2 - 192, height - 32 - 64);
        AddChild(testBlock032);
        TestBlock testBlock04 = new TestBlock(width / 2 - 256, height - 32);
        AddChild(testBlock04);
        TestBlock testBlock = new TestBlock(width / 2, height - 32);
        AddChild(testBlock);
        TestBlock testBlock1 = new TestBlock(width / 2 + 64, height - 32);
        AddChild(testBlock1);
        TestBlock testBlock2 = new TestBlock(width / 2 + 128, height - 32);
        AddChild(testBlock2);
        TestBlock testBlock3 = new TestBlock(width / 2 + 192, height - 32);
        AddChild(testBlock3);
        TestBlock testBlock4 = new TestBlock(width / 2 + 256, height - 32);
        AddChild(testBlock4);

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