using System;
using System.Linq;
using GXPEngine;
using GXPEngine.Core;

public class Player : Sprite
{
    private enum PlayerState { None, Jump, WallSlide, WallJump, Dash }
    PlayerState currentState;

    //Vertical movement variables
    private float fallSpeed;
    private float wallSlideSpeed;

    private float jumpHeight;
    private float jumpHeightFirstSection;
    private float jumpHeightSecondSection;

    private float jumpTimeMS;
    private float jumpTimeSectionMS;
    private float jumpTimeMSCounter;

    private float jumpAmountFirstSection;
    private float jumpAmountSecondSection;

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
        currentState = PlayerState.None;

        //Vertical movement variables
        jumpHeight = game.height / 16 * 2.5f;
        jumpHeightFirstSection = game.height / 16 * 2f;
        jumpHeightSecondSection = jumpHeight - jumpHeightFirstSection;

        jumpTimeMS = 800;
        jumpTimeSectionMS = jumpTimeMS / 4f;
        jumpTimeMSCounter = 0f;

        float frames = 60 * (jumpTimeSectionMS / 1000f);

        jumpAmountFirstSection = jumpHeightFirstSection / frames;
        jumpAmountSecondSection = jumpHeightSecondSection / frames;

        fallSpeed = jumpAmountFirstSection * 1.2f;
        wallSlideSpeed = fallSpeed * 0.5f;

        //Horizontal movement variables
        moveSpeed = 6f;
    }

    private void Update()
    {
        HandleCurrentState();

        ApplyHorizontalMovement();
    }

    private void HandleCurrentState()
    {
        switch (currentState)
        {
            case PlayerState.Jump:
                ApplyJump();
                break;
            case PlayerState.WallSlide:
                ApplyWallSlide();
                break;
            case PlayerState.WallJump:
                break;
            case PlayerState.Dash:
                break;
            default:
                ApplyVerticalMovement();
                break;
        }
    }

    private void SetCurrentState(PlayerState state)
    {
        currentState = state;
        switch (currentState)
        {
            case PlayerState.Jump:
                jumpTimeMSCounter = 0f;
                break;
            case PlayerState.WallSlide:
                break;
            case PlayerState.WallJump:
                break;
            case PlayerState.Dash:
                break;
            default:
                break;
        }
    }

    private void ApplyVerticalMovement()
    {
        var coll = MoveUntilCollision(0, fallSpeed);

        if (coll != null && Input.GetKeyDown(Key.SPACE))
        {
            SetCurrentState(PlayerState.Jump);
        }
    }

    private void ApplyJump()
    {
        jumpTimeMSCounter += Time.deltaTime;
        Jump();
    }

    private void Jump()
    {
        float amountToMove;
        if (jumpTimeMSCounter < jumpTimeSectionMS)
        {
            amountToMove = -jumpAmountFirstSection;
        }
        else if (jumpTimeMSCounter < jumpTimeSectionMS * 2)
        {
            amountToMove = -jumpAmountSecondSection;
        }
        else if (jumpTimeMSCounter < jumpTimeSectionMS * 3)
        {
            amountToMove = jumpAmountSecondSection;
        }
        else if (jumpTimeMSCounter < jumpTimeMS)
        {
            amountToMove = jumpAmountFirstSection;
        }
        else
        {
            SetCurrentState(PlayerState.None);
            return;
        }

        var coll = MoveUntilCollision(0, amountToMove);

        if (coll != null)
        {
            SetCurrentState(PlayerState.None);
        }
    }

    private void ApplyHorizontalMovement()
    {
        Collision coll = null;

        if (Input.GetKey(Key.A))
            coll = MoveUntilCollision(-moveSpeed, 0);
        else if (Input.GetKey(Key.D))
            coll = MoveUntilCollision(moveSpeed, 0);

        CheckForWallSliding(coll);
    }

    private void CheckForWallSliding(Collision coll)
    {
        if (coll != null && !IsGrounded(0, wallSlideSpeed))
        {
            SetCurrentState(PlayerState.WallSlide);
        }
        else if (currentState == PlayerState.WallSlide && coll == null)
        {
            SetCurrentState(PlayerState.None);
        }
    }

    private bool IsGrounded(float vx, float vy)
    {
        return GetFutureCollisions(vx, vy).FirstOrDefault(obj => obj.y > this.y) != null;
    }

    private void ApplyWallSlide()
    {
        var coll = MoveUntilCollision(0, wallSlideSpeed);

        if (coll != null)
            SetCurrentState(PlayerState.None);

        Console.WriteLine("Wall sliding!");
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