using GXPEngine;
using System;
using TiledMapParser;

public class Block : Destructable
{
    public bool ShouldDestruct => shouldDestruct;
    private bool shouldDestruct = false;

    private int startFrame;

    //Time between destruction frames, switched in HandleDestructAnimations method
    private int timeBetweenFramesMS;
	private int frameSwitchTimer;

    public Block(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows)
    {
        startFrame = 23;
        SetCycle(startFrame);

        timeBetweenFramesMS = 200;
        frameSwitchTimer = 0;
    }

	private void Update()
	{
		if (shouldDestruct)
        {
            frameSwitchTimer += Time.deltaTime;
            HandleDestructAnimations();

            Animate();
        }
        else if (isMarkedAsDestroyed)
        {
            StayDestroyed();
        }
	}

    /// <summary>
    /// Changes frames accordingly, while the object is being destroyed
    /// </summary>
    private void HandleDestructAnimations()
    {
        if (frameSwitchTimer < timeBetweenFramesMS)
        {
            SetCycle(startFrame);
        }
        else if (frameSwitchTimer > timeBetweenFramesMS && frameSwitchTimer < timeBetweenFramesMS * 2)
        {
            SetCycle(startFrame + 1);
        }
        else if (frameSwitchTimer > timeBetweenFramesMS * 2 && frameSwitchTimer < timeBetweenFramesMS * 3)
        {
            SetCycle(startFrame + 2);
        }
        else if (frameSwitchTimer > timeBetweenFramesMS * 3)
        {
            shouldDestruct = false;

            isMarkedAsDestroyed = true;
            timeToStayDestroyedTimerMS = 0;
            SetCycle(0);
        }
    }

    protected override void StayDestroyed()
    {
        timeToStayDestroyedTimerMS += Time.deltaTime;

        if (timeToStayDestroyedTimerMS > timeToStayDestroyedMS)
        {
            isMarkedAsDestroyed = false;
            SetCycle(startFrame);

            new Sound("Sounds/block_respawn.wav").Play(false, 0, 0.5f);
        }
    }

    public override void Destruct()
	{
        shouldDestruct = true;
        frameSwitchTimer = 0;
    }
}