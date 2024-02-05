using GXPEngine;
using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Class that represents all the GameLevels in the game
/// </summary>
public class GameLevel : Level
{
    private Player player;

    private HUD hud;
    private int durationToShowHUD;
    private int durationToShowHUDCounter;

    //The time the actual game started
    private int timeGameStartMS;

    private int deathCount;
    private int collectedStrawberries;

    private int currentlyCollectedStrawberries;

    public GameLevel(string fileName, int id, int timeGameStartMS, int deathCount, int collectedStrawberries) : base(fileName, id)
    {
        durationToShowHUD = 1000;
        durationToShowHUDCounter = 0;

        this.timeGameStartMS = timeGameStartMS;
        this.deathCount = deathCount;

        this.collectedStrawberries = collectedStrawberries;
        currentlyCollectedStrawberries = 0;

        CreateLevel();
    }

    private void Update()
    {
        CheckPlayerPosition();

        CheckStateForHUD();

        if (id == 21)
            CheckTriggersForHUD();
    }

    private void CheckPlayerPosition()
    {
        if (player.y < 0)
        {
            LoadLevel();
        }
        else if (player.y > game.height || player.ShouldDie)
        {
            PlayerDie();
        }
    }

    /// <summary>
    /// Checks whether to show the HUD elements or to hide them
    /// </summary>
    private void CheckStateForHUD()
    {
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

    /// <summary>
    /// Checks whether the Player currently collides with a trigger of type Flag.
    /// If it does, it shows the HUD, if not - it hides it.
    /// </summary>
    private void CheckTriggersForHUD()
    {
        if (player.CurrentTriggerCollisions.FirstOrDefault(obj => obj is Flag) != null)
        {
            hud.ShowEndState(deathCount, collectedStrawberries);
        }
        else
        {
            hud.ClearEndState();
        }
    }

    protected override void CreateLevel()
    {
        tiledLoader.rootObject = this;
        tiledLoader.addColliders = false;
        tiledLoader.LoadImageLayers();
        tiledLoader.LoadTileLayers(0);
        tiledLoader.LoadObjectGroups(0);
        tiledLoader.addColliders = true;
        tiledLoader.autoInstance = true;
        tiledLoader.LoadTileLayers(1);
        if (currentlyCollectedStrawberries == 0)
            tiledLoader.LoadObjectGroups(1);
        tiledLoader.LoadObjectGroups(2);

        player = FindObjectOfType<Player>();

        if (hud == null)
        {
            //Checks whether to create a special HUD, used only for the last GameLevel
            switch (id)
            {
                case 21:
                    hud = new HUD(Time.time, timeGameStartMS);
                    break;
                default:
                    hud = new HUD();
                    break;
            }
        }

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
        deathCount++;
        //Sets the var only if the player has currently collected the strawberry
        if (player.CollectedStrawberries != 0)
            currentlyCollectedStrawberries = player.CollectedStrawberries;

        new Sound("Sounds/die1.wav").Play(false, 0, 0.5f);
        ReloadLevel();
    }

    /// <summary>
    /// Loads the next GameLevel
    /// </summary>
    protected override void LoadLevel()
    {
        int increment = 1;
        if (id + 1 == 7 || id + 1 == 11)
        {
            increment = 2;
        }
        if (id + 1 == 16)
        {
            increment = 3;
        }

        if (player.CollectedStrawberries != 0)
            currentlyCollectedStrawberries = player.CollectedStrawberries;

        Level level;
        if (id + 1 != 22)
            level = new GameLevel($"Levels/level{id + increment}.tmx", id + increment, timeGameStartMS, deathCount, collectedStrawberries + currentlyCollectedStrawberries);
        else
            level = new MenuLevel($"Levels/level{id + increment}.tmx", id + increment);

        Destroy();

        game.AddChild(level);
    }
}