using GXPEngine;
using System;

public class Particle : Sprite
{
    protected float speed;

    public Particle(string filename, float speed) : base(filename, true, false)
    { 
        this.speed = speed;
    }

    public Particle SetPos(float x, float y)
    {
        SetXY(x, y);
        return this;
    }

    public Particle SetScale(float scaleX, float scaleY)
    {
        SetScaleXY(scaleX, scaleY);
        return this;
    }

    private void Update()
    {
        MoveHorizontally();
    }

    protected void MoveHorizontally()
    {
        x += speed;

        if (x > game.width)
        {
            //Spawn new particle of the same kind
            RespawnParticle();
        }
    }

    private void RespawnParticle()
    {
        if (this is SnowParticle)
        {
            ((SnowParticle)this)
                .SetScale(Utils.Random(1, 5))
                .SetPos(0 - this.width, Utils.Random(0, game.height - this.height));
        }
        else
        {
            this
                .SetScale(Utils.Random(8, 31), Utils.Random(3, 16))
                .SetPos(0 - this.width, Utils.Random(0, game.height - this.height));
        }
    }
}
