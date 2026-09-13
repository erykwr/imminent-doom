using Godot;

namespace imminent_doom.hero;

public partial class RunState : State
{
	
	public override void Enter()
	{
		AnimationPlayer?.Play("Run");
	}

	public override void HandleInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("dash"))
		{
			EmitSignal(State.SignalName.Transitioned, this, "Dash");
		}
		else if (Input.IsActionJustPressed("attack"))
		{
			EmitSignal(State.SignalName.Transitioned, this, "Attack");
		}
	}
	
	public override void PhysicsUpdate(float delta)
	{
		if (!IsMoving())
		{
			EmitSignal(State.SignalName.Transitioned, this, "Idle");
		}
	}

}
