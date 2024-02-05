using GXPEngine;
using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

public class FlyingStrawberry : Strawberry
{
    private byte animDelay;

    private bool shouldDissappear;

    public FlyingStrawberry(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows, obj)
    {
        animDelay = 10;
        SetCycle(0, 3, animDelay);

        shouldDissappear = false;
    }

    protected override Collider createCollider()
    {
        Canvas hitbox = new Canvas(4, 4, false);
        hitbox.y = 2;
        hitbox.SetOrigin(2, 2);
        //hitbox.graphics.Clear(Color.Yellow);
        AddChild(hitbox);
        return new BoxCollider(hitbox);
    }

    private void Update()
    {
        //Checks if the player dashes
        if (Input.GetKeyDown(Key.LEFT_SHIFT) || Input.GetKeyDown(Key.X))
        {
            shouldDissappear = true;
        }

        if (!shouldDissappear)
            MoveVertically();
        //Progressively move the strawberry out of the screen
        else
        {
            y -= 1f;
            if (y < 0f)
                Destroy();
        }

        Animate();
    }

    private void MoveVertically()
    {
        if (Time.time % 9 == 0)
            y += Mathf.Sin(Time.time);
    }
}
