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
    EasyDraw endState;

    Font font;

    //Variables set only when the final GameLevel has been reached and the current HUD is being used by it
    int timeEndGameMS = -1;
    int timeStartGameMS = 1;

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

        endState = new EasyDraw(game.width / 2, 30, false);
        endState.TextFont(font);
        endState.TextAlign(CenterMode.Center, CenterMode.Center);
        endState.Fill(Color.White);
        endState.SetOrigin(endState.width / 2, endState.height / 2);
        endState.SetXY(game.width / 2, 30);
        AddChild(endState);
    }

    //Ctor for the final GameLevel
    public HUD(int timeEndGameMS, int timeStartGameMS) : this()
    {
        this.timeEndGameMS = timeEndGameMS;
        this.timeStartGameMS = timeStartGameMS;
    }

    public void ShowTime(int timeGameStartMS)
    {
        time.Text(CalculateTime(Time.time, timeGameStartMS), true, 255, 0, 0, 0);
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

    public void ShowEndState(int deathCount)
    {
        if (timeStartGameMS == -1 || timeEndGameMS == -1)
        {
            throw new InvalidOperationException("Not supposed to call this method if the current HUD is not a child of the final GameLevel");
        }

        endState.Text($"{CalculateTime(timeEndGameMS, timeStartGameMS)}\n\nDeaths:{deathCount}", true, 255, 0, 0, 0);
    }

    public void ClearEndState()
    {
        if (timeStartGameMS == -1 || timeEndGameMS == -1)
        {
            throw new InvalidOperationException("Not supposed to call this method if the current HUD is not a child of the final GameLevel");
        }

        endState.ClearTransparent();
    }

    private string CalculateTime(int currentTimeMS, int timeGameStartMS)
    {
        int timeMS = currentTimeMS - timeGameStartMS;

        int seconds = (timeMS / 1000) % 60;
        int minutes = (timeMS / (1000 * 60)) % 60;
        int hours = (timeMS / (1000 * 60 * 60)) & 24;

        StringBuilder sb = new StringBuilder();

        sb.Append((hours < 10 ? $"0{hours}" : $"{hours}") + ":");
        sb.Append((minutes < 10 ? $"0{minutes}" : $"{minutes}") + ":");
        sb.Append(seconds < 10 ? $"0{seconds}" : $"{seconds}");

        return sb.ToString();
    }
}
