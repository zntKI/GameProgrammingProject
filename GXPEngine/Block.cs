using GXPEngine;
using TiledMapParser;

public class Block : AnimationSprite
{
	public Block(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows)
    {
	}
}