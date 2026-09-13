using Godot;

namespace imminent_doom.hero;

public partial class Hero : CharacterBody3D
{
	[Export] public float Speed = 8.0f;
	[Export] public float Acceleration = 25.0f;
	[Export] public float RotationSpeed = 15.0f;
	
	[Export] public float MovementThreshold = 0.1f;
	
	[Export] public float DashRange = 4.0f;
	[Export] public float DashFrames = 10f;
	[Export] public float DashStepSize = 0.15f;
	[Export] public uint TerrainLayer = 1;

	[Export] Camera3D Camera { get; set; }
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("Hero2/AnimationPlayer");
		if (_animationPlayer == null)
		{
			GD.PrintErr("AnimationPlayer is null");
		}
		if (Camera == null)
		{
			GD.PrintErr("Camera3D is null");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if(Camera == null) return;
		if (_isDashing)
		{
			_dashFrame++;
			float t = Mathf.Min(_dashFrame / DashFrames, 1f);
			GlobalPosition = _dashStart.Lerp(_dashEnd, t);

			if (_dashFrame >= DashFrames)
				EndDash();

			return;
		}
		if (Input.IsActionJustPressed("ui_select"))
		{
			StartDash();
			return;
		}
		
		UpdateMovement((float) delta);
		UpdateAnimations();
	}

	private void UpdateMovement(float delta)
	{
		Vector3 velocity = Velocity;

		Vector2 inputDir = Input.GetVector(
			"ui_left",
			"ui_right",
			"ui_up",
			"ui_down"
		);

		Vector3 camForward = -Camera.GlobalTransform.Basis.Z;
		Vector3 camRight = Camera.GlobalTransform.Basis.X;
		camForward.Y = 0;
		camForward = camForward.Normalized();
		camRight.Y = 0;
		camRight = camRight.Normalized();

		Vector3 direction = (camRight * inputDir.X + camForward * -inputDir.Y);

		if (direction.Length() > 0.01f)
		{
			direction = direction.Normalized();
			Vector3 targetVel = direction * Speed;
			velocity.X = Mathf.MoveToward(velocity.X, targetVel.X, Acceleration * delta);
			velocity.Z = Mathf.MoveToward(velocity.Z, targetVel.Z, Acceleration * delta);
			
			float targetAngle = Mathf.Atan2(direction.X, direction.Z);
			Rotation = new Vector3(
				Rotation.X,
				Mathf.LerpAngle(Rotation.Y, targetAngle, RotationSpeed * delta),
				Rotation.Z
			);
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0f, Acceleration * delta);
			velocity.Z = Mathf.MoveToward(velocity.Z, 0f, Acceleration * delta);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void UpdateAnimations()
	{
		Vector2 horizontalVelocity = new Vector2(Velocity.X, Velocity.Z);
		
		string targetAnimation = horizontalVelocity.Length() > MovementThreshold 
			? "Run" 
			: "Idle";

		if (_animationPlayer.CurrentAnimation != targetAnimation)
		{
			_animationPlayer.Play(targetAnimation);
		}
	}

	#region Dash

	private Vector3 _dashStart;
	private Vector3 _dashEnd;
	private float _dashFrame;
	private bool _isDashing;
	
	private Vector3 GetForwardDirection()
	{
		return GlobalTransform.Basis.Z;
	}
	
	private void StartDash()
	{
		_dashStart = GlobalPosition;
		_dashEnd = FindDashLanding(GetForwardDirection());
		_dashFrame = 0;
		_isDashing = true;

		_animationPlayer.Play("Dash", customBlend: 0.05);
	}

	private void EndDash()
	{
		_isDashing = false;
		_animationPlayer.Play("Idle");
	}
	
	private Vector3 FindDashLanding(Vector3 direction)
	{
		direction = direction.Normalized();
		var spaceState = GetWorld3D().DirectSpaceState;
		var shape = GetNode<CollisionShape3D> ("CollisionShape3D").Shape;

		var query = new PhysicsShapeQueryParameters3D
		{
			Shape = shape,
			CollisionMask = TerrainLayer,
			Exclude = new Godot.Collections.Array<Rid> { GetRid() }
		};

		for (float dist = DashRange; dist > 0f; dist -= DashStepSize)
		{
			Vector3 candidate = GlobalPosition + direction * dist;
			query.Transform = new Transform3D(Basis.Identity, candidate);

			var overlaps = spaceState.IntersectShape(query, maxResults: 1);
			if (overlaps.Count == 0)
				return candidate;
		}

		return GlobalPosition;
	}
	
	
	#endregion
}
