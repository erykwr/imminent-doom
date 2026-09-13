using Godot;

namespace imminent_doom.hero;

public partial class State : Node
{
	[Export] public string AnimationPlayerNodePath = "../../Hero2/AnimationPlayer";
	[Export] public string AnimationTreeNodePath = "../../Hero2/AnimationTree";
	[Export] public string CharacterBody3DNodePath = "../..";
	[Export] public string CameraNodePath = "../../../Camera3D";
	[Export] public string CollisionShape3DNodePath = "../../CollisionShape3D";

	[Export] public float Speed = 8.0f;
	[Export] public float Acceleration = 25.0f;
	[Export] public float RotationSpeed = 15.0f;	
	[Export] public float MovementThreshold = 0.1f;

	
	protected AnimationPlayer AnimationPlayer;
	protected CharacterBody3D CharacterBody3D;
	protected AnimationTree AnimationTree;
	protected AnimationNodeStateMachinePlayback Playback;
	protected Camera3D Camera;
	protected CollisionShape3D CollisionShape3D;
	
	public override void _Ready()
	{
		AnimationPlayer = GetNode<AnimationPlayer>(AnimationPlayerNodePath);
		CharacterBody3D = GetNode<CharacterBody3D>(CharacterBody3DNodePath);
		AnimationTree = GetNode<AnimationTree>(AnimationTreeNodePath);
		Playback = AnimationTree?.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();
		Camera = GetNode<Camera3D>(CameraNodePath);
		CollisionShape3D = GetNode<CollisionShape3D>(CollisionShape3DNodePath);
		if (AnimationPlayer == null)
		{
			GD.PrintErr("Could not find animation player");
		}
		if (CharacterBody3D == null) 
		{
			GD.PrintErr("Could not find character body 3D");
		}
		if (Camera == null)
		{
			GD.PrintErr("Could not find camera 3D");
		}
		if (CollisionShape3D == null)
		{
			GD.PrintErr("Could not find collision shape 3D");
		}
		if (AnimationTree == null)
		{
			GD.Print("Could not find animation tree");
		}
		if (Playback == null)
		{
			GD.PrintErr("Could not find playback");
		}
	}
	
	[Signal] public delegate void TransitionedEventHandler(State state, string newStateName);

	public virtual void Enter() { }
	public virtual void Exit() { }
	public virtual void HandleInput(InputEvent @event) { }
	public virtual void PhysicsUpdate(float delta) { }
	
	public virtual void UpdateMovement(float delta)
	{
		Vector3 velocity = CharacterBody3D.Velocity;

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
			CharacterBody3D.Rotation = new Vector3(
				CharacterBody3D.Rotation.X,
				Mathf.LerpAngle(CharacterBody3D.Rotation.Y, targetAngle, RotationSpeed * delta),
				CharacterBody3D.Rotation.Z
			);
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0f, Acceleration * delta);
			velocity.Z = Mathf.MoveToward(velocity.Z, 0f, Acceleration * delta);
		}

		CharacterBody3D.Velocity = velocity;
		CharacterBody3D.MoveAndSlide();
		
		Vector2 horizontalVelocity = new Vector2(CharacterBody3D.Velocity.X, CharacterBody3D.Velocity.Z);
		float speed = Mathf.Clamp(horizontalVelocity.Length() / Speed, 0f, 1f);
		AnimationTree.Set("parameters/BlendSpace1D/blend_position", speed);
	}
	
	protected bool IsMoving()
	{
		Vector2 horizontalVelocity = new Vector2(CharacterBody3D.Velocity.X, CharacterBody3D.Velocity.Z);
		
		return horizontalVelocity.Length() > MovementThreshold;
	}
}