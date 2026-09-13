using Godot;

namespace imminent_doom.state_machine;

public partial class State : Node
{
	[Signal] public delegate void TransitionedEventHandler(State state, string newStateName);

	public virtual void Enter() { }
	public virtual void Exit() { }
	public virtual void HandleInput(InputEvent @event) { }
	public virtual void PhysicsUpdate(double delta) { }
}
