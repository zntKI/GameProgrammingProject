using GXPEngine;
using GXPEngine.Core;
using System.Drawing;
using TiledMapParser;

public class Spikes : AnimationSprite
{
	public Spikes(string imageFile, int cols, int rows, TiledObject obj = null) : base(imageFile, cols, rows, addCollider:true)
	{
    }
    
    protected override Collider createCollider()
    {
        Canvas hitbox = new Canvas(4, 3, false);
        hitbox.y = 4;
        hitbox.SetOrigin(1.5f, 3);
        //hitbox.graphics.Clear(Color.Red);
        AddChild(hitbox);
        return new BoxCollider(hitbox);
    }
    
}
