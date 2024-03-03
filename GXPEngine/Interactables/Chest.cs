using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

public class Chest : AnimationSprite
{
    public Chest(string filename, int cols, int rows, TiledObject obj=null) : base(filename, cols, rows)
    {
    }

    public void Destruct()
    {
        //Load strawberry
        Strawberry str = new Strawberry("Levels/strawberry.png", 1, 1);
        this.parent.AddChild(str);
        str.SetOrigin(str.width / 2, str.height / 2);
        str.SetXY(this.x, this.y);

        Destroy();
    }
}