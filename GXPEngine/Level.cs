using GXPEngine;
using GXPEngine.Core;
using System;
using TiledMapParser;

public class Level : GameObject
{
    private int id;
	private TiledLoader tiledLoader;

    private Player player;

	public Level(string fileName, int id)
	{
        tiledLoader = new TiledLoader(fileName);
        this.id = id;

		CreateLevel();
	}

    private void Update()
    {
        CheckPlayerPosition();
    }

    private void CheckPlayerPosition()
    {
        Vector2 globalPos = TransformPoint(player.x, player.y);
        if (globalPos.y < 0)
        {
            LoadLevel();
        }
        else if (globalPos.y > game.height || player.ShouldDie)
        {
            PlayerDie();
        }
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

        player = FindObjectOfType<Player>();
    }

    public void ReloadLevel()
    {
        var children = GetChildren();
        foreach (var child in children)
        { 
            child.Destroy();
        }
        CreateLevel();
    }

    private void PlayerDie()
    {
        new Sound("Sounds/die1.wav").Play(false, 0, 0.5f);
        ReloadLevel();
    }

    private void LoadLevel()
    {
        Level level = new Level($"level{id + 1}.tmx", id + 1);

        Destroy();

        game.AddChild(level);
    }
}