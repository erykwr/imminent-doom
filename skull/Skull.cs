using Godot;

namespace imminent_doom.skull;

public partial class Skull : Node3D
{
	public override void _Ready()
	{
		var animationPlayer =  GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("Idle");
	}

}
