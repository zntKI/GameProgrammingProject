using System;
using System.Collections.Generic;
using GXPEngine;
public class Player : Sprite
{

    //Jump variables
    private float fallSpeed;
    private float jumpHeight;

    public Player(float x, float y) : base("square.png", false)
    {
        this.SetOrigin(width / 2, height / 2);
        this.SetXY(x, y);

        InitVariables();
    }

    private void InitVariables()
    {
        //Jump variables
        fallSpeed = 3f;
        jumpHeight = game.height / 16 * 2.25f;
    }

    private void Update()
    {
        ApplyVerticalMovement();
    }

    private void ApplyVerticalMovement()
    {
        //TODO: remove this once you have added the actual level
        if (this.y + this.height / 2 + fallSpeed < game.height)
        {
            MoveUntilCollision(0, fallSpeed);
        }
    }
}
