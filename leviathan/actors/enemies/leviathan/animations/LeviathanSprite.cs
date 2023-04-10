using Godot;
using System;

public partial class LeviathanSprite : Node2D
{
	[Signal]
	public delegate void DeathAnimationFinishedEventHandler();
	AnimationPlayer Animations;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Animations = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public bool IsPlaying(string animationName) {
		if (!Animations.IsPlaying()) {
			return false;
		}
		if (Animations.CurrentAnimation != animationName) {
			return false;
		}
		return true;
	}

	public void Play(string animationName) {
		if (IsPlaying(animationName)) {
			return;
		}
		Animations.Play(animationName);
	}

	public void Play(string animationName, float customBlend) {
		if (IsPlaying(animationName)) {
			return;
		}
		Animations.Play(animationName, customBlend);
	}

	public void FinishedDeathAnimation() {
		EmitSignal(SignalName.DeathAnimationFinished);
	}
}
