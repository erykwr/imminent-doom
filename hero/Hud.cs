using Godot;
using imminent_doom.common;

namespace imminent_doom.hero;

public partial class Hud : Control
{
	[Export] private CharacterBody3D _hero;
	private ProgressBar _healthBar;
	
	public override void _Ready()
	{
		_healthBar = GetNode<ProgressBar>("CanvasLayer/HealthBar");
		var health = _hero?.GetNode<Health>("Health");
		if (health != null)
		{
			health.HealthChanged += OnHealthChanged;
		}
	}

	private void OnHealthChanged(float currentHealth, float maxHealth)
	{
		if(_healthBar ==  null) return;
		GD.Print("Health: " + currentHealth + "/" + maxHealth);
		_healthBar.Value = currentHealth;
		_healthBar.MaxValue = maxHealth;
	}

}
