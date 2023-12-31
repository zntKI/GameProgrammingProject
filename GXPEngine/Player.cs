using System;
using System.Linq;
using GXPEngine;
using GXPEngine.Core;

public class Player : Sprite
{
    private enum PlayerState { None, Fall, Jump, WallSlide, WallJump, Dash }
    PlayerState currentState;

    private enum SpriteDirection { Left, Top, Right, Bottom }
    SpriteDirection spriteDirection;

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
    private float currentMoveSpeed;

    //Dash variables
    private bool canDash;

    private Vector2 dashDirection;

    private Vector2 dashDistance;
    private Vector2 dashDistanceFirstSection;
    private Vector2 dashDistanceSecondSection;

    private float dashTimeMS;
    private float dashTimeSectionMS;
    private float dashTimeMSCounter;

    private Vector2 dashAmountFirstSection;
    private Vector2 dashAmountSecondSection;

    public Player(float x, float y) : base("square.png", false)
    {
        this.SetOrigin(width / 2, height / 2);
        this.SetXY(x, y);

        InitVariables();
    }

    private void InitVariables()
    {
        currentState = PlayerState.Fall;

        spriteDirection = SpriteDirection.Right;

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
        currentMoveSpeed = moveSpeed;

        //Dash variables
        canDash = true;

        dashDirection = new Vector2();

        dashDistance = new Vector2(game.height / 16 * 4f, game.height / 16 * 3.5f);
        dashDistanceFirstSection = new Vector2(game.height / 16 * 3.5f, game.height / 16 * 3f);
        dashDistanceSecondSection = new Vector2(dashDistance.x - dashDistanceFirstSection.x, dashDistance.y - dashDistanceFirstSection.y);

        dashTimeMS = 450;
        dashTimeSectionMS = dashTimeMS / 2f;
        dashTimeMSCounter = 0f;

        frames = 60 * (dashTimeSectionMS / 1000f);

        dashAmountFirstSection = new Vector2(dashDistanceFirstSection.x / frames, dashDistanceFirstSection.y / frames);
        dashAmountSecondSection = new Vector2(dashDistanceSecondSection.x / frames, dashDistanceSecondSection.y / frames);
    }

    private void Update()
    {
        CheckForInput();
        HandleCurrentState();

        ApplyHorizontalMovement();
    }

    private void CheckForInput()
    {
        CheckInputForDirection();

        if (Input.GetKeyDown(Key.LEFT_SHIFT) && canDash)
        {
            SetCurrentState(PlayerState.Dash);
        }
    }

    /// <summary>
    /// Check for input regarding the direction variables for the sprite and dash and sets values to those accordingly
    /// </summary>
    private void CheckInputForDirection()
    {
        if (Input.GetKeyDown(Key.A))
        {
            spriteDirection = SpriteDirection.Left;
            dashDirection.x = -1;
        }
        else if (Input.GetKeyDown(Key.D))
        {
            spriteDirection = SpriteDirection.Right;
            dashDirection.x = 1;
        }

        if (Input.GetKey(Key.W))
        {
            spriteDirection = SpriteDirection.Top;
            dashDirection.y = -1;
            if (!Input.GetKey(Key.A) && !Input.GetKey(Key.D))
            {
                dashDirection.x = 0;
            }
        }
        else if (Input.GetKey(Key.S))
        {
            spriteDirection = SpriteDirection.Bottom;
            dashDirection.y = 1;
            if (!Input.GetKey(Key.A) && !Input.GetKey(Key.D))
            {
                dashDirection.x = 0;
            }
        }
        else
        {
            switch (dashDirection.x)
            {
                case -1:
                    spriteDirection = SpriteDirection.Left;
                    break;
                case 1:
                    spriteDirection = SpriteDirection.Right;
                    break;
            }
            dashDirection.y = 0;
        }
    }

    private void HandleCurrentState()
    {
        switch (currentState)
        {
            case PlayerState.Fall:
                ApplyVerticalMovement();
                break;
            case PlayerState.Jump:
                ApplyJump();
                break;
            case PlayerState.WallSlide:
                ApplyWallSlide();
                break;
            case PlayerState.WallJump:
                break;
            case PlayerState.Dash:
                ApplyDash();
                break;
            default:
                ApplyNoVerticalMovement();
                break;
        }
    }

