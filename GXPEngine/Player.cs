using System;
using System.Linq;
using GXPEngine;
using GXPEngine.Core;

public class Player : Sprite
{
    private enum PlayerState { None, Fall, Jump, WallSlide, WallJump, Dash }
    PlayerState currentState;

    private Vector2 facingDirection;

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
        facingDirection = new Vector2();

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
        if (Input.GetKey(Key.W))
        {
            facingDirection.y = -1;
        }
        else if (Input.GetKey(Key.S))
        {
            facingDirection.y = 1;
        }
        else
        {
            facingDirection.y = 0;
        }

        if (Input.GetKeyDown(Key.LEFT_SHIFT) && canDash)
        {
            SetCurrentState(PlayerState.Dash);
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
                dashDirection = facingDirection;
                break;
            default:
                break;
        }
    }

    private void ApplyNoVerticalMovement()
    {
        if (!IsGrounded(0, fallSpeed))
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

    private void ApplyVerticalMovement()
    {
        var coll = MoveUntilCollision(0, fallSpeed);
        if (coll != null)
        {
            SetCurrentState(PlayerState.None);
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
            SetCurrentState(PlayerState.Fall);
            return;
        }

        var coll = MoveUntilCollision(0, amountToMove);

        if (coll != null)
        {
            SetCurrentState(PlayerState.Fall);
        }
    }

    private void ApplyHorizontalMovement()
    {
        Collision coll = null;

        if (Input.GetKey(Key.A))
        {
            coll = MoveUntilCollision(-currentMoveSpeed, 0);
            facingDirection.x = -1;
        }
        else if (Input.GetKey(Key.D))
        {
            coll = MoveUntilCollision(currentMoveSpeed, 0);
            facingDirection.x = 1;
        }
        else
        {
            //Fix bug with dashing direction
            facingDirection.x = 0;
        }

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
            SetCurrentState(PlayerState.Fall);
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
            SetCurrentState(PlayerState.Fall);

        //Console.WriteLine("Wall sliding!");
    }

    private void ApplyDash()
    {
        dashTimeMSCounter += Time.deltaTime;
        Dash();
    }

    private void Dash()
    {
        //Code depending on the last registered facing direction before the dash
        Vector2 amountToDash;
        if (dashTimeMSCounter < dashTimeSectionMS)
        {
            amountToDash = new Vector2(dashAmountFirstSection.x, dashAmountFirstSection.y);
            currentMoveSpeed = moveSpeed * 0.1f;
        }
        //else if (dashTimeMSCounter < dashTimeMS)
        //{
        //    amountToDash = new Vector2(dashAmountSecondSection.x, dashAmountSecondSection.y);
        //    currentMoveSpeed = moveSpeed * 0.5f;
        //}
        else
        {
            SetCurrentState(PlayerState.Fall);
            currentMoveSpeed = moveSpeed;
            return;
        }

        amountToDash.x *= dashDirection.x;
        amountToDash.y *= dashDirection.y;
        var coll = MoveUntilCollision(amountToDash.x, amountToDash.y);

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