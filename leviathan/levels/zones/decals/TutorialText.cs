using Godot;
using System;

public partial class TutorialText : Area2D
{
	// Called when the node enters the scene tree for the first time.
	AnimationPlayer Animations;
	public override void _Ready()
	{
		Animations = GetNode<AnimationPlayer>("AnimationPlayer");
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyLeft;
	}

	protected void OnBodyEntered(Node2D body) {
		if (body is Player) {
			Animations.Play("Pulse", 2f);
		}
	}
	protected void OnBodyLeft(Node2D body) {
		if (body is Player) {
			Animations.Play("Invisible", 2f);
		}
	}
}
