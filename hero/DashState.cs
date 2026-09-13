using Godot;

namespace imminent_doom.hero;

public partial class DashState : State
{
	[Export] public float DashRange = 4.0f;
	[Export] public float DashFrames = 10f;
	[Export] public float DashStepSize = 0.15f;
	[Export] public uint TerrainLayer = 1;
	
	private Vector3 _dashStart;
	private Vector3 _dashEnd;
	private float _dashFrame;
	
	public override void Enter()
	{
		_dashStart = CharacterBody3D.GlobalPosition;
		_dashEnd = FindDashLanding(GetForwardDirection());
		_dashFrame = 0;

		Playback?.Travel("Dash");
	}

	public override void Exit()
	{
		Playback?.Travel("BlendSpace1D");
	}

	public override void PhysicsUpdate(float delta)
	{
		_dashFrame++;
		float t = Mathf.Min(_dashFrame / DashFrames, 1f);
		CharacterBody3D.GlobalPosition = _dashStart.Lerp(_dashEnd, t);

		if (_dashFrame >= DashFrames)
		{
			EmitSignal(State.SignalName.Transitioned, this, "Idle");
		}
	}

	public override void UpdateMovement(float delta)
	{
	}


	private Vector3 GetForwardDirection()
	{
		return CharacterBody3D.GlobalTransform.Basis.Z;
	}
	
	private Vector3 FindDashLanding(Vector3 direction)
	{
		direction = direction.Normalized();
		var spaceState = CharacterBody3D.GetWorld3D().DirectSpaceState;

		var query = new PhysicsShapeQueryParameters3D
		{
			Shape = CollisionShape3D.Shape,
			CollisionMask = TerrainLayer,
			Exclude = new Godot.Collections.Array<Rid> { CharacterBody3D.GetRid() }
		};

		for (float dist = DashRange; dist > 0f; dist -= DashStepSize)
		{
			Vector3 candidate = CharacterBody3D.GlobalPosition + direction * dist;
			query.Transform = new Transform3D(Basis.Identity, candidate);

			var overlaps = spaceState.IntersectShape(query, maxResults: 1);
			if (overlaps.Count == 0)
				return candidate;
		}

		return CharacterBody3D.GlobalPosition;
	}

}
