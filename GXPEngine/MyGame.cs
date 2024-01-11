using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using TiledMapParser;
using System.Collections.Generic;

public class MyGame : Game {

	LevelList levelList;

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

        //AddChild(new Level("level1.tmx"));
        levelList = new LevelList();
        AddChild(levelList);

        Level level1 = new Level("level1.tmx");
        AddChild(level1);

        levelList.AddLevel(level1);
    }

	void Update() {
		
	}

	static void Main()
	{
		new MyGame().Start();                   // Create a "MyGame" and start it
	}
}

public class LevelList : GameObject
{
    List<Level> levels = new List<Level>();
	public Level currentLevel;

    public void AddLevel(Level level)
    {
        levels.Add(level);
        currentLevel = level;
    }
}