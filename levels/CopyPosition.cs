using Godot;

namespace imminent_doom.levels;

public partial class CopyPosition : Camera3D
{
	[Export] public Node3D Source { get; set; }

	private Vector3 _offset;
	
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
		if(Source == null) return;
		SetGlobalPosition(Source.GlobalPosition + _offset);
	}

}
