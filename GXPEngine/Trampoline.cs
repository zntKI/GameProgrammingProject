using GXPEngine;
using TiledMapParser;

public class Trampoline : AnimationSprite
{
    private float suppressedDurationMS;
    private float suppressedTimeCounterMS;


    public Trampoline(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows)
	{
        //TODO: fix the bounds of the collider
    }

    private void Update()
    {
        if (suppressedTimeCounterMS < suppressedDurationMS)
            suppressedTimeCounterMS += Time.deltaTime;
        else
            SetCycle(18);

        Animate();
    }

    public void DoAnimation(float suppressedDurationMS)
    {
        this.suppressedDurationMS = suppressedDurationMS;
        suppressedTimeCounterMS = 0f;
        SetCycle(19);
    }
}
