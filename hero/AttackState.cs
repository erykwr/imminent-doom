using Godot;
using imminent_doom.common;

namespace imminent_doom.hero;

public partial class AttackState : State
{
	private readonly StringName _animationName = "Attack";
	
	[Export] public Area3D Hitbox { get; set; }
	
	public override void Enter()
	{
		Playback?.Travel(_animationName);
	}

	public override void HandleInput(InputEvent @event)
	{
		if (Input.IsActionJustReleased("attack"))
		{
			EmitSignal(State.SignalName.Transitioned, this, "Idle");
		}
	}

	public override void UpdateMovement(float delta)
	{
	}

	public override void Exit()
	{
		Playback?.Travel("BlendSpace1D");
	}

	public void HitStarted()
	{
		if (Hitbox != null)
		{
			Hitbox.Monitoring = true;
		}
	}

	public void HitFinished()
	{
		if (Hitbox != null)
		{
			Hitbox.Monitoring = false;
		}
	}

	public void OnBodyEntered(Node3D node)
	{
		Health health = node.GetNode<Health>("Health");
		if (health != null)
		{
			health.TakeDamage(10.0f, CharacterBody3D);
		}
	}
	
}
