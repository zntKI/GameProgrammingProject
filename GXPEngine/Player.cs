using System;
using System.Collections.Generic;
using GXPEngine;
public class Player : Sprite
{
    public Player(float x, float y) : base("square.png", false, true)
    {
        this.SetOrigin(width / 2, height / 2);
        this.SetXY(x, y);
    }


}
