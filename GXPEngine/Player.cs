using System;
using System.Collections.Generic;
using GXPEngine;
using GXPEngine.Core;
public class Player : Sprite
{

    //Jump variables
    private float fallSpeed;
    private float jumpHeight;
    private float jumpHeightFirst;
    private float jumpHeightSecond;

    private float jumpTimeMS;
    private float jumpTimeSectionMS;
    private float jumpTimeMSCounter;

    private float jumpAmountFirst;
    private float jumpAmountSecond;

    private bool isJumping;

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
        jumpHeight = game.height / 16 * 2.5f;
        jumpHeightFirst = game.height / 16 * 2f;
        jumpHeightSecond = jumpHeight - jumpHeightFirst;

        jumpTimeMS = 1330;
        jumpTimeSectionMS = jumpTimeMS / 4f;
        jumpTimeMSCounter = 0f;

        isJumping = false;

        float frames = 60 * (jumpTimeSectionMS / 1000f);

        jumpAmountFirst = jumpHeightFirst / frames;
        jumpAmountSecond = jumpHeightSecond / frames;
    }

    private void Update()
    {
        Console.WriteLine(game.currentFps);
        ApplyVerticalMovement();
    }

    private void ApplyVerticalMovement()
    {
        if (!isJumping)
        {
            var coll = MoveUntilCollision(0, fallSpeed);

            if (Input.GetKeyDown(Key.SPACE) && coll != null)
            {
                isJumping = true;

                jumpTimeMSCounter = 0;
            }
        }
        else
        {
            jumpTimeMSCounter += Time.deltaTime;
            Console.WriteLine(jumpTimeMSCounter);

            Jump();
        }
    }

    private void Jump()
    {
        if (jumpTimeMSCounter < jumpTimeSectionMS)
        {
            y -= jumpAmountFirst;
        }
        else if (jumpTimeMSCounter < jumpTimeSectionMS * 2)
        {
            y -= jumpAmountSecond;
        }
        else if (jumpTimeMSCounter < jumpTimeSectionMS * 3)
        {
            y += jumpAmountSecond;
        }
        else if (jumpTimeMSCounter < jumpTimeMS)
        {
            //y += jumpAmountFirst;
            MoveUntilCollision(0, jumpAmountFirst);
        }
        else
        {
            isJumping = false;
        }
    }
}

public class TestBlock : Sprite
{
    public TestBlock(float x, float y) : base("colors.png", false)
    {
        this.SetOrigin(width / 2, height / 2);
        this.SetXY(x, y);
    }
}