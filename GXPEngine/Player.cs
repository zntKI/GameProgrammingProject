using System;
using System.Drawing;
using System.Linq;
using GXPEngine;
using GXPEngine.Core;
using TiledMapParser;

public class Player : AnimationSprite
{
    private enum PlayerState { None, Fall, Jump, WallSlide, WallJump, Dash, Bounce, OnCloud }
    PlayerState currentState;

    private int durationToNotMove;
    private int durationToNotMoveCounter;

    public bool ShouldDie => shouldDie;
    private bool shouldDie;

    //Vertical movement variables
    private float fallSpeed;
    private float currentFallSpeed;
    private float fallSpeedIncrement;
    private float currentFallSpeedIncrement;
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

    private Vector2 dashDistance;

    private float dashTimeMS;
    private float dashAirTimeMS;
    private float dashTimeMSCounter;

    private Vector2 dashAmount;

    //Wall jump variables
    private Vector2 wallJumpDistance;

    private float wallJumpTimeMS;
    private float wallJumpAirTimeMS;
    private float wallJumpTimeMSCounter;

    private Vector2 wallJumpAmount;

    private enum WallJumpDirection { Left = -1, Right = 1 }
    private WallJumpDirection wallJumpDirection;

    //Bounce variables
    private float bounceHeight;
    private float bounceHeightFirstSection;
    private float bounceHeightSecondSection;

    private float bounceTimeMS;
    private float bounceTimeSectionMS;
    private float bounceTimeCounterMS;

    private float bounceAmountFirstSection;
    private float bounceAmountSecondSection;

    public Player(string imageFile, int cols, int rows, TiledObject obj=null) : base(imageFile, cols, rows)
    {
        this.SetOrigin(width / 2, height / 2);

        new Sound("Sounds/respawn.wav").Play(false, 0, 0.5f);

        InitVariables();
    }

    protected override Collider createCollider()
    {
        Canvas hitbox = new Canvas(4, 4, false);
        hitbox.y = 2;
        hitbox.SetOrigin(2, 2);
        hitbox.graphics.Clear(Color.Red);
        AddChild(hitbox);
        return new BoxCollider(hitbox);
    }

    private void InitVariables()
    {
        SetCurrentState(PlayerState.Fall);

        durationToNotMove = 200;
        durationToNotMoveCounter = 0;

        shouldDie = false;

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
        fallSpeedIncrement = 0.08f;
        currentFallSpeedIncrement = fallSpeedIncrement;

        wallSlideSpeed = fallSpeed * 0.5f;

        //Horizontal movement variables
        moveSpeed = 1f;
        currentMoveSpeed = moveSpeed;

        //Dash variables
        canDash = true;

        dashDirection = new Vector2();
        tempDashDirection = new Vector2();

        dashDistance = new Vector2(game.width / 16 * 4f, game.height / 16 * 3.5f);

        dashTimeMS = 225;
        dashAirTimeMS = 120;
        dashTimeMSCounter = 0f;

        frames = 60 * (dashTimeMS / 1000f);

        dashAmount = new Vector2(dashDistance.x / frames, dashDistance.y / frames);

        //WallJump variables
        wallJumpDistance = new Vector2(game.width / 16 * 3.2f, (game.height / 16 * 2.4f) * -1f);

        wallJumpTimeMS = 300f;
        wallJumpAirTimeMS = 150f;
        wallJumpTimeMSCounter = 0f;

        frames = 60 * (wallJumpTimeMS / 1000f);

        wallJumpAmount = new Vector2(wallJumpDistance.x / frames, wallJumpDistance.y / frames);

        wallJumpDirection = WallJumpDirection.Left;

        //Bounce variables
        bounceHeight = game.height / 16 * 4.5f;
        bounceHeightFirstSection = game.height / 16 * 3.5f;
        bounceHeightSecondSection = bounceHeight - bounceHeightFirstSection;

        bounceTimeMS = 900f;
        bounceTimeSectionMS = bounceTimeMS / 4f;
        bounceTimeCounterMS = 0f;

        frames = 60 * (bounceTimeSectionMS / 1000f);

        bounceAmountFirstSection = bounceHeightFirstSection / frames;
        bounceAmountSecondSection = bounceHeightSecondSection / frames;
    }

    private void Update()
    {
        if (durationToNotMoveCounter < durationToNotMove)
        {
            durationToNotMoveCounter += Time.deltaTime;
            return;
        }

        CheckForInput();
        HandleCurrentState();

        ApplyHorizontalMovement();

        Animate();
    }

    private void CheckForInput()
    {
        CheckInputForDirection();

        if (Input.GetKeyDown(Key.SPACE) || Input.GetKeyDown(Key.C))
        {
            if (currentState == PlayerState.None)
            {
                SetCurrentState(PlayerState.Jump);
            }
            else
            {
                var result = IsWalled();
                if (result.Item1) SetCurrentState(PlayerState.WallJump, result.Item2);
            }
        }

        if ((Input.GetKeyDown(Key.LEFT_SHIFT) || Input.GetKeyDown(Key.X)) && canDash)
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
            Mirror(true, false);

            tempDashDirection.x = -1;
        }
        else if (Input.GetKeyDown(Key.D))
        {
            Mirror(false, false);

            tempDashDirection.x = 1;
        }

