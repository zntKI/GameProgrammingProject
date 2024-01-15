using System;
using System.Linq;
using GXPEngine;
using GXPEngine.Core;
using TiledMapParser;

public class Player : AnimationSprite
{
    private enum PlayerState { None, Fall, Jump, WallSlide, WallJump, Dash }
    PlayerState currentState;

    private enum SpriteDirection { Left, Top, Right, Bottom }
    SpriteDirection spriteDirection;

    LevelList levelList;

    //Vertical movement variables
    private float fallSpeed;
    private float currentFallSpeed;
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
    private Vector2 tempDashDirection;

    //private Vector2 dashDistance;
    private Vector2 dashDistanceFirstSection;
    //private Vector2 dashDistanceSecondSection;

    private float dashTimeMS;
    private float dashTimeSectionMS;
    private float dashTimeMSCounter;

    private Vector2 dashAmountFirstSection;
    //private Vector2 dashAmountSecondSection;

    //Wall jump variables
    private Vector2 wallJumpDistance;

    private float wallJumpTimeMS;
    private float wallJumpTimeMSCounter;

    private Vector2 wallJumpAmount;

    private enum WallJumpDirection { Left = -1, Right = 1 }
    private WallJumpDirection wallJumpDirection;

    public Player(string imageFile, int cols, int rows, TiledObject obj=null) : base(imageFile, cols, rows)
    {
        //TODO: fix the bounds of the collider so that its more fair
        this.SetOrigin(width / 2, height / 2);
        SetCycle(1, 4);

        InitVariables();
    }

    private void InitVariables()
    {
        currentState = PlayerState.Fall;

        spriteDirection = SpriteDirection.Right;

        levelList = game.FindObjectOfType<LevelList>();

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
        currentFallSpeed = fallSpeed;

        wallSlideSpeed = fallSpeed * 0.5f;

        //Horizontal movement variables
        moveSpeed = 1f;
        currentMoveSpeed = moveSpeed;

        //Dash variables
        canDash = true;

        dashDirection = new Vector2();
        tempDashDirection = new Vector2();

        //dashDistance = new Vector2(game.width / 16 * 4f, game.height / 16 * 4f);
        dashDistanceFirstSection = new Vector2(game.width / 16 * 3.5f, game.height / 16 * 3.5f);
        //dashDistanceSecondSection = new Vector2(dashDistance.x - dashDistanceFirstSection.x, dashDistance.y - dashDistanceFirstSection.y);

        dashTimeMS = 450;
        dashTimeSectionMS = dashTimeMS / 2f;
        dashTimeMSCounter = 0f;

        frames = 60 * (dashTimeSectionMS / 1000f);

        dashAmountFirstSection = new Vector2(dashDistanceFirstSection.x / frames, dashDistanceFirstSection.y / frames);
        //dashAmountSecondSection = new Vector2(dashDistanceSecondSection.x / frames, dashDistanceSecondSection.y / frames);

        //WallJump variables
        wallJumpDistance = new Vector2(game.width / 16 * 2.5f, (game.height / 16 * 2.5f) * -1f);

        wallJumpTimeMS = 300f;
        wallJumpTimeMSCounter = 0f;

        frames = 60 * (wallJumpTimeMS / 1000f);

        wallJumpAmount = new Vector2(wallJumpDistance.x / frames, wallJumpDistance.y / frames);

        wallJumpDirection = WallJumpDirection.Left;
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
            tempDashDirection.x = -1;
        }
        else if (Input.GetKeyDown(Key.D))
        {
            spriteDirection = SpriteDirection.Right;
            tempDashDirection.x = 1;
        }

        if (Input.GetKey(Key.W))
        {
            spriteDirection = SpriteDirection.Top;
            tempDashDirection.y = -1;
            if (!Input.GetKey(Key.A) && !Input.GetKey(Key.D))
            {
                tempDashDirection.x = 0;
            }
        }
        else if (Input.GetKey(Key.S))
        {
            spriteDirection = SpriteDirection.Bottom;
            tempDashDirection.y = 1;
            if (!Input.GetKey(Key.A) && !Input.GetKey(Key.D))
            {
                tempDashDirection.x = 0;
            }
        }
        else
        {
            switch (tempDashDirection.x)
            {
                case -1:
                    spriteDirection = SpriteDirection.Left;
                    break;
                case 1:
                    spriteDirection = SpriteDirection.Right;
                    break;
            }
            tempDashDirection.y = 0;
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
                ApplyWallJump();
                break;
            case PlayerState.Dash:
                ApplyDash();
                break;
            default:
                ApplyNoVerticalMovement();
                break;
        }

