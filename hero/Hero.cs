using Godot;

namespace imminent_doom.hero;

public partial class Hero : CharacterBody3D
{
	[Signal] public delegate void OnDeathEventHandler();

	private void onDeath()
	{
		EmitSignal(SignalName.OnDeath);
	}

}
