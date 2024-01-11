using GXPEngine;
using GXPEngine.Core;
using System;
using TiledMapParser;

public class Level : GameObject
{
	TiledLoader tiledLoader;

	public Level(string fileName)
	{
        tiledLoader = new TiledLoader(fileName);

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

    public void CheckColl(Collision coll)
    {
        if (coll.other is Spikes)
        {
            coll.self.LateDestroy();
            CreateLevel();
        }
    }
}