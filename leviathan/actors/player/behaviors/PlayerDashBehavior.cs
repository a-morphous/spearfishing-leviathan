using Godot;

public class PlayerDashBehavior : IActorBehavior
{
	public bool CanAirDash = false;
	public float DashSpeed = 800f;
	public float DashTime = 0.15f;
	public float DashCooldown = 0.5f;

	protected float dashTimer = 0f;
	protected float dashCooldownTimer = 0f;

	protected Vector2 velocity;

	protected bool previousDirection;

	public virtual void Update(Actor actor, double delta)
	{
		velocity = actor.Velocity;
		SetPreviousDirection();
		
		if (actor.State != Actor.ACTOR_STATES.DASHING)
		{
			if (dashCooldownTimer > 0)
			{
				dashCooldownTimer -= (float)delta;
			}
			else if (Input.IsActionJustPressed("dash") && (actor.IsOnFloor() || CanAirDash))
			{
				Dash(actor);
			}
		}
		else
		{
			SetDashSpeed();
			dashTimer += (float)delta;
			if (Input.IsActionJustReleased("dash"))
			{
				if (dashTimer < DashTime / 2f)
				{
					dashTimer = DashTime / 2f;
				}
			}

			if (dashTimer >= DashTime)
			{
				dashTimer = 0;
				actor.SetState(actor.PreviousState);
			}
		}
		actor.Velocity = velocity;
	}

	protected void SetPreviousDirection() {
		if (velocity.X > 0.01f)
		{
			previousDirection = true;
		}
		else if (velocity.X < -0.01f)
		{
			previousDirection = false;
		}
	}

	protected virtual void Dash(Actor actor)
	{
		actor.SetState(Actor.ACTOR_STATES.DASHING);
		AfterDash();
	}

	protected void AfterDash(){
		dashCooldownTimer = DashCooldown;
		dashTimer = 0f;
	}
	protected virtual void SetDashSpeed()
	{
		velocity.X = previousDirection ? DashSpeed : -DashSpeed;
	}
}

public class PlayerSwimDashBehavior: PlayerDashBehavior {

	public PlayerSwimDashBehavior() {
		DashSpeed = 800f;
		DashTime = 0.10f;
		DashCooldown = 0.5f;
	}

	public override void Update(Actor actor, double delta)
	{
		velocity = actor.Velocity;
		SetPreviousDirection();
		if (actor.State != Actor.ACTOR_STATES.SWIM_DASHING)
		{
			if (dashCooldownTimer > 0)
			{
				dashCooldownTimer -= (float)delta;
			}
			else if (Input.IsActionJustPressed("dash") && (actor.IsInWater))
			{
				Dash(actor);
			}
		}
		else
		{
			SetDashSpeed();
			dashTimer += (float)delta;
			if (Input.IsActionJustReleased("dash"))
			{
				if (dashTimer < DashTime / 2f)
				{
					dashTimer = DashTime / 2f;
				}
			}

			if (dashTimer >= DashTime)
			{
				dashTimer = 0;
				actor.SetState(Actor.ACTOR_STATES.SWIMMING);
			}
		}
		actor.Velocity = velocity;
	}

	protected override void Dash(Actor actor)
	{
		AudioStreamManager.Get(actor).Play("res://assets/sfx/swim_dash_long.wav", -9);
		actor.SetState(Actor.ACTOR_STATES.SWIM_DASHING);
		AfterDash();
	}

	protected override void SetDashSpeed()
	{
		if (Input.GetVector("move_left", "move_right", "move_up", "move_down").LengthSquared() == 0f) {
			// base it on previous direction
			base.SetDashSpeed();
			return;
		}
		float direction = Mathf.Snapped(Input.GetVector("move_left", "move_right", "move_up", "move_down").Angle(), Mathf.DegToRad(45));
		Vector2 dir = Vector2.FromAngle(direction);
		velocity = dir * DashSpeed;
	}
}