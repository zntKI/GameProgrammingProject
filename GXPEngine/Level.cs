using GXPEngine;
using GXPEngine.Core;
using System;
using TiledMapParser;

public class Level : GameObject
{
    public int Id => id;

    private int id;
	TiledLoader tiledLoader;

	public Level(string fileName, int id)
	{
        tiledLoader = new TiledLoader(fileName);
        this.id = id;

		CreateLevel();
	}

    private void CreateLevel()
    {
        tiledLoader.rootObject = this;
        tiledLoader.addColliders = false;
        tiledLoader.LoadTileLayers(1);
        tiledLoader.LoadObjectGroups(1);
        tiledLoader.addColliders = true;
        tiledLoader.autoInstance = true;
        tiledLoader.LoadTileLayers(0);
        tiledLoader.LoadObjectGroups(0);
    }

    public void ReloadLevel()
    { 
        CreateLevel();
    }
}