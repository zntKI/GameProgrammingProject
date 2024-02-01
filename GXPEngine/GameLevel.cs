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
    private int durationToShowHUD;
    private int durationToShowHUDCounter;

    private int timeGameStartMS;

    public GameLevel(string fileName, int id, int timeGameStartMS) : base(fileName, id)
    {
        durationToShowHUD = 1000;
        durationToShowHUDCounter = 0;

        this.timeGameStartMS = timeGameStartMS;
    }

    private void Update()
    {
        CheckPlayerPosition();

        if (durationToShowHUDCounter < durationToShowHUD)
        {
            hud.ShowTime(timeGameStartMS);
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

        if (hud == null)
            hud = new HUD();

        AddChild(hud);
        durationToShowHUDCounter = 0;
    }

    public void ReloadLevel()
    {
        var children = GetChildren().Where(obj => !(obj is HUD));
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
        int increment = 1;
        //7 11 17
        if (id + 1 == 7 || id + 1 == 11)
        {
            increment = 2;
        }
        if (id + 1 == 16)
        {
            increment = 3;
        }

        Level level;
        if (id + 1 != 22)
            level = new GameLevel($"level{id + increment}.tmx", id + increment, timeGameStartMS);
        else
            level = new MenuLevel($"level{id + increment}.tmx", id + increment);

        Destroy();

        game.AddChild(level);
    }
}