        if (Input.GetKey(Key.W))
        {
            tempDashDirection.y = -1;
            if (!Input.GetKey(Key.A) && !Input.GetKey(Key.D))
            {
                tempDashDirection.x = 0;
            }
        }
        else if (Input.GetKey(Key.S))
        {
            tempDashDirection.y = 1;
            if (!Input.GetKey(Key.A) && !Input.GetKey(Key.D))
            {
                tempDashDirection.x = 0;
            }
        }
        else
        {
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
            case PlayerState.Bounce:
                ApplyBounce();
                break;
            case PlayerState.None:
                ApplyNoVerticalMovement();
                break;
            default:
                //Console.WriteLine("OnCloud!");
                break;
        }

        CheckPosition();
    }

    private void CheckPosition()
    {
        Vector2 globalPos = TransformPoint(0, 0);
        if (globalPos.x - width / 2 < 0)
        {
            globalPos.x = 0 + width / 2;

            Vector2 newLocal = parent.InverseTransformPoint(globalPos.x, globalPos.y);
            this.x = newLocal.x;
        }
        else if (this.x + width / 2 > game.width)
        {
            globalPos.x = game.width - width / 2;

            Vector2 newLocal = parent.InverseTransformPoint(globalPos.x, globalPos.y);
            this.x = newLocal.x;
        }
    }

    private void SetCurrentState(PlayerState state, params float[] extraParams)
    {
        currentState = state;
        switch (currentState)
        {
            case PlayerState.Fall:
                if (extraParams.Length > 0)
                    currentFallSpeed = extraParams[0];

                if (extraParams.Length > 1)
                    currentFallSpeedIncrement = extraParams[1];
                else
                    currentFallSpeedIncrement = this.fallSpeedIncrement;

                SetCycle(3);
                break;
            case PlayerState.Jump:
                jumpTimeMSCounter = 0f;

                SetCycle(3);
                new Sound("Sounds/jump.wav").Play();
                break;
            case PlayerState.WallSlide:
                SetCycle(5);
                break;
            case PlayerState.WallJump:
                if (extraParams.Length == 0)
                    throw new ArgumentException("Cannot set the state to wall jumping without extra param", nameof(extraParams));

                if (extraParams[0] == 1)
                {
                    wallJumpDirection = _mirrorX ? WallJumpDirection.Right : WallJumpDirection.Left;
                    Mirror(!_mirrorX, false);
                }
                else
                {
                    wallJumpDirection = _mirrorX ? WallJumpDirection.Left : WallJumpDirection.Right;
                }

                wallJumpTimeMSCounter = 0f;

                SetCycle(3);
                new Sound("Sounds/walljump.wav").Play();
                break;
            case PlayerState.Dash:
                canDash = false;
                dashTimeMSCounter = 0f;
                dashDirection = tempDashDirection;

                SetCycle(3);
                new Sound("Sounds/dash.wav").Play(false, 0, 0.5f);
                break;
            case PlayerState.Bounce:
                canDash = true;
                bounceTimeCounterMS = 0f;

                SetCycle(3);
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
            SetCurrentState(PlayerState.Fall, fallSpeed * 0.2f);
        }
        else
        {
            canDash = true;
            CheckColl(MoveUntilCollision(0f, fallSpeed));
        }
    }

    /// <summary>
    /// <para>Adding a specified amount to the player's Y coordinate, simulating the effect of gravity</para>
    /// Called when the current state is set to Fall
    /// </summary>
    private void ApplyVerticalMovement()
    {
        //Lerp?
        if (currentFallSpeed < fallSpeed)
        {
            currentFallSpeed += currentFallSpeedIncrement;
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
        if (Input.GetKey(Key.A))
        {
            coll = MoveUntilCollision(-currentMoveSpeed, 0);
            SetCycle(1, 4, 20);
        }
        else if (Input.GetKey(Key.D))
        {
            coll = MoveUntilCollision(currentMoveSpeed, 0);
            SetCycle(1, 4, 20);
        }
        else
        {
            if (Input.GetKey(Key.W) && currentState != PlayerState.Dash)
                SetCycle(7);
            else if (Input.GetKey(Key.S) && currentState != PlayerState.Dash)
                SetCycle(6);
            else if (currentState == PlayerState.None)
                SetCycle(1);
        }

        CheckForWallSliding(coll);

        if (coll != null)
            CheckColl(coll);
    }

    /// <summary>
    /// Checks if wall sliding is available or if it is not anymore
    /// </summary>
    /// <param name="coll">Possible sideways collision with a wall(or null)</param>
    private void CheckForWallSliding(Collision coll)
    {
        if (currentState == PlayerState.Dash || currentState == PlayerState.Bounce)
            return;

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

    private (bool, int) IsWalled()
    {
        var leftColl = GetFutureCollisions(-moveSpeed * 2, 0).FirstOrDefault(obj => obj.x < this.x);
        var rightColl = GetFutureCollisions(moveSpeed * 2, 0).FirstOrDefault(obj => obj.x > this.x);

        if (leftColl != null)
        {
            return _mirrorX ? (true, 1) : (true, 0);
        }
        else if (rightColl != null)
        {
            return _mirrorX ? (true, 0) : (true, 1);
        }

        return (false, 0);
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
        //Set the x and y of the dash separately so that the y value can be tampered with by applying gravity

        //Code depending on the last registered facing direction before the dash
        Vector2 amountToDash;
        if (dashTimeMSCounter < dashTimeMS)
        {
            amountToDash = new Vector2(dashAmount.x, dashAmount.y);
            currentMoveSpeed = moveSpeed * 0.1f;
        }
        else if (dashTimeMSCounter < dashTimeMS + dashAirTimeMS)
        {
            amountToDash = new Vector2();
            if (Input.GetKey(Key.A) || Input.GetKey(Key.D))
            {
                SetCurrentState(PlayerState.Fall, fallSpeed * 0.01f, currentFallSpeedIncrement * 0.8f);
                currentMoveSpeed = moveSpeed;
                return;
            }
        }
        else
        {
            SetCurrentState(PlayerState.Fall, fallSpeed * 0.1f, currentFallSpeedIncrement * 0.8f);
            currentMoveSpeed = moveSpeed;
            return;
        }

        amountToDash.x *= dashDirection.x;
        amountToDash.y *= dashDirection.y;
        var collX = MoveUntilCollision(amountToDash.x, 0);
        var collY = MoveUntilCollision(0, amountToDash.y);

        if (collX != null && CheckColl(collX))
            return;
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
        else if (wallJumpTimeMSCounter < wallJumpTimeMS + wallJumpAirTimeMS)
        {
            if (Input.GetKey(Key.A) || Input.GetKey(Key.D))
            {
                wallJumpTimeMSCounter = wallJumpTimeMS + wallJumpAirTimeMS;
            }
        }
        else
        {
            SetCurrentState(PlayerState.Fall, fallSpeed * 0.01f);
            if (Input.GetKey(Key.A))
                Mirror(true, false);
            else if (Input.GetKey(Key.D))
                Mirror(false, false);
        }
    }

    private void ApplyBounce()
    {
        bounceTimeCounterMS += Time.deltaTime;
        Bounce();
    }

    private void Bounce()
    {
        float amountToBounce;
        if (bounceTimeCounterMS < bounceTimeSectionMS)
        {
            amountToBounce = -bounceAmountFirstSection;
        }
        else if (bounceTimeCounterMS < bounceTimeSectionMS * 2)
        {
            amountToBounce = -bounceAmountSecondSection;
        }
        else if (bounceTimeCounterMS < bounceTimeSectionMS * 3)
        {
            amountToBounce = bounceAmountSecondSection;
        }
        else if (bounceTimeCounterMS < bounceTimeMS)
        {
            amountToBounce = bounceAmountFirstSection;
        }
        else
        {
            SetCurrentState(PlayerState.Fall, fallSpeed);
            return;
        }

        var collY = MoveUntilCollision(0, amountToBounce);

        if (collY != null)
        {
            if (CheckColl(collY)) return;
            SetCurrentState(PlayerState.Fall, fallSpeed);
        }
    }

    /// <summary>
    /// Checks the type of the object with which the collision occured
    /// </summary>
    /// <param name="coll">The collision that occured</param>
    /// <returns>true if code should process it as normal collider, otherwise false</returns>
    private bool CheckColl(Collision coll)
    {
        if (coll.other is Spikes)
        {
            shouldDie = true;
            return true;
        }
        else if (coll.other is Trampoline)
        {
            SetCurrentState(PlayerState.Bounce);
            ((Trampoline)coll.other).DoAnimation(bounceTimeSectionMS);

            new Sound("Sounds/die1.wav").Play();

            return true;
        }
        else if (coll.other is Block && (coll.normal.y == -1 || coll.normal.x == -1 || coll.normal.x == 1)
            && !((Block)coll.other).ShouldDestruct)
        {
            ((Block)coll.other).Destruct();

            new Sound("Sounds/crumbling.wav").Play();
        }
        else if (coll.other is Balloon && !canDash)
        {
            //TODO: Make trigger
            canDash = true;
            ((Balloon)coll.other).Destruct();

            new Sound("Sounds/balloon_collect.wav").Play();

            return true;
        }
        else if (coll.other is Cloud)
        {
            //Sets the Player a child of Cloud
            coll.other.AddChild(this);
            x = x - coll.other.x;
            y = -height;
            SetCurrentState(PlayerState.None);
            return true;
        }
        return false;
    }

    //private void Die()
    //{
    //    if (parent == null) return;

    //    new Sound("Sounds/die1.wav").Play(false, 0, 0.5f);

    //    Destroy();
    //    levelList.CurrentLevel.ReloadLevel();
    //}
}