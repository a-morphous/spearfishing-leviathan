using Godot;

public class LeviathanIntroBehavior : IActorBehavior
{
	Node2D Target;
	public float MaxSpeed = 120;

	// physics
	public float VelocityDecay = 0.98f;
	public float Acceleration = 10f;
	public float ChangeCourseAcceleration = 20f;

	Vector2 velocity;

	public float CurrentAngle = 0;
	public float AngleSpeed = 200;


	public LeviathanIntroBehavior(Node2D target)
	{
		Target = target;
	}

	public void Update(Actor actor, double delta)
	{
		if (Target == null)
		{
			return;
		}
		if (!(actor is Leviathan)) {
			return;
		}
		Leviathan lev = (Leviathan)actor;
		if (actor.GlobalPosition.DistanceTo(Target.GlobalPosition) < 100) {
			lev.LeviathanState = Leviathan.LEVIATHAN_STATES.NIGHTMARE_CHARGE;
			lev.IsInvincible = false;
		}
		velocity = actor.Velocity;
		Vector2 direction = actor.GlobalPosition.DirectionTo(Target.GlobalPosition);
		velocity = MaxSpeed * direction;

		actor.Velocity = velocity;
	}
}