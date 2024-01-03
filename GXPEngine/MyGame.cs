using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using TiledMapParser;

public class MyGame : Game {
	public MyGame() : base(128, 128, false, false, 1024, 1024, true)
	{
		//      int[,] level = new int[16, 16]
		//      {
		//          { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1},
		//          { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1},
		//          { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		//      };

		//      for (int i = 0; i < level.GetLength(0); i++)
		//      {
		//          for (int j = 0; j < level.GetLength(1); j++)
		//          {
		//              if (level[i, j] == 1)
		//              {
		//                  TestBlock testBlock11 = new TestBlock(j * 64 + 32, i * 64 + 32);
		//                  AddChild(testBlock11);
		//              }
		//          }
		//      }

		//      Player player = new Player(width / 2, height / 2); ;
		//AddChild(player);

		TiledLoader tiledLoader = new TiledLoader("level1.tmx");

		tiledLoader.addColliders = false;
		tiledLoader.LoadTileLayers(0);
		tiledLoader.LoadObjectGroups(1);
		tiledLoader.addColliders = true;
		tiledLoader.LoadTileLayers(1);
		tiledLoader.LoadObjectGroups(0);
	}

	void Update() {
		
	}

	static void Main()
	{
		new MyGame().Start();                   // Create a "MyGame" and start it
	}
}