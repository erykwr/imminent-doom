using Godot;
using imminent_doom.common;
using imminent_doom.levels;
using imminent_doom.skull;

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
		
		Vector2 inputDir = Input.GetVector(
			"ui_left",
			"ui_right",
			"ui_up",
			"ui_down"
		);

		Vector3 camForward = -Camera.GlobalTransform.Basis.Z;
		Vector3 camRight = Camera.GlobalTransform.Basis.X;
		camForward.Y = 0;
		camForward = camForward.Normalized();
		camRight.Y = 0;
		camRight = camRight.Normalized();

		Vector3 direction = (camRight * inputDir.X + camForward * -inputDir.Y);
		if (direction.Length() > 0.01f)
		{
			direction = direction.Normalized();
			float targetAngle = Mathf.Atan2(direction.X, direction.Z);
			CharacterBody3D.Rotation = new Vector3(
				CharacterBody3D.Rotation.X,
				Mathf.LerpAngle(CharacterBody3D.Rotation.Y, targetAngle, RotationSpeed * delta),
				CharacterBody3D.Rotation.Z
			);
		}
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
		Juice.HitStop(GetTree());
		(node as Skull)?.Flash();
		health.TakeDamage(10.0f, CharacterBody3D);
		
		if (_hasShaken) return;
		_hasShaken = true;
		(Camera as CopyPosition)?.Shake();
	}
	
}
