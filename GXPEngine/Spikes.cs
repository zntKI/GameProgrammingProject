﻿using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

public class Spikes : AnimationSprite
{
	public Spikes(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows)
	{
		//TODO: fix the bounds of the collider so that its more fair
	}
}