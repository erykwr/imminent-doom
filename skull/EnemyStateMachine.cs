using System.Linq;
using Godot;
using Godot.Collections;

namespace imminent_doom.skull;

public partial class EnemyStateMachine : Node
{
    [Export] public NodePath InitialStatePath;

    private EnemyState _currentState;
    private NavigationAgent3D _navAgent;
    private readonly Dictionary<string, EnemyState> _states = new();

    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is EnemyState state)
            {
                _states[state.Name] = state;
                state.Transitioned += OnChildTransitioned;
            }
        }

        _currentState = GetNode<EnemyState>(InitialStatePath);
        
        _navAgent = _currentState.NavAgent;
        if (_navAgent is { AvoidanceEnabled: true })
            _navAgent.VelocityComputed += OnVelocityComputed;

        _currentState.Enter();
    }

    public override void _ExitTree()
    {
        if (_navAgent is { AvoidanceEnabled: true })
            _navAgent.VelocityComputed -= OnVelocityComputed;
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentState?.PhysicsUpdate((float) delta);
    }

    private void OnVelocityComputed(Vector3 safeVelocity) => _currentState?.ApplyVelocity(safeVelocity);

    private void OnChildTransitioned(EnemyState state, string newStateName)
    {
        if (state != _currentState || !_states.TryGetValue(newStateName, out EnemyState newState)) return;
        
        _currentState.Exit();
        newState.Enter();
        _currentState = newState;
    }

}
