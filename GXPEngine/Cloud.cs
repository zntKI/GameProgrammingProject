using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

public class Cloud : AnimationSprite
{
	private bool isGoingRight;

	public Cloud(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows)
    {
        SetOrigin(0, 0);
        isGoingRight = obj.GetBoolProperty("isGoingRight");
	}

	private void Update()
	{
		if (Time.time % 3 == 0){
            if (isGoingRight)
            {
                x += 1;
                if (x >= game.width + width / 2)
                {
                    x = 0 - width;
                }
            }
            else
            {
                x -= 1;
                if (x <= 0 - width)
                {
                    x = game.width;
                }
            }
        }
	}
}
