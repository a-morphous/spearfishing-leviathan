using Godot;

public class PlayerJumpBehavior : IActorBehavior
{
	public float JumpHeight = 96f; // in pixels
	public float UpwardMovementMultiplier = 1;
	public float DownwardMovementMultiplier = 1.5f;
	public float JumpCutoffMultiplier = 5f;
	public int MaxAirJumps = 0;
	public float VelocityLimit = 10000f;
	public float CoyoteTime = 0.15f;
	public float JumpBuffer = 0.15f;
	public bool UseVariableJumpHeight = true;

	private float jumpSpeed;
	private float defaultGravityScale = 1f;

	private bool canJumpAgain = false;
	private bool desiredJump;
	private float jumpBufferCounter;
	private float coyoteTimeCounter = 0;
	private bool pressingJump;
	public bool onGround;
	private bool currentlyJumping;
	private float gravityScale;

	private Vector2 velocity;

	public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public void Reset() {
		desiredJump = false;
		pressingJump = false;
	}
	
	public void Update(Actor actor, double delta)
	{
		velocity = actor.Velocity;

		// input
		if (Input.IsActionJustPressed("jump"))
		{
			desiredJump = true;
			pressingJump = true;
		} else if (Input.IsActionJustReleased("jump")) {
			pressingJump = false;
		}

		//Jump buffer allows us to queue up a jump, which will play when we next hit the ground
		if (JumpBuffer > 0)
		{
			//Instead of immediately turning off "desireJump", start counting up...
			//All the while, the DoAJump function will repeatedly be fired off
			if (desiredJump)
			{
				jumpBufferCounter += (float)delta;

				if (jumpBufferCounter > JumpBuffer)
				{
					//If time exceeds the jump buffer, turn off "desireJump"
					desiredJump = false;
					jumpBufferCounter = 0;
				}
			}
		}

		//If we're not on the ground and we're not currently jumping, that means we've stepped off the edge of a platform.
		//So, start the coyote time counter...
		if (!currentlyJumping && !actor.IsOnFloor())
		{
			coyoteTimeCounter += (float)delta;
		}
		else
		{
			//Reset it when we touch the ground, or jump
			coyoteTimeCounter = 0;
		}

		//Keep trying to do a jump, for as long as desiredJump is true
		if (desiredJump)
		{
			DoAJump(actor);
			actor.Velocity = velocity;

			//Skip gravity calculations this frame, so currentlyJumping doesn't turn off
			//This makes sure you can't do the coyote time double jump bug
			return;
		}

		calculateGravity(actor, delta);
		actor.Velocity = new Vector2(velocity.X, Mathf.Clamp(velocity.Y, -VelocityLimit, VelocityLimit));
	}

	private void calculateGravity(Actor actor, double delta)
	{
		if (actor.Velocity.Y < -0.01f)
		{
			if (actor.IsOnFloor())
			{
				actor.GravityFactor = defaultGravityScale;
			}
			else
			{
				if (UseVariableJumpHeight)
				{
					if (pressingJump && currentlyJumping)
					{
						actor.GravityFactor = UpwardMovementMultiplier;
					}
					else
					{
						actor.GravityFactor = JumpCutoffMultiplier;
					}
				}
				else
				{
					actor.GravityFactor = UpwardMovementMultiplier;
				}
			}
		}

		else if (actor.Velocity.Y > 0.01f)
		{

			if (actor.IsOnFloor())
			{
				actor.GravityFactor = defaultGravityScale;
			}
			else
			{
				actor.GravityFactor = DownwardMovementMultiplier;
			}

		}
		else
		{
			if (actor.IsOnFloor())
			{
				currentlyJumping = false;
			}

			actor.GravityFactor = defaultGravityScale;
		}

	}

	private void DoAJump(Actor actor)
	{

		if (actor.IsOnFloor() || (coyoteTimeCounter > 0.03f && coyoteTimeCounter < CoyoteTime) || canJumpAgain)
		{
			desiredJump = false;
			jumpBufferCounter = 0;
			coyoteTimeCounter = 0;

			//If we have double jump on, allow us to jump again (but only once)
			canJumpAgain = (MaxAirJumps == 1 && canJumpAgain == false);

			//Determine the power of the jump, based on our gravity and stats
			jumpSpeed = Mathf.Sqrt(2 * Gravity * JumpHeight);

			velocity.Y = -jumpSpeed;
			currentlyJumping = true;
			AudioStreamManager.Get(actor).Play("res://assets/sfx/jump.wav", -1);
		}

		if (JumpBuffer == 0)
		{
			//If we don't have a jump buffer, then turn off desiredJump immediately after hitting jumping
			desiredJump = false;
		}
	}
}