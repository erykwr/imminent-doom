using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using imminent_doom.skull;

namespace imminent_doom.levels;

public partial class EnemySpawner : Node3D
{
	[ExportGroup("Spawn area")]
	[Export] public PackedScene EnemyScene;
	[Export] public CollisionShape3D SpawnBox;
	[Export] public Node3D Target;
	[Export] public float MinDistanceFromTarget = 8f;
	[Export] public int MaxAttempts = 20;
	[Export] public float MinSpacing = 1f;

	[Export(PropertyHint.Layers3DPhysics)] public uint BlockMask = 0b1110;

	[ExportGroup("Wave shape")]
	[Export] public int FirstBatch = 2;
	[Export] public float BatchGrowth = 2f;
	[Export] public float MinDelay = 2f;
	[Export] public float MaxDelay = 3f;
	[Export] public int TestWaveSize = 20;
	

	public int Alive { get; private set; }

	private readonly List<Vector3> _batchSpots = [];

	public override void _Ready()
	{
		if (TestWaveSize > 0)
			_ = SpawnWave(TestWaveSize);
	}

	public async Task SpawnWave(int total)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		try
		{
			int remaining = total;
			int batch = FirstBatch;
			int batchIndex = 1;

			while (remaining > 0)
			{
				int count = Math.Min(batch, remaining);
				int spawned = SpawnBatch(count);

				remaining -= count;
				batch = Mathf.CeilToInt(batch * BatchGrowth);
				batchIndex++;

				if (remaining > 0)
				{
					double delay = GD.RandRange(MinDelay, MaxDelay);
					await ToSignal(GetTree().CreateTimer(delay, false), SceneTreeTimer.SignalName.Timeout);
				}
			}
		}
		catch (Exception e)
		{
			GD.PushError($"[Spawner] Wave failed: {e}");
		}
	}

	public int SpawnBatch(int count)
	{
		_batchSpots.Clear();
		int spawned = 0;
		for (int i = 0; i < count; i++)
			if (TrySpawn()) spawned++;
		return spawned;
	}

	public bool TrySpawn()
	{
		if (EnemyScene == null)
		{
			GD.PushError("[Spawner] EnemyScene is not assigned.");
			return false;
		}

		var enemy = (Skull) EnemyScene.Instantiate<Node3D>();
		enemy.Player = (CharacterBody3D) Target;
		var col = enemy.GetNode<CollisionShape3D>("CollisionShape3D");

		if (!FindSpot(col, out Vector3 spot))
		{
			enemy.Free();
			return false;
		}

		GetParent().AddChild(enemy);
		enemy.GlobalPosition = spot;

		_batchSpots.Add(spot);
		Alive++;
		enemy.TreeExited += () => Alive--;

		return true;
	}

	private bool FindSpot(CollisionShape3D col, out Vector3 spot)
	{
		spot = default;

		var box = SpawnBox?.Shape as BoxShape3D;
		if (box == null)
		{
			GD.PushError("[Spawner] SpawnBox must be a CollisionShape3D with a BoxShape3D.");
			return false;
		}

		var space = GetWorld3D().DirectSpaceState;
		var shapeQuery = new PhysicsShapeQueryParameters3D
		{
			Shape = col.Shape,
			CollisionMask = BlockMask
		};

		int nearTarget = 0, tooClose = 0, blocked = 0;

		for (int i = 0; i < MaxAttempts; i++)
		{
			var local = new Vector3(
				(float) GD.RandRange(-0.5, 0.5) * box.Size.X,
				-box.Size.Y / 2f,
				(float) GD.RandRange(-0.5, 0.5) * box.Size.Z);
			Vector3 p = SpawnBox.GlobalTransform * local + Vector3.Up * 0.05f;

			if (Target != null)
			{
				Vector3 t = Target.GlobalPosition;
				if (new Vector2(p.X - t.X, p.Z - t.Z).Length() < MinDistanceFromTarget)
				{
					nearTarget++;
					continue;
				}
			}

			bool close = false;
			foreach (var s in _batchSpots)
				if (s.DistanceTo(p) < MinSpacing) { close = true; break; }
			if (close)
			{
				tooClose++;
				continue;
			}

			shapeQuery.Transform = new Transform3D(Basis.Identity, p) * col.Transform;
			if (space.IntersectShape(shapeQuery, 1).Count > 0)
			{
				blocked++;
				continue;
			}

			spot = p;
			return true;
		}
		return false;
	}
}
