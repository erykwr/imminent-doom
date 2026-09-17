using Godot;
using imminent_doom.common;

namespace imminent_doom.levels;

public partial class KillNode : Node
{
	[Export] private CharacterBody3D _characterBody3D;
	
	private Health _health;
	
	public override void _Ready()
	{
		_health = _characterBody3D?.GetNode<Health>("Health");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("debug"))
		{
			_health?.TakeDamage(2.0f, this);
			GD.Print("Applying Damage");
		}
	}

}
