using Godot;

namespace imminent_doom.skull;

public partial class EnemyIdleState : EnemyState
{
	public override void Enter()
	{
		CharacterBody3D.Velocity = Vector3.Zero;
	}
 
	public override void PhysicsUpdate(float delta)
	{
		if (DistanceToPlayer() <= ApproachRadius)
		{
			EmitSignal(EnemyState.SignalName.Transitioned, this, "Approaching");
		}
	}
}
