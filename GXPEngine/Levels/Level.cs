using GXPEngine;
using GXPEngine.Core;
using System;
using TiledMapParser;

public abstract class Level : GameObject
{
    protected int id;
	protected TiledLoader tiledLoader;

	protected Level(string fileName, int id)
    {
        this.id = id;
        tiledLoader = new TiledLoader(fileName);

        CreateLevel();
    }

    protected abstract void CreateLevel();

	protected abstract void LoadLevel();
}