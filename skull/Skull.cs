using Godot;

namespace imminent_doom.skull;

public partial class Skull : CharacterBody3D
{
	[Export] public CollisionShape3D CollisionShape3D;

	public override void _Ready()
	{
		CollisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
	}
	
	private void Die()
	{
		SetPhysicsProcess(false);
		CollisionShape3D?.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);

		var tween = CreateTween();
		tween.TweenProperty(this, "scale", Scale * 0.05f, 0.3f);
		tween.TweenCallback(Callable.From(QueueFree));
	}

}
