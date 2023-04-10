using Godot;

public class PlayerSwimmingBehavior : IActorBehavior
{

	public float MaxSpeed = 300f;
	public float MaxAcceleration = 30f;
	public float MaxDeceleration = 30f;
	public float MaxTurnSpeed = 80f;

	private Vector2 velocity;
	private Vector2 desiredVelocity;

	private Vector2 moveDirection;
	private float maxSpeedChange;

	public void Update(Actor actor, double delta)
	{
		velocity = actor.Velocity;
		actor.GravityFactor = 1.5f;

		moveDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (!actor.IsInWater)
		{
			moveDirection.Y = 0;
		}

		desiredVelocity = moveDirection * Mathf.Max(MaxSpeed, 0f);

		if (!actor.IsInWater && actor.IsOnFloor())
		{
			actor.SetState(Actor.ACTOR_STATES.DEFAULT);
		}
		else
		{
			swimWithAcceleration((Player)actor, delta);
		}
	}

	private void swimWithAcceleration(Player actor, double delta)
	{
		if (moveDirection.X != 0)
		{
			if (Mathf.Sign(moveDirection.X) != Mathf.Sign(velocity.X))
			{
				maxSpeedChange = MaxTurnSpeed;
			}
			else
			{
				maxSpeedChange = MaxAcceleration;
			}
		}
		else
		{
			maxSpeedChange = MaxDeceleration;
		}

		Vector2 NewVelocity = new Vector2(Mathf.MoveToward(velocity.X, desiredVelocity.X, maxSpeedChange), Mathf.MoveToward(velocity.Y, desiredVelocity.Y, maxSpeedChange));
		if (!actor.IsInWater)
		{
			NewVelocity.Y = velocity.Y; // don't have drag when you're actually in the air...
		}
		if (actor.playerHead.CanBreathe && moveDirection.Y < -0.5)
		{
			NewVelocity.Y = -800; // jump out of the water
		}

		actor.Velocity = NewVelocity;

	}
}

public class PlayerToggleSwimBehavior : IActorBehavior
{
	public void Update(Actor actor, double delta)
	{
		if (Input.IsActionJustPressed("jump"))
		{
			if (actor.IsInWater && !actor.IsOnFloor() && actor.State != Actor.ACTOR_STATES.SWIMMING)
			{
				Vector2 velocity = actor.Velocity;
				velocity /= 3;
				actor.Velocity = velocity;
				actor.SetState(Actor.ACTOR_STATES.SWIMMING);

			}
			else
			{
				actor.SetState(Actor.ACTOR_STATES.DEFAULT);
			}
		}
	}
}