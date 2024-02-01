using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

public class MenuLevel : Level
{
    private readonly List<EasyDraw> easyDraws;

    Font font;

    public MenuLevel(string fileName, int id) : base(fileName, id)
    {
        font = Utils.LoadFont("pico-8-mono.ttf", 3);
        switch (id)
        {
            case 0:
                easyDraws = new List<EasyDraw>() { new EasyDraw(12 * 8, 13, false) };

                InitEasyDraw(0, "Press 'ENTER' to start", game.width / 2, game.height / 2 + 20);
                break;
            default:
                easyDraws = new List<EasyDraw>() { new EasyDraw(14 * 8, 13, false), new EasyDraw(12 * 8, 13, false) };

                InitEasyDraw(0, "Press 'ENTER' to restart", game.width / 2, game.height / 2 + 20);
                InitEasyDraw(1, "Press 'DELETE' to quit", game.width / 2, game.height / 2 + 30);
                break;
        }
    }

    private void InitEasyDraw(int i, string message, float x, float y)
    {
        if (i >= easyDraws.Capacity)
            throw new Exception("Cannot pass index higher than the list's initial count");

        easyDraws[i].TextFont(font);
        easyDraws[i].TextAlign(CenterMode.Center, CenterMode.Center);
        easyDraws[i].Fill(Color.White);
        easyDraws[i].Text(message, true, 255, 0, 0, 0);
        easyDraws[i].SetOrigin(easyDraws[i].width / 2, easyDraws[i].height / 2);
        easyDraws[i].SetXY(x, y);
        AddChild(easyDraws[i]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(Key.ENTER))
        {
            LoadLevel();
        }
        else if (id != 0 && Input.GetKeyDown(Key.DELETE2))
        {
            game.Destroy();
        }
    }

    protected override void CreateLevel()
    {
        tiledLoader.rootObject = this;
        tiledLoader.addColliders = false;
        tiledLoader.LoadTileLayers(0);
    }

    protected override void LoadLevel()
    {
        GameLevel level = new GameLevel($"level{1}.tmx", 1, Time.time, 0);

        Destroy();
        game.AddChild(level);
    }
}
