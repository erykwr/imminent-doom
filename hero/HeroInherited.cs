using Godot;

namespace imminent_doom.hero;

public partial class HeroInherited : Node3D
{
	[Signal] public delegate void AttackStartedEventHandler();
	[Signal] public delegate void AttackFinishedEventHandler();
	[Signal] public delegate void FootStepEventHandler();

	private AnimationTree _animationTree;

	public override void _Ready()
	{
		_animationTree = GetNode<AnimationTree>("AnimationTree");
	}
	
	public void EnableHitbox()
	{
		EmitSignal(SignalName.AttackStarted);
	}
	
	public void DisableHitbox()
	{
		EmitSignal(SignalName.AttackFinished);
	}
	
	public void OnFootStep()
	{
		if (_animationTree.Get("parameters/BlendSpace1D/blend_position").AsSingle() < 0.05f) return;
		EmitSignal(SignalName.FootStep);
	}

}