    private void SetCurrentState(PlayerState state)
    {
        currentState = state;
        switch (currentState)
        {
            case PlayerState.Fall:
                break;
            case PlayerState.Jump:
                jumpTimeMSCounter = 0f;
                break;
            case PlayerState.WallSlide:
                break;
            case PlayerState.WallJump:
                break;
            case PlayerState.Dash:
                canDash = false;
                dashTimeMSCounter = 0f;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// <para>An equivalent to being grounded</para>
    /// Called when the current state is set to None
    /// </summary>
    private void ApplyNoVerticalMovement()
    {
        //If falling off a platform
        if (!IsGrounded(fallSpeed))
        {
            SetCurrentState(PlayerState.Fall);
        }
        else
        {
            canDash = true;
        }

        if (Input.GetKeyDown(Key.SPACE))
        {
            SetCurrentState(PlayerState.Jump);
        }
    }

    /// <summary>
    /// <para>Adding a specified amount to the player's Y coordinate, simulating the effect of gravity</para>
    /// Called when the current state is set to Fall
    /// </summary>
    private void ApplyVerticalMovement()
    {
        var coll = MoveUntilCollision(0, fallSpeed);
        if (coll != null)
        {
            SetCurrentState(PlayerState.None);
        }
    }

    /// <summary>
    /// Called when the current state is set to Jump
    /// </summary>
    private void ApplyJump()
    {
        jumpTimeMSCounter += Time.deltaTime;
        Jump();
    }

    /// <summary>
    /// Moves the player with an amount depending on the current time period of the jump
    /// </summary>
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
            SetCurrentState(PlayerState.Fall);
            return;
        }

        var coll = MoveUntilCollision(0, amountToMove);

        //Stops the jump if an interference with a collider occurres
        if (coll != null)
        {
            SetCurrentState(PlayerState.Fall);
        }
    }

    /// <summary>
    /// <para>Moves the player horizontally depending on the current input</para>
    /// Called every frame
    /// </summary>
    private void ApplyHorizontalMovement()
    {
        Collision coll = null;

        if (Input.GetKey(Key.A))
        {
            coll = MoveUntilCollision(-currentMoveSpeed, 0);
        }
        else if (Input.GetKey(Key.D))
        {
            coll = MoveUntilCollision(currentMoveSpeed, 0);
        }

        CheckForWallSliding(coll);
    }

    /// <summary>
    /// Checks if wall sliding is available or if it is not anymore
    /// </summary>
    /// <param name="coll">Possible sideways collision with a wall(or null)</param>
    private void CheckForWallSliding(Collision coll)
    {
        if (coll != null && !IsGrounded(wallSlideSpeed))
        {
            SetCurrentState(PlayerState.WallSlide);
        }
        else if (currentState == PlayerState.WallSlide && coll == null)
        {
            SetCurrentState(PlayerState.Fall);
        }
    }

    /// <summary>
    /// Checks for collisions with colliders placed beneath the player
    /// </summary>
    /// <param name="vy">The amount to try to increase the position with in order to check for a collision</param>
    /// <returns>true, if a collision is present</returns>
    private bool IsGrounded(float vy)
    {
        return GetFutureCollisions(0, vy).FirstOrDefault(obj => obj.y > this.y) != null;
    }

    /// <summary>
    /// <para>Adding a specified amount to the player's Y coordinate, simulating the effect of wall sliding</para>
    /// Called when the current state is set to WallSlide
    /// </summary>
    private void ApplyWallSlide()
    {
        var coll = MoveUntilCollision(0, wallSlideSpeed);

        if (coll != null)
            SetCurrentState(PlayerState.Fall);

        //Console.WriteLine("Wall sliding!");
    }

    /// <summary>
    /// Called when the current state is set to Dash
    /// </summary>
    private void ApplyDash()
    {
        dashTimeMSCounter += Time.deltaTime;
        Dash();
    }

    /// <summary>
    /// Moves the player with an amount depending on the dash direction
    /// </summary>
    private void Dash()
    {
        //Code depending on the last registered facing direction before the dash
        Vector2 amountToDash;
        if (dashTimeMSCounter < dashTimeSectionMS)
        {
            amountToDash = new Vector2(dashAmountFirstSection.x, dashAmountFirstSection.y);
            currentMoveSpeed = moveSpeed * 0.1f;
        }
        else
        {
            SetCurrentState(PlayerState.Fall);
            currentMoveSpeed = moveSpeed;
            return;
        }

        amountToDash.x *= dashDirection.x;
        amountToDash.y *= dashDirection.y;
        var coll = MoveUntilCollision(amountToDash.x, amountToDash.y);

        //Stops the dash if an interference with a collider occurres
        if (coll != null)
        {
            SetCurrentState(PlayerState.Fall);
            currentMoveSpeed = moveSpeed;
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