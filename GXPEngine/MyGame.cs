using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using TiledMapParser;
using System.Collections.Generic;
using System.Reflection.Emit;

public class MyGame : Game
{

    LevelList levelList;

    public MyGame() : base(128, 128, false, false, 1024, 1024, true)
    {
        levelList = new LevelList();
        AddChild(levelList);

        levelList.LoadLevel();
    }

    void Update()
    {

    }

    static void Main()
    {
        new MyGame().Start();                   // Create a "MyGame" and start it
    }
}

public class LevelList : GameObject
{
    public Level CurrentLevel => currentLevel;

    private int startingIndex = 3;
    private Level currentLevel;

    public void LoadLevel()
    {
        int levelId = currentLevel == null ? startingIndex : currentLevel.Id + 1;
        Level level = new Level($"level{levelId}.tmx", levelId);

        if (levelId != startingIndex)
        {
            game.RemoveChild(currentLevel);
            currentLevel.Destroy();
        }

        game.AddChild(level);
        //levels.Add(level);
        currentLevel = level;
    }
}