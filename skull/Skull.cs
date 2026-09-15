using Godot;

namespace imminent_doom.skull;

public partial class Skull : CharacterBody3D
{
    [Export] private Node3D _healthBarAnchor;
    [Export] private ProgressBar _healthBarUi;
    [Export] private Camera3D _camera;
    
    public override void _Process(double delta)
    {
        UpdateHealthBarPosition();
    }

    private void UpdateHealthBarPosition()
    {
        Vector3 worldPos = _healthBarAnchor.GlobalPosition;

        Vector3 camLocal = _camera.GlobalTransform.AffineInverse() * worldPos;
        if (camLocal.Z > 0)
        {
            _healthBarUi.Visible = false;
            return;
        }

        Vector2 screenPos = _camera.UnprojectPosition(worldPos);
        _healthBarUi.Visible = true;
        _healthBarUi.Position = screenPos - _healthBarUi.Size / 2f;
    }

    private void OnHealthChanged(float currentHealth, float maxHealth)
    {
        _healthBarUi.SetValue(currentHealth / maxHealth);
    }
}
