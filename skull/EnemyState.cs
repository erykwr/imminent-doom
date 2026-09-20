using Godot;

namespace imminent_doom.skull;

public partial class EnemyState : Node
{
	[Signal] public delegate void TransitionedEventHandler(EnemyState state, string newStateName);

	[Export] public NodePath EnemyBodyPath = "../..";
	[Export] public NodePath EnemyAnimationPlayerPath = "../../Skull_Inherited/AnimationPlayer";
	[Export] public NodePath EnemyAnimationTreePath = "../../Skull_Inherited/AnimationTree";
	[Export] public NodePath NavigationAgentPath = "../../NavigationAgent3D";

	[Export] public float Speed = 2.0f;
	[Export] public float Acceleration = 15.0f;
	[Export] public float RotationSpeed = 10.0f;
	[Export] public float MovementThreshold = 0.1f;

	[Export] public float ApproachRadius = 8.0f;
	[Export] public float AttackRadius = 2.0f;

	protected CharacterBody3D CharacterBody3D;
	protected AnimationPlayer AnimationPlayer;
	protected AnimationTree AnimationTree;
	protected AnimationNodeStateMachinePlayback Playback;
	public NavigationAgent3D NavAgent;
	public Node3D Player;

	public override void _Ready()
	{
		CharacterBody3D = GetNode<CharacterBody3D>(EnemyBodyPath);

		if (EnemyAnimationPlayerPath != null)
			AnimationPlayer = GetNodeOrNull<AnimationPlayer>(EnemyAnimationPlayerPath);
		if (EnemyAnimationTreePath != null)
			AnimationTree = GetNodeOrNull<AnimationTree>(EnemyAnimationTreePath);

		Playback = AnimationTree?.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();

		NavAgent = GetNode<NavigationAgent3D>(NavigationAgentPath);
	}

	public virtual void Enter() { }
	public virtual void Exit() { }
	public virtual void PhysicsUpdate(float delta) { }

	protected float DistanceToPlayer()
	{
		if (Player == null) return float.MaxValue;
		return CharacterBody3D.GlobalPosition.DistanceTo(Player.GlobalPosition);
	}
	
	protected void MoveToward(Vector3 targetPos, float delta)
	{
		if(NavAgent == null) return;
		Vector3 velocity = CharacterBody3D.Velocity;

		NavAgent.TargetPosition = targetPos;
		
		Vector3 direction = NavAgent.IsNavigationFinished()
			? Vector3.Zero
			: NavAgent.GetNextPathPosition() - CharacterBody3D.GlobalPosition;

		direction.Y = 0;
		Vector3 desiredVelocity;

		if (direction.Length() > MovementThreshold)
		{
			direction = direction.Normalized();
			Vector3 targetVel = direction * Speed;
			desiredVelocity = new Vector3(
				Mathf.MoveToward(velocity.X, targetVel.X, Acceleration * delta),
				velocity.Y,
				Mathf.MoveToward(velocity.Z, targetVel.Z, Acceleration * delta)
			);

			FaceDirection(direction, delta);
		}
		else
		{
			desiredVelocity = new Vector3(
				Mathf.MoveToward(velocity.X, 0f, Acceleration * delta),
				velocity.Y,
				Mathf.MoveToward(velocity.Z, 0f, Acceleration * delta)
			);
		}

		if (NavAgent is { AvoidanceEnabled: true })
			NavAgent.Velocity = desiredVelocity;
		else
			ApplyVelocity(desiredVelocity);
	}

	public void ApplyVelocity(Vector3 velocity)
	{
		CharacterBody3D.Velocity = velocity;
		CharacterBody3D.MoveAndSlide();

		if (AnimationTree == null) return;

		Vector2 horizontalVelocity = new Vector2(CharacterBody3D.Velocity.X, CharacterBody3D.Velocity.Z);
		float speedRatio = Mathf.Clamp(horizontalVelocity.Length() / Speed, 0f, 1f);
		AnimationTree.Set("parameters/BlendSpace1D/blend_position", speedRatio);
	}

	protected void FaceDirection(Vector3 direction, float delta)
	{
		if (direction.Length() < 0.01f) return;
		float targetAngle = Mathf.Atan2(direction.X, direction.Z);
		CharacterBody3D.Rotation = new Vector3(
			CharacterBody3D.Rotation.X,
			Mathf.LerpAngle(CharacterBody3D.Rotation.Y, targetAngle, RotationSpeed * delta),
			CharacterBody3D.Rotation.Z
		);
	}

	protected void FacePlayer(float delta)
	{
		if (Player == null) return;
		Vector3 dir = Player.GlobalPosition - CharacterBody3D.GlobalPosition;
		dir.Y = 0;
		FaceDirection(dir, delta);
	}

}
