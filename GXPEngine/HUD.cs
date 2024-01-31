using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HUD : GameObject
{
    EasyDraw time;
    EasyDraw score;

    Font font;

    public HUD()
    {
        font = Utils.LoadFont("pico-8-mono.ttf", 3);

        time = new EasyDraw(4 * 8 + 2, 10, false);
        time.TextFont(font);
        time.TextAlign(CenterMode.Center, CenterMode.Center);
        time.Fill(Color.White);
        //time.Text("00:00:12", true, 255, 0, 0, 0);
        time.SetOrigin(time.width / 2, time.height / 2);
        time.SetXY(20f, 7f);
        AddChild(time);

        score = new EasyDraw(10 * 8, 13, false);
        score.TextFont(font);
        score.TextAlign(CenterMode.Center, CenterMode.Center);
        score.Fill(Color.White);
        //score.Text("100P", true, 255, 0, 0, 0);
        score.SetOrigin(score.width / 2, score.height / 2);
        score.SetXY(game.width / 2, game.height / 2);
        AddChild(score);
    }

    public void ShowTime()
    {
        int timeMS = Time.time;

        int seconds = (timeMS / 1000) % 60;
        int minutes = (timeMS / (1000 * 60)) % 60;
        int hours = (timeMS / (1000 * 60 * 60)) & 24;

        StringBuilder sb = new StringBuilder();

        sb.Append((hours < 10 ? $"0{hours}" : $"{hours}") + ":");
        sb.Append((minutes < 10 ? $"0{minutes}" : $"{minutes}") + ":");
        sb.Append(seconds < 10 ? $"0{seconds}" : $"{seconds}");

        time.Text(sb.ToString(), true, 255, 0, 0, 0);
    }

    public void ClearTime()
    { 
        time.ClearTransparent();
    }

    public void ShowScore(int levelId)
    {
        score.Text($"{levelId * 100}p", true, 255, 0, 0, 0);
    }

    public void ClearScore()
    {
        score.ClearTransparent();
    }
}
