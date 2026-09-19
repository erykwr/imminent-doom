using Godot;

namespace imminent_doom.skull;

public partial class EnemyApproachingState : EnemyState
{

	public override void PhysicsUpdate(float delta)
	{
		float dist = DistanceToPlayer();

		if (dist > ApproachRadius)
		{
			EmitSignal(EnemyState.SignalName.Transitioned, this, "Idle");
			return;
		}

		if (dist <= AttackRadius)
		{
			EmitSignal(EnemyState.SignalName.Transitioned, this, "Attack");
			return;
		}

		MoveToward(Player.GlobalPosition, delta);
	}
}
