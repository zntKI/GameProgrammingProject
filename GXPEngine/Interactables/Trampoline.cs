using GXPEngine;
using GXPEngine.Core;
using System.Drawing;
using TiledMapParser;

public class Trampoline : AnimationSprite
{
    private int startFrame;
    private int suppressedFrame;

    private float suppressedDurationMS;
    private float suppressedTimeCounterMS;

    public Trampoline(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows)
	{
        startFrame = 18;
        suppressedFrame = 19;
    }

    protected override Collider createCollider()
    {
        Canvas hitbox = new Canvas(8, 5, false);
        hitbox.y = 4;
        hitbox.SetOrigin(4, 4);
        //hitbox.graphics.Clear(Color.Red);
        AddChild(hitbox);
        return new BoxCollider(hitbox);
    }

    private void Update()
    {
        if (suppressedTimeCounterMS < suppressedDurationMS)
            suppressedTimeCounterMS += Time.deltaTime;
        else
            SetCycle(startFrame);

        Animate();
    }

    /// <summary>
    /// Sets the current frame to be the suppressed one
    /// </summary>
    /// <param name="suppressedDurationMS">How long should it be surppressed</param>
    public void DoAnimation(float suppressedDurationMS)
    {
        this.suppressedDurationMS = suppressedDurationMS;
        suppressedTimeCounterMS = 0f;
        SetCycle(suppressedFrame);
    }
}
