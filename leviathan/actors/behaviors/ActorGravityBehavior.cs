using Godot;
using System;
public class ActorGravityBehavior : IActorBehavior
{
	Vector2 velocity;
	public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public float MaxGravity = 800;
	public void Update(Actor actor, double delta)
	{
		velocity = actor.Velocity;

		if (!actor.IsOnFloor())
		{
			velocity.Y += Gravity * (float)delta * actor.GravityFactor;
		}

		velocity.Y = MathF.Min(velocity.Y, MaxGravity);

		actor.Velocity = velocity;
	}
}