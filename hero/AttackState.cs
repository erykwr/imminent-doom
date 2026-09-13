using Godot;

namespace imminent_doom.hero;

public partial class AttackState : State
{
	private readonly StringName _animationName = "Attack";
	
	public override void Enter()
	{
		AnimationPlayer?.Play(_animationName);
		if (AnimationPlayer != null)
		{
			AnimationPlayer.AnimationFinished += AnimationFinished;
		}
	}

	public override void Exit()
	{
		if (AnimationPlayer != null)
		{
			AnimationPlayer.AnimationFinished -= AnimationFinished;
		}
	}


	private void AnimationFinished(StringName animationName)
	{
		if (animationName == _animationName)
		{
			EmitSignal(State.SignalName.Transitioned, this, "Idle");
		}
	}

}
