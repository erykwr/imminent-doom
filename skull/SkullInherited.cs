using Godot;

namespace imminent_doom.skull;

public partial class SkullInherited : Node3D
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
