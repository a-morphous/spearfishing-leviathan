using Godot;
using System;

public partial class FlowerSprite : Node2D
{
	[Signal]
	public delegate void FireEventHandler();

	AnimationPlayer Animations;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Animations = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void PlayFireAnimation() {
		Animations.Play("Shoot", 0.1);
	}

	public void ReturnToIdleAnimation() {
		Animations.Play("Idle", 0.1);
	}

	public void SendFireSignal() {
		EmitSignal(SignalName.Fire);
	}
}
