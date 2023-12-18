using System;
using System.Collections.Generic;
using GXPEngine;
using GXPEngine.Core;
public class Player : Sprite
{

    //Vertical movement variables
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

    //Horizontal movement variables
    private float moveSpeed;

    public Player(float x, float y) : base("square.png", false)
    {
        this.SetOrigin(width / 2, height / 2);
        this.SetXY(x, y);

        InitVariables();
    }

    private void InitVariables()
    {
        //Vertical movement variables
        jumpHeight = game.height / 16 * 2.5f;
        jumpHeightFirstSection = game.height / 16 * 2f;
        jumpHeightSecondSection = jumpHeight - jumpHeightFirstSection;

        jumpTimeMS = 800;
        jumpTimeSectionMS = jumpTimeMS / 4f;
        jumpTimeMSCounter = 0f;

        isJumping = false;

        float frames = 60 * (jumpTimeSectionMS / 1000f);

        jumpAmountFirstSection = jumpHeightFirstSection / frames;
        jumpAmountSecondSection = jumpHeightSecondSection / frames;

        fallSpeed = jumpAmountFirstSection * 1.2f;

        //Horizontal movement variables
        moveSpeed = 6f;
    }

    private void Update()
    {
        if (!isJumping)
            ApplyVerticalMovement();
        else
            ApplyJump();

        ApplyHorizontalMovement();
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

    private void ApplyHorizontalMovement()
    {
        if (Input.GetKey(Key.A))
            MoveUntilCollision(-moveSpeed, 0);
        else if (Input.GetKey(Key.D))
            MoveUntilCollision(moveSpeed, 0);
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