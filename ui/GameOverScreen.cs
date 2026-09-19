using Godot;

namespace imminent_doom.ui;

public partial class GameOverScreen : CanvasLayer
{

	private void OnPlayerDied() => Show();

	public override void _UnhandledInput(InputEvent e)
	{
		if (Visible && e.IsActionPressed("ui_accept"))
			GetTree().ReloadCurrentScene();
	}

}
