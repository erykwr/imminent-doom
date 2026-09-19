using Godot;
using Godot.Collections;

namespace imminent_doom.hero;

public partial class StateMachine : Node
{
	[Export] public NodePath InitialStatePath;

	private State _currentState;
	private readonly Dictionary<string, State> _states = new();

	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is State state)
			{
				_states[state.Name] = state;
				state.Transitioned += OnChildTransitioned;
			}
		}

		_currentState = GetNode<State>(InitialStatePath);
		_currentState.Enter();
	}

	public override void _UnhandledInput(InputEvent @event) => _currentState?.HandleInput(@event);

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float) delta;
		_currentState?.UpdateMovement(dt);
		_currentState?.PhysicsUpdate(dt);
	} 

	private void OnChildTransitioned(State state, string newStateName)
	{
		if (state != _currentState || !_states.TryGetValue(newStateName, out State newState)) return;

		_currentState.Exit();
		newState.Enter();
		_currentState = newState;
	}
}