using Godot;
using System;

public partial class EnemyBullet : EnemyAttack
{
	public float Speed = 400;

	public float BulletGravity = 100;
	Vector2 Velocity;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Velocity = Vector2.FromAngle(GlobalRotation) * Speed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Velocity.Y += BulletGravity * (float)delta;
		Position += Velocity * (float)delta;
	}
}
