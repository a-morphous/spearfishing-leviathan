using Godot;
using System;

public partial class AttackArea : Area2D
{
	CollisionShape2D Shape;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Shape = GetNode<CollisionShape2D>("CollisionShape2D");
		Shape.Disabled = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public async void Activate(){
		Shape.Disabled = false;
		await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
		Shape.Disabled = true;
	}
}
