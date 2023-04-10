using Godot;

public class PlayerMovementBehavior : IActorBehavior
{
	public float MaxSpeed = 300f;
	public float MaxAcceleration = 80f;
	public float MaxDeceleration = 150f;
	public float MaxTurnSpeed = 80f;
	public float MaxAirAcceleration = 30f;
	public float MaxAirDeceleration = 30f;
	public float MaxAirTurnSpeed = 30f;

	private float maxSpeedChange;
	private float acceleration;
	private float deceleration;
	private float turnSpeed;

	private float directionX;
	private Vector2 velocity;
	private Vector2 desiredVelocity;

	public void Update(Actor actor, double delta)
	{
		velocity = actor.Velocity;
		directionX = Input.GetVector("move_left", "move_right", "move_up", "move_down").X;

		desiredVelocity = new Vector2(directionX, 0f) * Mathf.Max(MaxSpeed, 0f);
		runWithAcceleration(actor, delta);
	}

	private void runWithAcceleration(CharacterBody2D actor, double delta)
	{
		//Set our acceleration, deceleration, and turn speed stats, based on whether we're on the ground on in the air
		acceleration = actor.IsOnFloor() ? MaxAcceleration : MaxAirAcceleration;
		deceleration = actor.IsOnFloor() ? MaxDeceleration : MaxAirDeceleration;
		turnSpeed = actor.IsOnFloor() ? MaxTurnSpeed : MaxAirTurnSpeed;

		if (directionX != 0)
		{
			//If the sign (i.e. positive or negative) of our input direction doesn't match our movement, it means we're turning around and so should use the turn speed stat.
			if (Mathf.Sign(directionX) != Mathf.Sign(velocity.X))
			{
				maxSpeedChange = turnSpeed;
			}
			else
			{
				//If they match, it means we're simply running along and so should use the acceleration stat
				maxSpeedChange = acceleration;
			}
		}
		else
		{
			maxSpeedChange = deceleration;
		}

		Vector2 NewVelocity = new Vector2(Mathf.MoveToward(velocity.X, desiredVelocity.X, maxSpeedChange), velocity.Y);
		actor.Velocity = NewVelocity;

	}
}