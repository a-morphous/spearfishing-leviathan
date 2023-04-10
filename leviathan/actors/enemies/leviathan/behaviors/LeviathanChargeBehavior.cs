using Godot;

public class LeviathanChargeBehavior : IActorBehavior
{
	Node2D Target;
	public float MaxSpeed = 90;

	// physics
	public float VelocityDecay = 0.98f;
	public float Acceleration = 10f;
	public float ChangeCourseAcceleration = 20f;

	Vector2 velocity;

	public float CurrentAngle = 0;
	public float AngleSpeed = 200;


	public LeviathanChargeBehavior(Node2D target)
	{
		MaxSpeed = Mathf.Sqrt(2 * MaxSpeed * MaxSpeed);
		Target = target;
		AngleSpeed = Mathf.DegToRad(AngleSpeed);
	}

	public void Reset(Actor actor)
	{
		CurrentAngle = actor.GlobalPosition.AngleToPoint(Target.GlobalPosition);
	}

	public void Update(Actor actor, double delta)
	{
		if (Target == null)
		{
			return;
		}
		velocity = actor.Velocity;
		velocity *= VelocityDecay;
		float ActualMoveAngle = actor.Velocity.Angle();
		// move the angle towards the target
		float DesiredAngle = actor.GlobalPosition.AngleToPoint(Target.GlobalPosition);
		float NeededMovement = Mathf.LerpAngle(CurrentAngle, DesiredAngle, 1f);

		float MaxAngleCanMove = (float)(AngleSpeed * delta);

		if (NeededMovement < MaxAngleCanMove)
		{
			CurrentAngle = DesiredAngle;
		}
		else
		{
			float ActualRotation = Mathf.Clamp(NeededMovement, CurrentAngle - MaxAngleCanMove, CurrentAngle + MaxAngleCanMove);
			CurrentAngle = ActualRotation;
		}

		float TurnAroundMovement = Mathf.LerpAngle(ActualMoveAngle, DesiredAngle, 1f);
		float actualAccel = Acceleration;
		if (TurnAroundMovement > Mathf.DegToRad(120))
		{
			actualAccel = ChangeCourseAcceleration;
		}

		// if we're within a good range angle wise to the target, speed up
		Vector2 CurrentDirection = Vector2.FromAngle(CurrentAngle);
		velocity.X += CurrentDirection.X * actualAccel;
		velocity.Y += CurrentDirection.Y * actualAccel;

		velocity.Clamp(new Vector2(-MaxSpeed, -MaxSpeed), new Vector2(MaxSpeed, MaxSpeed));
		actor.Velocity = velocity;
	}
}