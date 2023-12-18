using System;
using System.Collections.Generic;
using GXPEngine;
using GXPEngine.Core;
public class Player : Sprite
{

    //Jump variables
    private float fallSpeed;
    private float jumpHeight;
    private float jumpHeightFirstSection;
    private float jumpHeightSecondSection;

    private float jumpTimeMS;
    private float jumpTimeSectionMS;
    private float jumpTimeMSCounter;

    private float jumpAmountFirstSection;
    private float jumpAmountSecondSection;

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
        jumpHeight = game.height / 16 * 2.5f;
        jumpHeightFirstSection = game.height / 16 * 2f;
        jumpHeightSecondSection = jumpHeight - jumpHeightFirstSection;

        jumpTimeMS = 1330;
        jumpTimeSectionMS = jumpTimeMS / 4f;
        jumpTimeMSCounter = 0f;

        isJumping = false;

        float frames = 60 * (jumpTimeSectionMS / 1000f);

        jumpAmountFirstSection = jumpHeightFirstSection / frames;
        jumpAmountSecondSection = jumpHeightSecondSection / frames;

        fallSpeed = jumpAmountFirstSection * 2;
    }

    private void Update()
    {
        if (!isJumping)
            ApplyVerticalMovement();
        else
            ApplyJump();
    }

    private void ApplyVerticalMovement()
    {
        var coll = MoveUntilCollision(0, fallSpeed);

        if (Input.GetKeyDown(Key.SPACE) && coll != null)
        {
            isJumping = true;
            jumpTimeMSCounter = 0;
        }
    }

    private void ApplyJump()
    {
        jumpTimeMSCounter += Time.deltaTime;
        Jump();
    }

    private void Jump()
    {
        if (jumpTimeMSCounter < jumpTimeSectionMS)
        {
            y -= jumpAmountFirstSection;
        }
        else if (jumpTimeMSCounter < jumpTimeSectionMS * 2)
        {
            y -= jumpAmountSecondSection;
        }
        else if (jumpTimeMSCounter < jumpTimeSectionMS * 3)
        {
            y += jumpAmountSecondSection;
        }
        else if (jumpTimeMSCounter < jumpTimeMS)
        {
            MoveUntilCollision(0, jumpAmountFirstSection);
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