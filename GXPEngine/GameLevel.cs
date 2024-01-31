using GXPEngine;
using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameLevel : Level
{
    private Player player;

    private HUD hud;
    private int durationToShowHUD = 1000;
    private int durationToShowHUDCounter = 0;

    public GameLevel(string fileName, int id) : base(fileName, id)
    {
    }

    private void Update()
    {
        CheckPlayerPosition();

        if (durationToShowHUDCounter < durationToShowHUD)
        {
            hud.ShowTime();
            hud.ShowScore(id);

            durationToShowHUDCounter += Time.deltaTime;
        }
        else
        {
            hud.ClearTime();
            hud.ClearScore();
        }
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

    protected override void CreateLevel()
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

        hud = new HUD();
        AddChild(hud);

        durationToShowHUDCounter = 0;
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

    protected override void LoadLevel()
    {
        GameLevel level = new GameLevel($"level{id + 1}.tmx", id + 1);

        Destroy();

        game.AddChild(level);
    }
}