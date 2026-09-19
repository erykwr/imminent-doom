using Godot;

namespace imminent_doom.skull;

public partial class Skull : CharacterBody3D
{
	[Export] public CollisionShape3D CollisionShape3D;
	[Export] public CharacterBody3D Player;
	
	public override void _Ready()
	{
		CollisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
		if (Player == null)
		{
			GD.Print("Player not set for enemy: " + Name);
		}

		foreach (Node child in GetNode("StateMachine").GetChildren())
		{
			if (child is EnemyState state)
			{
				state.Player = Player;
			}
		}
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
