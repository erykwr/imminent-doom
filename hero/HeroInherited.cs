using Godot;

namespace imminent_doom.hero;

public partial class HeroInherited : Node3D
{
	[Signal] public delegate void AttackStartedEventHandler();
	[Signal] public delegate void AttackFinishedEventHandler();
	
	
	public void EnableHitbox()
	{
		EmitSignal(SignalName.AttackStarted);
	}
	
	public void DisableHitbox()
	{
		EmitSignal(SignalName.AttackFinished);
	}

}
