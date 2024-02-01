using GXPEngine;
using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

public class Balloon : AnimationSprite
{
    public bool IsMarkedAsDestroyed => isMarkedAsDestroyed;

    private bool isMarkedAsDestroyed = false;
    private int timeToStayDestroyedMS = 2000;
    private int timeToStayDestroyedTimerMS = 0;

    private byte animDelay = 30;

    public Balloon(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows)
    {
        collider.isTrigger = true;
		SetCycle(1, 3, animDelay);
    }

    protected override Collider createCollider()
    {
        Canvas hitbox = new Canvas(4, 4, false);
        hitbox.y = 1;
        hitbox.SetOrigin(2, 6);
        //hitbox.graphics.Clear(Color.Yellow);
        AddChild(hitbox);
        return new BoxCollider(hitbox);
    }

    private void Update()
	{
        if (isMarkedAsDestroyed)
        {
            timeToStayDestroyedTimerMS += Time.deltaTime;

            if (timeToStayDestroyedTimerMS > timeToStayDestroyedMS)
            {
                isMarkedAsDestroyed = false;
                SetCycle(1, 3, animDelay);

                new Sound("Sounds/block_respawn.wav").Play();
            }
        }

        MoveVertically();
        Animate();
	}

	private void MoveVertically()
	{
        if (Time.time % 20 == 0)
            y += Mathf.Sin(Time.time);
	}

    public void Destruct()
    {
        isMarkedAsDestroyed = true;
        timeToStayDestroyedTimerMS = 0;
        SetCycle(0);
    }
}
