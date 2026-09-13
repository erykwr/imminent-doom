using Godot;

namespace imminent_doom.hero;

public partial class AttackState : State
{
	private readonly StringName _animationName = "Attack";
	
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

}
