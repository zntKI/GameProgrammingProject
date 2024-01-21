using GXPEngine;
using System;
using TiledMapParser;

public class Block : AnimationSprite
{
    public bool ShouldDestruct => shouldDestruct;
    public bool IsMarkedAsDestroyed => isMarkedAsDestroyed;

    private int startFrame = 23;

	private bool shouldDestruct = false;
	private bool isMarkedAsDestroyed = false;

	private int timeBetweenFramesMS = 200;
	private int frameSwitchTimer = 0;

    private int timeToStayDestroyedMS = 2000;
    private int timeToStayDestroyedTimerMS = 0;


    public Block(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows)
    {
        //TODO: Fix blocks not working after dying once
        SetCycle(startFrame);
    }

	private void Update()
	{
		if (shouldDestruct)
        {
            frameSwitchTimer += Time.deltaTime;

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

            Animate();
        }
        else if (isMarkedAsDestroyed)
        {
            timeToStayDestroyedTimerMS += Time.deltaTime;

            if (timeToStayDestroyedTimerMS > timeToStayDestroyedMS)
            {
                isMarkedAsDestroyed = false;
                SetCycle(startFrame);
            }
            else
            {
                SetCycle(0);
            }
        }
	}

	public void Destruct()
	{
        shouldDestruct = true;
        frameSwitchTimer = 0;
    }
}