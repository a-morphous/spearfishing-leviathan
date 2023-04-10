using Godot;
using System;
using deVoid.Utils;

public partial class PlayerActivateLeviathanArea : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += (Node2D body) => {
			if (!body.IsInGroup("player")) {
				return;
			}
			Signals.Get<LeviathanEnterSignal>().Dispatch();
			QueueFree();
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
