using Godot;

namespace imminent_doom.common;

public partial class Health : Node
{
	[Signal]
	public delegate void HealthChangedEventHandler(float currentHealth, float maxHealth);

	[Signal]
	public delegate void DamagedEventHandler(float amount, Node source);

	[Signal]
	public delegate void HealedEventHandler(float amount);

	[Signal]
	public delegate void DiedEventHandler();

	[Export]
	public float MaxHealth { get; set; } = 100f;

	[Export]
	public bool IsInvincible { get; set; }

	public float CurrentHealth { get; private set; }

	public bool IsDead { get; private set; }

	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
	}

	public void TakeDamage(float amount, Node source = null)
	{
		if (IsDead || IsInvincible || amount <= 0f)
			return;

		CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);

		EmitSignal(SignalName.Damaged, amount, source);
		EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);

		if (CurrentHealth <= 0f)
		{
			Die();
		}
	}
	

	private void Die()
	{
		IsDead = true;
		EmitSignal(SignalName.Died);
	}
}
