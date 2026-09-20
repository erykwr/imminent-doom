using Godot;
using imminent_doom.common;
using imminent_doom.levels;

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
		Hitbox?.SetMonitoring(false);
	}
	
	private bool _hasShaken;

	private void HitStarted()
	{
		_hasShaken = false;
		Hitbox?.SetMonitoring(true);
	}

	private void HitFinished()
	{
		Hitbox?.SetMonitoring(false);
	}

	private void OnBodyEntered(Node3D node)
	{
		Health health = node.GetNode<Health>("Health");
		if (health == null) return;
		Juice.HitStop(GetTree(),0.05f, 0.2f);
		health.TakeDamage(10.0f, CharacterBody3D);
		
		if (_hasShaken) return;
		_hasShaken = true;
		(Camera as CopyPosition)?.Shake();
	}
	
}
