using System;
using Godot;
using imminent_doom.common;

namespace imminent_doom.skull;

public sealed partial class EnemyAttackingState : EnemyState
{
	[Export] public float AttackCooldown = 1.5f;
	
	[Export] public Area3D Hitbox { get; set; }
	
	private Random _random = new();

	public override void Enter()
	{
		CharacterBody3D.Velocity = Vector3.Zero;
		Playback?.Travel("Attack");
	}

	public override void Exit()
	{
		Playback?.Travel("BlendSpace1D");
		Hitbox?.SetMonitoring(false);
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

	private void HitStarted()
	{
		Hitbox?.SetMonitoring(true);
	}

	private void HitFinished()
	{
		Hitbox?.SetMonitoring(false);
	}
	
	private void OnBodyEntered(Node3D node)
	{
		Health health = node.GetNode<Health>("Health");
		float damage = _random.NextSingle() * 1.0f + 0.5f;
		
		health?.TakeDamage(damage, CharacterBody3D);
	}
	
}
