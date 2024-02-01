using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

/// <summary>
/// Type of objects that can be destroyed and then created again
/// </summary>
public abstract class Destructable : AnimationSprite
{
    public bool IsMarkedAsDestroyed => isMarkedAsDestroyed;
    protected bool isMarkedAsDestroyed = false;

    protected int timeToStayDestroyedMS;
    protected int timeToStayDestroyedTimerMS;

    protected Destructable(string filename, int cols, int rows) : base(filename, cols, rows)
    {
        timeToStayDestroyedMS = 2000;
        timeToStayDestroyedTimerMS = 0;
    }

    /// <summary>
    /// Marks the object destroyed
    /// </summary>
    public abstract void Destruct();

    /// <summary>
    /// Counts the time that the objects need to stay destroyed
    /// </summary>
    protected abstract void StayDestroyed();
}