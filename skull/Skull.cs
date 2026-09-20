using System.Collections.Generic;
using Godot;

namespace imminent_doom.skull;

public partial class Skull : CharacterBody3D
{
	[Export] public CollisionShape3D CollisionShape3D;
	[Export] public CharacterBody3D Player;

	private MeshInstance3D _mesh;

	public override void _Ready()
	{
		CollisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
		if (Player == null)
		{
			GD.Print("Player not set for enemy: " + Name);
		}

		foreach (Node child in GetNode("StateMachine").GetChildren())
		{
			if (child is EnemyState state)
			{
				state.Player = Player;
			}
		}

		_mesh = GetNode<MeshInstance3D>("Skull_Inherited/Armature/Skeleton3D/Skull");
	}

	private void Die()
	{
		SetPhysicsProcess(false);
		CollisionShape3D?.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);

		var tween = CreateTween();
		tween.TweenProperty(this, "scale", Scale * 0.05f, 0.3f);
		tween.TweenCallback(Callable.From(QueueFree));
	}

	static readonly StandardMaterial3D FlashMat = new()
	{
		AlbedoColor = Colors.DarkRed,
		ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
	};

	public async void Flash(float duration = 0.08f)
	{
		var mat = _mesh.MaterialOverlay;
		_mesh.MaterialOverlay = FlashMat;

		// await ToSignal(GetTree().CreateTimer(
		// 		duration,
		// 		true,
		// 		false,
		// 		true
		// 	),
		// 	SceneTreeTimer.SignalName.Timeout
		// );
		//
		// if (!IsInstanceValid(this)) return;
		// 	_mesh.MaterialOverlay = mat;
	}

}
