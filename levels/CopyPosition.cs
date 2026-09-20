using Godot;

namespace imminent_doom.levels;

public partial class CopyPosition : Camera3D
{
	[Export] public Node3D Source { get; set; }
	[Export] public float MaxOffset { get; set; } = 0.2f;
	[Export] public float Decay { get; set; } = 3.0f;

	private float _trauma;
	private readonly RandomNumberGenerator _rng = new();
	
	private Vector3 _offset;
	
	public void Shake(float amount = 0.5f)
	{
		_trauma = Mathf.Min(_trauma + amount, 1.0f);
	}
	
	public override void _Ready()
	{
		if (Source == null)
		{
			GD.PrintErr("Source not set");
			return;
		}
		_offset = GetGlobalPosition() - Source.GetGlobalPosition();
		
	}
	
	public override void _Process(double delta)
	{
		if (_trauma <= 0f)
		{
			HOffset = 0f;
			VOffset = 0f;
			return;
		}

		HOffset = MaxOffset * _trauma * _rng.RandfRange(-1f, 1f);
		VOffset = MaxOffset * _trauma * _rng.RandfRange(-1f, 1f);
		_trauma = Mathf.Max(_trauma - Decay * (float)delta, 0f);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if(Source == null) return;
		SetGlobalPosition(Source.GlobalPosition + _offset);
	}

}
