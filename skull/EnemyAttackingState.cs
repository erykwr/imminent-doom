using Godot;

namespace imminent_doom.skull;

public sealed partial class EnemyAttackingState : EnemyState
{
	[Export] public float AttackCooldown = 1.5f;

	public override void Enter()
	{
		CharacterBody3D.Velocity = Vector3.Zero;
		Playback?.Travel("Attack");
	}

	public override void Exit()
	{
		Playback?.Travel("BlendSpace1D");
	}

	public override void PhysicsUpdate(float delta)
	{
		float dist = DistanceToPlayer();

		if (dist > AttackRadius)
		{
			EmitSignal(EnemyState.SignalName.Transitioned, this,
				dist <= ApproachRadius ? "Approaching" : "Idle");
			return;
		}

		FacePlayer(delta);
	}

}
