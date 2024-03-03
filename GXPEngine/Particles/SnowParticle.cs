using GXPEngine;

public class SnowParticle : Particle
{
    private float yMoveAmount;

    public SnowParticle(string filename, float speed, float yMoveAmount) : base(filename, speed)
    {
        this.yMoveAmount = yMoveAmount;
    }

    public Particle SetScale(float scale)
    {
        SetScaleXY(scale);
        return this;
    }

    private void Update()
    {
        MoveHorizontally();
        MoveVertically();
    }

    private void MoveVertically()
    {
        if (Time.time % 10 == 0)
            y += Mathf.Sin(Time.time) * yMoveAmount;
    }
}
