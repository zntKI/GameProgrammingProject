using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

public class Strawberry : AnimationSprite
{
	public Strawberry(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows, addCollider: true)
    {
		collider.isTrigger = true;
	}
}
