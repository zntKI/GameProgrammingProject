using GXPEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

public class Flag : AnimationSprite
{
    private int startFrame;
    private byte animDelay;

    public Flag(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows)
    {
		collider.isTrigger = true;

        startFrame = 118;
        animDelay = 10;
        SetCycle(startFrame, 3, animDelay);
	}

    private void Update()
    {
        Animate();
    }
}