        CheckPosition();
    }

    private void CheckPosition()
    {
        if (this.y < 0)
        {
            levelList.LoadLevel();
        }
        else if (this.y > game.height)
        {
            Die();
        }
    }

    private void SetCurrentState(PlayerState state, float startingFallSpeed=-1f)
    {
        currentState = state;
        switch (currentState)
        {
            case PlayerState.Fall:
                if (startingFallSpeed != -1f)
                    currentFallSpeed = startingFallSpeed;
                break;
            case PlayerState.Jump:
                jumpTimeMSCounter = 0f;
                break;
            case PlayerState.WallSlide:
                break;
            case PlayerState.WallJump:
                wallJumpTimeMSCounter = 0f;
                break;
            case PlayerState.Dash:
                canDash = false;
                dashTimeMSCounter = 0f;
                dashDirection = tempDashDirection;
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
            SetCurrentState(PlayerState.Fall, fallSpeed);
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
        if (currentFallSpeed < fallSpeed)
        {
            currentFallSpeed += 0.5f;
        }
        else
        {
            currentFallSpeed = fallSpeed;
        }

        var coll = MoveUntilCollision(0, currentFallSpeed);
        if (coll != null)
        {
            if (CheckColl(coll)) return;
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
            SetCurrentState(PlayerState.Fall, fallSpeed);
            return;
        }

        var coll = MoveUntilCollision(0, amountToMove);

        //Stops the jump if an interference with a collider occurres
        if (coll != null)
        {
            if (CheckColl(coll)) return;
            SetCurrentState(PlayerState.Fall, fallSpeed);
        }
    }

    /// <summary>
    /// <para>Moves the player horizontally depending on the current input</para>
    /// Called every frame
    /// </summary>
    private void ApplyHorizontalMovement()
    {
        if (currentState == PlayerState.WallJump) return;

        Collision coll = null;
        bool isFacingRight = true;

        if (Input.GetKey(Key.A))
        {
            coll = MoveUntilCollision(-currentMoveSpeed, 0);
            isFacingRight = false;

            Animate(0.05f);
        }
        else if (Input.GetKey(Key.D))
        {
            coll = MoveUntilCollision(currentMoveSpeed, 0);
            isFacingRight = true;

            Animate(0.05f);
        }
        else
        {
            currentFrame = 1;
            SetCycle(1, 4);
        }

        if (currentState != PlayerState.Dash)
        {
            CheckForWallSliding(coll);
        }
        if (coll != null)
        {
            if (CheckColl(coll)) return;
            CheckForWallJump(isFacingRight);
        }
    }

    private void CheckForWallJump(bool isFacingRight)
    {
        if (Input.GetKeyDown(Key.SPACE) && !IsGrounded(fallSpeed))
        {
            SetCurrentState(PlayerState.WallJump);
            wallJumpDirection = isFacingRight ? WallJumpDirection.Left : WallJumpDirection.Right;
        }
    }

    /// <summary>
    /// Checks if wall sliding is available or if it is not anymore
    /// </summary>
    /// <param name="coll">Possible sideways collision with a wall(or null)</param>
    private void CheckForWallSliding(Collision coll)
    {
        if (coll != null && !IsGrounded(wallSlideSpeed))
        {
            if (CheckColl(coll)) return;
            SetCurrentState(PlayerState.WallSlide);
        }
        else if (currentState == PlayerState.WallSlide && coll == null)
        {
            SetCurrentState(PlayerState.Fall, wallSlideSpeed);
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
        {
            if (CheckColl(coll)) return;
            SetCurrentState(PlayerState.None);
        }

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
            SetCurrentState(PlayerState.Fall, fallSpeed * 0.2f);
            currentMoveSpeed = moveSpeed;
            return;
        }

        amountToDash.x *= dashDirection.x;
        amountToDash.y *= dashDirection.y;
        var collX = MoveUntilCollision(amountToDash.x, 0);
        var collY = MoveUntilCollision(0, amountToDash.y);

        if (collX != null && CheckColl(collX)) return;
        //Stops the dash if an interference with a collider occurres
        if (collY != null)
        {
            if (CheckColl(collY)) return;
            SetCurrentState(PlayerState.Fall, fallSpeed * 0.2f);
            currentMoveSpeed = moveSpeed;
        }
    }

    private void ApplyWallJump()
    {
        wallJumpTimeMSCounter += Time.deltaTime;
        WallJump();
    }

    private void WallJump()
    {
        if (wallJumpTimeMSCounter < wallJumpTimeMS)
        {
            Vector2 amountToWallJump = new Vector2(wallJumpAmount.x * (int)wallJumpDirection , wallJumpAmount.y);

            var collX = MoveUntilCollision(amountToWallJump.x, 0);
            var collY = MoveUntilCollision(0, amountToWallJump.y);

            if (collX != null && CheckColl(collX)) return;
            if (collY != null)
            {
                if (CheckColl(collY)) return;
                SetCurrentState(PlayerState.Fall, fallSpeed * 0.2f);
            }
        }
        else
        {
            SetCurrentState(PlayerState.Fall, fallSpeed * 0.2f);
        }
    }

    /// <summary>
    /// Checks the type of the object with which the collision occured
    /// </summary>
    /// <param name="coll">The collision that occured</param>
    /// <returns>true if level is reloaded, otherwise false</returns>
    private bool CheckColl(Collision coll)
    {
        if (coll.other is Spikes)
        {
            //TODO: fix player respawning twice
            //TODO: check what needs to be done to fix the game slowing down
            //after a number of times the Player dies - what needs to be done in addition to just deleting it?
            Die();
            return true;
        }
        return false;
    }

    private void Die()
    {
        levelList.CurrentLevel.RemoveChild(this);
        Destroy();
        levelList.CurrentLevel.ReloadLevel();
    }
}