using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

public class ChestKey : AnimationSprite
{
    private int startFrame;
    private byte animDelay;

    private Chest chestToOpen;

    public ChestKey(string filename, int cols, int rows, TiledObject obj = null) : base(filename, cols, rows)
    {
        collider.isTrigger = true;

        startFrame = 8;
        animDelay = 30;
        SetCycle(startFrame, 3, animDelay);
    }

    private void Update()
    {
        if (chestToOpen == null)
            chestToOpen = this.parent.FindObjectOfType<Chest>();

        Animate();
    }

    public void OpenChest()
    {
        chestToOpen.Destruct();

        Destroy();
    }
}